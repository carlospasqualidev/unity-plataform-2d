using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXController : MonoBehaviour
{
    public static SFXController instance;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip[] audioClips, float volume = 1f)
    {
        if (audioClips.Length == 0)
            return;

        int randomIndex = Random.Range(0, audioClips.Length);
        audioSource.PlayOneShot(audioClips[randomIndex], volume);
    }
}
