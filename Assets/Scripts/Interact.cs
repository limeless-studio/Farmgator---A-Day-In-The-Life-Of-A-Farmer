using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public OutlineFx.OutlineFx[] outlineFxs;
    public AudioSource audioSource; // Attach an AudioSource component to this GameObject
    public AudioClip[] audioClip; // Add your audio clips to this array
    public AudioClip audioClipCoin; // Add your audio clips to this array
    public AudioClip audioClipHarvest; // Add your audio clips to this array
    public AudioClip audioClipDoor; // Add your audio clips to this array

    PlayerController playerController;
    GameManager gameManager;
    public bool fadeInAndOut;
    private bool startFadeInAndOut;
    private bool endFadeInAndOut;
    private float fadeInAndOutTimer = 2.3f;
    private float fadeInAndOutTime;
    public bool destroyOnInteract;
    public GameObject destroyEffect;
    DayNightCycle dayNightCycle;

    [Header("Change Player Locaiton")]
    public bool onsen;
    public Transform onsenTransform;
    public bool inOnsenInteract;
    public bool inOnsen;
    private float inOnsenTimer = 2.3f;
    private float inOnsenTime;
    private float outOnsenTimer = 2.3f;
    private float outOnsenTime;
    public bool outOnsen;



    [Header("Task")]
    public bool taskCompleteTrigger;
    TaskManager taskManager;
    public int taskNumber;
    bool taskComplete;

    [Header("Target")]
    public bool targetInteraction;
    public bool saluteInteraction;
    public bool winInteraction;
    public bool canInteractWithTarget;
    public bool isMouse;
    public Transform targetTransform;

    [Header ("Animation")]
    public bool animate;
    Animator animator;
    public bool singleInteraction;
    bool interacted;
    bool playedForward;

    [Header("Animation")]
    public bool switchCamera;
    public GameObject cameraToSwitchTo;

    public bool isCoin;
    public bool isFish;
    public bool isDoor;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        playerController = FindAnyObjectByType<PlayerController>();  
        dayNightCycle = FindAnyObjectByType<DayNightCycle>();
        taskManager = FindAnyObjectByType<TaskManager>();
        
        if (outlineFxs.Length == 0)
            outlineFxs = GetComponentsInChildren<OutlineFx.OutlineFx>();
        
        foreach (var outline in outlineFxs)
            outline.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if (targetInteraction)
        {
            canInteractWithTarget = true;
        }
    }
    private void Update()
    {
        if (startFadeInAndOut)
        {
            if (fadeInAndOutTime > 0)
            {
                fadeInAndOutTime -= Time.deltaTime;
            }
            else
            {
                if (!endFadeInAndOut)
                {
                    CompleteTask();
                    endFadeInAndOut = true;
                    startFadeInAndOut = false;
                }
            }
        }

        if (inOnsenInteract)
        {
            if (inOnsenTime > 0)
            {
                inOnsenTime -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Change Player Position");
                if (!inOnsen)
                {
                    gameManager.DecreaseOpacity();
                    inOnsen = true;
                    playerController.inOnsen = true;
                    playerController.InOnsen();
                    inOnsenInteract = false;
                    Debug.Log("Changeed Position");
                }
            }
        }

        if (inOnsen)
        {
            if (outOnsenTime > 0)
            {
                outOnsenTime -= Time.deltaTime;
            }
            else
            {
                playerController.transform.position = playerController.originalTransform.position;

                gameManager.DecreaseOpacity();
                outOnsen = true;
                playerController.inOnsen = false;
                playerController.canMove = true;
                CompleteTask();
            }
        }
    }
    
    public void Highlight()
    {
        foreach (var outline in outlineFxs)
            outline.enabled = true;
    }
    
    public void Unhighlight()
    {
        foreach (var outline in outlineFxs)
            outline.enabled = false;
    }

    public void Interaction(bool status)
    {
        if (animate)
        {
            if (isDoor)
            {
                audioSource.PlayOneShot(audioClipDoor);
                isDoor = false;
            }
            Animate();
        }

        if (taskCompleteTrigger)
        {
            CompleteTask();
        }

        if(switchCamera)
        {
            SwtichCamera(status);
        }

        if (onsen)
        {
            if(inOnsen)
            {
                OutOnsen();
            }
            else
            {
                Onsen();
            }
        }
    }

    public void CompleteTask()
    {
        if (!taskComplete)
        {
            if (isCoin)
            {
                audioSource.PlayOneShot(audioClipCoin);
                isCoin = false;
            }
            if (isFish)
            {
                audioSource.PlayOneShot(audioClipHarvest);
                isFish = false;
            }
            
            if(fadeInAndOut && !startFadeInAndOut)
            {
                gameManager.IncreaseOpacity();
                startFadeInAndOut = true;
                fadeInAndOutTime = fadeInAndOutTimer;
            }

            if (fadeInAndOutTime> 0)
            {
                return;
            }
            taskManager.ConfirmTaskCompletion(taskNumber);
            taskComplete = true;
            dayNightCycle.AdvanceTime();
            if (destroyOnInteract)
            {
                if (fadeInAndOut)
                {
                    Debug.Log("test");
                    gameManager.DecreaseOpacity();
                    fadeInAndOut = false;
                    playerController.canMove = true;
                }
                if (destroyEffect)
                {
                    Instantiate(destroyEffect, transform.position + Vector3.up * .5f, Quaternion.identity);
                }
                StartCoroutine(DestroyRoutine());
            }
            if (fadeInAndOut)
            {
                Debug.Log("test");
                gameManager.DecreaseOpacity();
                fadeInAndOut = false;
                playerController.canMove = true;
            }
        }
    }
    
    IEnumerator DestroyRoutine()
    {
        // Scale down the object and move towards the player
        Transform player = PlayerController.Instance.transform;
        float bp = 1f;
        AnimationCurve curve = AnimationCurve.EaseInOut(0, bp, 1, bp);
        curve.AddKey(0.5f, bp + .5f);
        while (transform.localScale.magnitude > 0.01f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, Time.deltaTime * 2);
            var pos = player.position + Vector3.up * curve.Evaluate(1 - (transform.position - player.position).magnitude / 10);
            transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 2);
            yield return null;
        }
        
        // Destroy the object
        Destroy(gameObject);
    }
    
    public void Animate()
    {
  
        if (singleInteraction)
        {
          
            if (!interacted)
            {
                animator.Play("Play");
                interacted = true; 
            }
        }
        else
        {
            if (!playedForward)
            {
                animator.Play("PlayF");
                playedForward = true;
            }
            else
            {
                animator.Play("PlayB");
                playedForward = false;
            }
        }
    }
    public void SwtichCamera(bool status)
    {
        if(status)
        {
            cameraToSwitchTo.SetActive(true);
        }
        else
        {
            cameraToSwitchTo.SetActive(false);
        }
    }

    public void TargetInteraction()
    {
        if(canInteractWithTarget)
        {
            targetTransform.GetComponent<Animator>().Play("Interact");
            canInteractWithTarget = false;
        }
    }

    public void Onsen()
    {
        inOnsenInteract = true;
        gameManager.IncreaseOpacity();
        inOnsenTime = inOnsenTimer;
    }
    public void OutOnsen()
    {
        gameManager.IncreaseOpacity();
        outOnsenTime = outOnsenTimer;
    }

    public void PlaySound()
    {
        AudioClip clip = audioClip[UnityEngine.Random.Range(0, audioClip.Length)];
        audioSource.PlayOneShot(clip);
    }
}