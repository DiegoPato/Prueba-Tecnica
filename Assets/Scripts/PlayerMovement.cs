using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Input and movement variables
    PlayerInput playerInput;
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    float appliedYMovement; 
    bool isMovementPressed;

    // Character controller and camera
    public CharacterController controller;
    public Transform cam;

    // Speed and acceleration parameters
    [SerializeField] private float MaxSpeed = 20f;
    [SerializeField] private float Acceleration = 50f;
    private float currentSpeed = 0;

    // Gravity variables
    private float gravity = -9.8f;
    private float groundedGravity = -0.05f;
    private bool isFalling;

    // Jump variables
    private bool isJumpPressed = false;
    private float initialJumpVelocity;
    private float maxJumpHeight  = 8f;
    private float maxJumpTime = 1f;
    private bool isJumping = false;
    [SerializeField] private AudioSource jumpAudio;

    // Miscellaneous variables
    [SerializeField] private bool fallClamp = false;
    private float coyoteTime = 0.2f;
    private float coyoteTimer;
    Queue<KeyCode> inputBuffer;
    private List<IInteractable> interactables = new List<IInteractable>();
    private string mapScene = "DemoScene_Forest";

    // Respawn position
    private Vector3 respawnPosition;

    // Rotation variables
    [SerializeField] private float turnSmoothTime = 0.1f;
    private float turnSmoothvelocity;

    void Awake()
    {
        playerInput = new PlayerInput();
        controller = GetComponent<CharacterController>();

        // Input event subscriptions
        playerInput.Movement.Move.started += onMovementInput;
        playerInput.Movement.Move.canceled += onMovementInput;
        playerInput.Movement.Move.performed += onMovementInput;
        playerInput.Movement.Jump.started += onJumpInput;
        playerInput.Movement.Jump.canceled += onJumpInput;
        playerInput.Movement.Respawn.started += onRespawnInput;
        playerInput.Movement.Interact.started += onInteractInput;
        playerInput.Movement.ExitGame.started += onExitGameInput;

        inputBuffer = new Queue<KeyCode>();
        respawnPosition = this.transform.position;

        setupJumpvariables();
    }

    private void setupJumpvariables()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }

    private void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>(); 
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    }

    private void onJumpInput(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
        if (context.started)
        {
            inputBuffer.Enqueue(KeyCode.Space);
            Invoke("DequeueAction", 0.15f);
        }
    }

    private void onRespawnInput(InputAction.CallbackContext context)
    {
        Teleport(respawnPosition);
    }

    private void onInteractInput(InputAction.CallbackContext context)
    {
        foreach (IInteractable interactable in interactables)
        {
            interactable.interact();
        }
    }
    
    private void onExitGameInput(InputAction.CallbackContext context)
    {
        Application.Quit();
    }

    private void DequeueAction()
    {
        if(inputBuffer.Count > 0)
            inputBuffer.Dequeue();
    }

    private void OnEnable()
    {
        playerInput.Movement.Enable();
    }

    private void OnDisable()
    {
        playerInput.Movement.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        MoveUpdate();
        HandleGravity();
        HandleJump();
    }

    private void MoveUpdate()
    {
        Vector3 Movement = new Vector3(0f, appliedYMovement, 0f);
        
        // Calculate target speed based on acceleration
        currentSpeed = Mathf.MoveTowards(currentSpeed, MaxSpeed, Acceleration * Time.deltaTime);

        if(isMovementPressed)
        {
            Vector3 direction3d = new Vector3(currentMovement.x, 0f, currentMovement.z);
            float targetAngle = Mathf.Atan2(direction3d.x, direction3d.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            Movement.x = moveDir.x * currentSpeed;
            Movement.z = moveDir.z * currentSpeed;
        }
        else
        {
            currentSpeed = 0f;
        }
        controller.Move(Movement * Time.deltaTime);
    }

    private void HandleRotation(){
        if(currentMovementInput.magnitude >= 0.1f)
        {
            Vector3 direction3d = new Vector3(currentMovement.x, 0f, currentMovement.z);
            float targetAngle = Mathf.Atan2(direction3d.x, direction3d.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothvelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        }
    }

    private void HandleGravity()
    {
        isFalling = currentMovement.y <= 0.0f || !isJumpPressed;
        float fallMultiplier = 2.0f;
        if (controller.isGrounded && isFalling) { 
            appliedYMovement = groundedGravity;
            coyoteTimer = coyoteTime;
        } else if (isFalling) { 
            coyoteTimer -= Time.deltaTime;
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * fallMultiplier * Time.deltaTime);
            if (!fallClamp)
                appliedYMovement = (previousYVelocity + currentMovement.y) * .5f;
            else
                appliedYMovement = Mathf.Max((previousYVelocity + currentMovement.y) * .5f, -20.0f);
            
        } else {
            float previousYVelocity = currentMovement.y;
            currentMovement.y = currentMovement.y + (gravity * Time.deltaTime);
            appliedYMovement = (previousYVelocity + currentMovement.y) * 0.5f;
        }
    }

    private void HandleJump(){
        if (!isJumping && (controller.isGrounded || coyoteTimer > 0.0f) && isJumpPressed)
        {
            isJumping = true;
            if(jumpAudio != null)
                jumpAudio.Play();
            currentMovement.y = initialJumpVelocity;
        } 
        else if (isJumping && inputBuffer.Count > 0 && controller.isGrounded)
        {
            if(inputBuffer.Count > 0)
            {
                if (inputBuffer.Peek() == KeyCode.Space)
                {
                    isJumping = true;
                    if(jumpAudio != null)
                        jumpAudio.Play();
                    currentMovement.y = initialJumpVelocity;
                    inputBuffer.Dequeue();
                }
            }
        }
        else if (!isJumpPressed && isJumping && controller.isGrounded) 
        {
            isJumping = false;
        }
    }

    private void StopMovement()
    {
        currentMovement = new Vector3(0f, 0f, 0f);
        appliedYMovement = 0;
    }

    public void setRespawnPosition(Vector3 newPosition)
    {
        respawnPosition = newPosition;
    }

    private void Teleport(Vector3 newPosition)
    {
        controller.enabled = false;
        StopMovement();
        controller.transform.position = newPosition;
        controller.enabled = true;
    }

    public void KillPlayer()
    {
        Teleport(respawnPosition);
    }

    public void addInteractable(IInteractable interactable)
    {
        interactables.Add(interactable);
    }

    public void removeInteractable(IInteractable interactable)
    {
        interactables.Remove(interactable);
    }
}
