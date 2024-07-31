using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KickFrog : MonoBehaviour
{
    public OutlineFx.OutlineFx outlineFx;
    public AudioSource audioSource; // Attach an AudioSource component to this GameObject
    public AudioClip audioClip; // Add your audio clips to this array
    public AudioClip audioClipFly; // Add your audio clips to this array


    TaskManager taskManager;
    PlayerController controller;
    Rigidbody rb;
    public float force;
    public bool isKicked;
    bool isKickingDone;
    float delayInSeconds = 2.5f; // Set the desired delay
    float actionDelay = 1.2f;

    private void Awake()
    {
        taskManager = FindAnyObjectByType<TaskManager>();
        controller = FindAnyObjectByType<PlayerController>();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (!outlineFx)
        {
            outlineFx = GetComponentInChildren<OutlineFx.OutlineFx>();
        }
        
        if (outlineFx) outlineFx.enabled = false;
    }
    void FixedUpdate()
    {
        if (isKicked)
        {
            if (actionDelay > 0)
            {
                actionDelay -= Time.deltaTime;
            }
            else
            {
                Vector3 directionToTarget = controller.transform.position - transform.position;
                Vector3 oppositeDirection = -directionToTarget.normalized;

                // Apply force to move away from the target
                rb.AddForce(oppositeDirection * force, ForceMode.Impulse);

                if (!isKickingDone)
                {
                    audioSource.PlayOneShot(audioClip);
                    audioSource.PlayOneShot(audioClipFly);
                    taskManager.ConfirmTaskCompletion(2);
                    isKickingDone = true;
                }
                delayInSeconds -= Time.deltaTime;
                if (delayInSeconds < 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
    
    public void HighlightFrog()
    {
        // Highlight the frog
        if(outlineFx) outlineFx.enabled = true;
    }
    
    public void UnhighlightFrog()
    {
        // Unhighlight the frog
        if(outlineFx) outlineFx.enabled = false;
    }
}
