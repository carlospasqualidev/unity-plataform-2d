using UnityEngine;

public class PlaySoundOnCollision : MonoBehaviour
{
    [SerializeField]
    AudioClip[] audioClips;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SFXController.instance.PlaySound(audioClips);
        }
    }
}
