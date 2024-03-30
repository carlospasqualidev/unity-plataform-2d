using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXController : MonoBehaviour
{
    public static SFXController instance;

    private AudioSource audioSource;

    [Header("HIT SOUNDS")]
    [SerializeField]
    private AudioClip[] hitSounds;

    [SerializeField]
    private AudioClip[] jumpSounds;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayHitSound(float volume = 1f)
    {
        PlaySound(hitSounds, volume);
    }

    public void PlayJumpSound(float volume = 1f)
    {
        PlaySound(jumpSounds, volume);
    }

    public void PlaySound(AudioClip[] audioClips, float volume = 1f)
    {
        if (audioClips.Length == 0)
            return;

        int randomIndex = Random.Range(0, audioClips.Length);
        audioSource.PlayOneShot(audioClips[randomIndex], volume);
    }
}
