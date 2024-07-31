using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;


public class PlayerController : MonoBehaviour
{
    private enum RigAnimMode
    {
        off,
        inc,
        dec,
    }
    
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = 9.8f; // Adjust this value as needed
    private CharacterController characterController;
    private Camera cam;
    private Vector3 velocity;
    public Vector3 MoveDirection;
    public Animator animator;

    [Header("Input")]
    public bool input_A_Flag; // A
    private float input_A_Flag_Timer;

    [Header("Stop Movement")]
    public bool canMove;
    public bool interacting;
    public bool interacting_Salute;
    public bool interacting_Win;

    [Header("Target and Rotation")]
    public Transform targetTransform;
    public float rotationSpeed = 1.0f; // Angular speed in radians per second.
    public float kickSpeed = 1.0f;
    public ChainIKConstraint leg;
    public Transform kickTarget;
    private RigAnimMode rigAnimMode;

    [Header("Change Player Locaiton")]
    public Transform originalTransform;
    public Transform onsenTransform;
    public bool inOnsen;

    public bool facingFrog;
    public bool kickingFrog;
    private void Start()
    {
        canMove = true;
        characterController = GetComponent<CharacterController>();
        cam = FindAnyObjectByType<Camera>();
        animator = GetComponent<Animator>();
        
        leg.weight = 0;

        
    }
    
    private void Update()
    {
        float delta = Time.deltaTime;
        Controls(delta);
        Movement();
        Animate();
        Rigging();
        
        if (interacting)
        {
            RotateTowardsTarget(delta);
        }
        
        if (facingFrog) 
        {
            RotateTowardsTarget(delta);
        }
    }

    private void Rigging()
    {
        switch (rigAnimMode)
        {
            case RigAnimMode.inc:
                leg.weight = Mathf.Lerp(leg.weight, 1, kickSpeed * Time.deltaTime);
                if (leg.weight > 0.95f)
                {
                    leg.weight = 1;
                    rigAnimMode = RigAnimMode.dec;
                }
                break;
            case RigAnimMode.dec:
                leg.weight = Mathf.Lerp(leg.weight, 0, kickSpeed * Time.deltaTime);
                if (leg.weight < 0.1f)
                {
                    leg.weight = 0;
                    rigAnimMode = RigAnimMode.off;
                }
                break;
        }
    }
    private void Animate()
    {
        if (facingFrog && kickingFrog)
        {
            animator.Play("Kick");
            return;
        }
        if (interacting_Salute && interacting)
        {
            animator.Play("Salute");
            return;
        }
        if (interacting_Win && interacting)
        {
            animator.Play("Win");
            return;
        }
        if (MoveDirection.sqrMagnitude < 0.2f) 
        {
            animator.Play("Idle");
        }
        else
        {
            animator.Play("Run");
        }
    }
    private void Controls(float delta)
    {
        if (Input.GetButtonDown("Fire1"))
        {
            input_A_Flag = true;
            input_A_Flag_Timer = 0;
        }
        if (input_A_Flag)
        {
            input_A_Flag_Timer += delta;
            if (input_A_Flag_Timer >= 0.1f)
            {
                input_A_Flag = false;
            }
        }
    }
    private void Movement()
    {
        if (interacting)
        {
            return;
        }

        if (canMove)
        {
            // Get input for movement
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            // Calculate movement direction relative to camera
            Vector3 cameraForward = cam.transform.forward;
            Vector3 cameraRight = cam.transform.right;
            cameraForward.y = 0f;
            cameraRight.y = 0f;
            MoveDirection = (cameraForward * verticalInput + cameraRight * horizontalInput).normalized;

            // Apply gravity
            velocity.y -= gravity * Time.deltaTime;

            // Check if player movement is close to zero
            if (MoveDirection.sqrMagnitude < 0.2f)
            {
                // Stop player movement
                velocity = Vector3.zero;
                
                characterController.Move((Vector3.zero * 0 + velocity) * Time.deltaTime);
            }
            else
            {
                // Move the player
                characterController.Move((MoveDirection * moveSpeed + velocity) * Time.deltaTime);

                // Make the player always face the direction of movement
                transform.LookAt(transform.position + MoveDirection);
            }
        }
        else
        {
            MoveDirection = Vector3.zero;
            // Stop player movement
            velocity = Vector3.zero;
            characterController.Move((Vector3.zero * 0 + velocity) * Time.deltaTime);

            if (inOnsen)
            {
                animator.Play("Onsen");
                Debug.Log("Onsen Anim");

                return;
            }

            if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animator.Play("Idle");
            }

        }
    }
    private void RotateTowardsTarget(float delta)
    {
        // Determine the direction to rotate towards.
        Vector3 targetDirection = targetTransform.position - transform.position;

        // Calculate the step size based on the rotation speed and frame time.
        float step = rotationSpeed * delta;

        // Rotate the forward vector towards the target direction by one step.
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, step, 0.0f);

        // Update the rotation of this object to face the new direction.
        var rotation = Quaternion.LookRotation(newDirection);
        
        transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
        // transform.rotation = rotation;
    }
    public void StopInteraction()
    {
        interacting = false;
        interacting_Salute = false;
        interacting_Win = false;
        facingFrog = false;
        kickingFrog = false;
    }
    private void OnTriggerStay(Collider other)
    {
        // Get the looking angle
        float angle = Vector3.Angle(transform.forward, other.transform.position - transform.position);
        
        if (other.CompareTag("Interact"))
        {
            Interact theOther = other.GetComponent<Interact>();

            if (theOther.switchCamera)
            {
                theOther.Interaction(true);
            }
            else
            {
                if (theOther.targetInteraction)
                {
                    interacting_Win = theOther.winInteraction;
                    interacting_Salute = theOther.saluteInteraction;
                    targetTransform = theOther.targetTransform;
                }
                if (input_A_Flag)
                {
                    if (theOther.targetInteraction && theOther.canInteractWithTarget)
                    {
                        interacting = true;
                        theOther.TargetInteraction();
                    }
                    if (theOther.fadeInAndOut)
                    {
                        canMove = false;
                    }

                    if (theOther.onsen)
                    {
                        originalTransform = transform;
                        onsenTransform = theOther.onsenTransform;

                        canMove = false;
                    }
                    theOther.Interaction(false);
                }
            }
        }

        if (other.CompareTag("Frog") && angle < 30)
        {
            KickFrog theOther = other.GetComponent<KickFrog>();
            facingFrog = true;
            targetTransform = theOther.transform;
            theOther.HighlightFrog();
            if (input_A_Flag)
            {
                kickingFrog = true;
                Invoke(nameof(RetargetKick), 1.15f);
                theOther.isKicked = true;
                canMove = false;
                Invoke(nameof(EnableMovement), 5f);
            }
        }

        if (other.CompareTag("Sleep"))
        {
            Sleep theOther = other.GetComponent<Sleep>();
            if (input_A_Flag)
            {
                theOther.sleep = true;
            }
        }
    }
    
    private void EnableMovement()
    {
        canMove = true;
    }

    private void RetargetKick()
    {
        leg.weight = 0;
        kickTarget.position = targetTransform.position;
        rigAnimMode = RigAnimMode.inc;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interact"))
        {
            Interact theOther = other.GetComponent<Interact>();

            if (theOther.switchCamera)
            {
                theOther.Interaction(false);
            }
        }

        if (other.CompareTag("Frog"))
        {
            facingFrog = false;
            kickingFrog = false;
            KickFrog theOther = other.GetComponent<KickFrog>();
            theOther.UnhighlightFrog();
        }
    }

    public void InOnsen()
    {
        transform.position = onsenTransform.position;
        transform.transform.rotation = onsenTransform.transform.rotation;
    }
}
