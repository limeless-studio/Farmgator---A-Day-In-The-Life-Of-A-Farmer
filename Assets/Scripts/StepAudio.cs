using UnityEngine;
using UnityEngine.Audio;

public class StepAudio : MonoBehaviour
{
    public AudioSource audioSource; // Attach an AudioSource component to this GameObject
    public AudioClip[] audioClip; // Add your audio clips to this array
    public void PlaySound()
    {
        AudioClip clip = audioClip[UnityEngine.Random.Range(0, audioClip.Length)];
        audioSource.PlayOneShot(clip);
    }
}
