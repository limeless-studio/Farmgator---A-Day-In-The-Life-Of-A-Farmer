using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayThisAudio : MonoBehaviour
{
    public AudioSource audioSource; // Attach an AudioSource component to this GameObject
    public AudioClip audioClip; // Add your audio clips to this array
    // Start is called before the first frame update
    public void PlayAudio()
    {
        audioSource.PlayOneShot(audioClip);
    }
}
