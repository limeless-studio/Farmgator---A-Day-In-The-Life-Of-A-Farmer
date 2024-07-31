using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskManager : MonoBehaviour
{
    public AudioSource audioSource; // Attach an AudioSource component to this GameObject
    public AudioClip[] audioClip; // Add your audio clips to this array
    public Image[] taskList;
    public bool[] taskListConfirmation;
    public Sprite[] sunList;
    public Image currentSunImage;
    int sunImageCount = 0;

    public Sprite[] wheelList;
    public Image currentWheelImage;

    // Start is called before the first frame update
    void Start()
    {
        currentSunImage.sprite = sunList[0];
    }

    public void ConfirmTaskCompletion(int i)
    {
        sunImageCount++;
        currentSunImage.sprite = sunList[sunImageCount];

        currentWheelImage.sprite = wheelList[sunImageCount];
        i -= 1;

        AudioClip clip = audioClip[UnityEngine.Random.Range(0, audioClip.Length)];
        audioSource.PlayOneShot(clip);

        if (taskListConfirmation[i])
        {
            return;
        }
        else
        {
            taskListConfirmation[i] = true;
            taskList[i].GetComponent<Animator>().Play("Play");
        }
    }
}
