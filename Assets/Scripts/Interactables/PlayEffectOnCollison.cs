using UnityEngine;

public class PlayEffectOnCollison : MonoBehaviour
{
    [SerializeField]
    ParticleSystem particle;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            particle.Play();
        }
    }
}
