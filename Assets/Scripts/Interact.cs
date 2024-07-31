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
        
        if (outlineFxs.Length > 0) 
            foreach (var outline in outlineFxs)
                outline.enabled = status;
    }

    public void CompleteTask()
    {
        if (!taskComplete)
        {
            if(fadeInAndOut && !startFadeInAndOut)
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
                Destroy(gameObject);
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