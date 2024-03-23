using System;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Vector2 _deadZoneThresholds = new(0.1f, 0.1f); //Limite de zona morta

    [SerializeField]
    private LayerMask _playerLayer;

    private Rigidbody2D _rb; //Rigidbody do jogador
    private CapsuleCollider2D _col; //Colisor do jogador
    private FrameInput _frameInput; //Entrada do frame
    private Vector2 _frameVelocity; //Velocidade do frame
    private Animator _animator; //Animator do jogador
    private SpriteRenderer _spriteRenderer; //SpriteRenderer do jogador
    private bool _cachedQueryStartInColliders; //Cache do componente Physics2D usado para detectar colisões
    private float _time;
    private ParticleSystem _dustParticles; //Partículas de poeira

    [Header("JUMP")]
    [SerializeField]
    private float _jumpPower = 36f;

    [SerializeField]
    private float _coyoteTime = 0.15f;

    [SerializeField]
    private float _jumpBuffer = 0.2f; //quantidade de tempo que armazenamos um salto. Isso permite a entrada do salto antes de realmente atingir o solo
    private bool _jumpToConsume; //Flag de pulo a ser consumido
    private bool _coyoteUsable; //Flag de 'Coyote Time' a ser consumido
    private bool _grounded; //Flag de chão
    private bool _bufferedJumpUsable; //Flag de pulo bufferizado a ser consumido
    private bool _endedJumpEarly; //Flag de pulo encerrado prematuramente
    private float _timeJumpWasPressed; //Tempo em que o pulo foi pressionado
    private float _frameLeftGrounded = float.MinValue; //Tempo em que o jogador deixou o chão

    [Header("FORCES")]
    [SerializeField]
    private float _groundDeceleration = 60f;

    [SerializeField]
    private float _airDeceleration = 30f;

    [SerializeField]
    private float _acceleration = 120f;

    [SerializeField]
    private float _maxSpeed = 14f;

    [SerializeField]
    private float _fallAcceleration = 110; //Aceleração de queda

    [SerializeField]
    private float _maxFallSpeed = 40; //Velocidade máxima de queda

    [SerializeField]
    private float _jumpEndEarlyGravityModifier = 3; //Modificador de gravidade para pulo encerrado prematuramente

    [SerializeField]
    private float _groundingForce = -1.5f; //Força de aterrisagem

    [SerializeField]
    private float _grounderDistance = 0.05f; //Distância do chão

    // Implementação da interface IPlayerController
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>(); //Pega o componente Rigidbody2D
        _col = GetComponent<CapsuleCollider2D>(); //Pega o componente CapsuleCollider2D
        _animator = GetComponent<Animator>(); //Pega o componente Animator
        _spriteRenderer = GetComponent<SpriteRenderer>(); //Pega o componente SpriteRenderer
        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders; //Pega o componente Physics2D
        _dustParticles = GetComponentInChildren<ParticleSystem>(); //Pega o componente ParticleSystem
    }

    void Update()
    {
        _time += Time.deltaTime; //Atualiza o tempo
        GetPlayerInput();
    }

    void FixedUpdate()
    {
        CheckCollisions();
        HandleJump();
        HandleGravity();
        HandleDirection();
        ApplyMovement();
        HandleAnimations();
        DustParticles();
    }

    #region  MOVEMENT

    private void GetPlayerInput()
    {
        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.C), // Verifica se o botão de pulo foi pressionado neste frame
            JumpHeld = Input.GetButton("Jump") || Input.GetKey(KeyCode.C), // Verifica se o botão de pulo está sendo mantido pressionado neste frame
            Move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) // Obtém as entradas de movimento horizontal e vertical
        };

        PreventSnapInput();

        // Se o botão de pulo foi pressionado, ativa a flag para consumir o pulo e registra o tempo em que foi pressionado
        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
    }

    private void PreventSnapInput()
    {
        _frameInput.Move.x =
            Mathf.Abs(_frameInput.Move.x) < _deadZoneThresholds.x
                ? 0
                : Mathf.Sign(_frameInput.Move.x);
        _frameInput.Move.y =
            Mathf.Abs(_frameInput.Move.y) < _deadZoneThresholds.y
                ? 0
                : Mathf.Sign(_frameInput.Move.y);
    }

    private void ApplyMovement() => _rb.velocity = _frameVelocity;

    #endregion

    #region GRAVITY

    private void HandleGravity()
    {
        // Lógica para lidar com a gravidade
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _groundingForce;

            return;
        }

        // Gravidade aplicada enquanto estiver no ar, com ajustes se o pulo for encerrado prematuramente
        var inAirGravity = _fallAcceleration;
        if (_endedJumpEarly && _frameVelocity.y > 0)
        {
            inAirGravity *= _jumpEndEarlyGravityModifier;
        }

        _frameVelocity.y = Mathf.MoveTowards(
            _frameVelocity.y,
            -_maxFallSpeed,
            inAirGravity * Time.fixedDeltaTime
        );
    }

    #endregion

    #region COLLISIONS

    // Variáveis relacionadas a colisões

    private void CheckCollisions()
    {
        // Desativa temporariamente as queries para iniciar em colliders (otimização)
        Physics2D.queriesStartInColliders = false;

        // Verifica colisões com o chão e teto usando um CapsuleCast
        bool groundHit = Physics2D.CapsuleCast(
            _col.bounds.center,
            _col.size,
            _col.direction,
            0,
            Vector2.down,
            _grounderDistance,
            ~_playerLayer
        );
        bool ceilingHit = Physics2D.CapsuleCast(
            _col.bounds.center,
            _col.size,
            _col.direction,
            0,
            Vector2.up,
            _grounderDistance,
            ~_playerLayer
        );

        // Se atingiu um teto, ajusta a velocidade vertical para ser no máximo zero
        if (ceilingHit)
            _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Se aterrissou no chão, atualiza as flags e invoca o evento GroundedChanged
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Se deixou o chão, atualiza as flags e invoca o evento GroundedChanged
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        // Restaura o valor original das queries para iniciar em colliders
        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion

    #region JUMPING


    // Propriedades para verificar se um pulo foi bufferizado ou se o jogador pode usar um 'Coyote Time'
    private bool HasBufferedJump =>
        _bufferedJumpUsable && _time < _timeJumpWasPressed + _jumpBuffer;
    private bool CanUseCoyote =>
        _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _coyoteTime;

    private void HandleJump()
    {
        // Se o pulo foi encerrado prematuramente, ajusta a flag correspondente
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0)
            _endedJumpEarly = true;

        // Se não há pulo a ser consumido e não há pulo bufferizado, retorna
        if (!_jumpToConsume && !HasBufferedJump)
            return;

        // Executa o pulo se estiver no chão ou se o 'Coyote Time' estiver ativo
        if (_grounded || CanUseCoyote)
            ExecuteJump();

        // Reseta a flag de pulo a ser consumido
        _jumpToConsume = false;
    }

    private void ExecuteJump()
    {
        // Reseta algumas flags relacionadas ao pulo
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        // Aplica a força vertical do pulo e invoca o evento Jumped
        _frameVelocity.y = _jumpPower;
        Jumped?.Invoke();
    }

    #endregion

    #region DIRECTION

    private void HandleDirection()
    {
        // Lógica para lidar com a direção horizontal do jogador
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? _groundDeceleration : _airDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(
                _frameVelocity.x,
                0,
                deceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            _frameVelocity.x = Mathf.MoveTowards(
                _frameVelocity.x,
                _frameInput.Move.x * _maxSpeed,
                _acceleration * Time.fixedDeltaTime
            );
            _spriteRenderer.flipX = _frameInput.Move.x < 0; // inverte o sprite do jogador
        }
    }

    #endregion

    #region ANIMATIONS

    private void HandleAnimations()
    {
        // animacoes de corrida
        _animator.SetFloat("velocity_x", Mathf.Abs(_frameVelocity.x)); // seta a velocidade para contorlar a animação

        _animator.SetBool("is_grounded", _grounded); // seta a flag de chão para controlar a animação
    }

    private void DustParticles()
    {
        if (Mathf.Abs(_frameInput.Move.x) > 0 && _grounded)
        {
            _dustParticles.Play();
        }
        else
        {
            _dustParticles.Stop();
        }
    }

    #endregion


    public struct FrameInput //Struct de entrada do frame para o jogador
    {
        public bool JumpDown; //Flag de pulo
        public bool JumpHeld; //Flag de pulo mantido
        public Vector2 Move; //Entrada de movimento
    }
}
