using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float rotationSpeed = 720f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float timeToRun = 3f;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private RuntimeAnimatorController idleController;

    private static readonly Quaternion AxisCompensation = Quaternion.identity;

    private CharacterController controller;
    private WalkAnimation walkAnimation;
    private Animator animator;
    private float verticalVelocity;
    private float moveHoldTime;
    private bool jumpRequested;
    private bool wasMoving;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (modelTransform == null)
            modelTransform = transform.Find("Powerroo_born");
        if (modelTransform != null)
        {
            walkAnimation = modelTransform.GetComponent<WalkAnimation>();
            animator = modelTransform.GetComponent<Animator>();
        }
    }

    private void Start()
    {
        SoundManager.EnsureExists();
        SetIdleMode(true);
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        float h = 0f;
        float v = 0f;

        if (keyboard.dKey.isPressed) h += 1f;
        if (keyboard.aKey.isPressed) h -= 1f;
        if (keyboard.wKey.isPressed) v += 1f;
        if (keyboard.sKey.isPressed) v -= 1f;

        Vector3 inputDir = new Vector3(h, 0f, v).normalized;
        bool isMoving = inputDir.sqrMagnitude > 0.01f;

        // Track hold time for run transition
        if (isMoving)
        {
            moveHoldTime += Time.deltaTime;
        }
        else
        {
            moveHoldTime = 0f;
        }

        bool isRunning = moveHoldTime >= timeToRun;
        float currentSpeed = isRunning ? runSpeed : moveSpeed;

        bool spaceDown = keyboard.spaceKey.isPressed;
        if (spaceDown && !jumpRequested && controller.isGrounded)
        {
            jumpRequested = true;
            verticalVelocity = jumpForce;
            if (SoundManager.Instance != null)
                SoundManager.Instance.PlayJumpSound();
        }
        else if (controller.isGrounded && !spaceDown)
        {
            verticalVelocity = -2f;
            jumpRequested = false;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
            if (!spaceDown) jumpRequested = false;
        }

        Vector3 move = Vector3.zero;

        if (isMoving)
        {
            if (modelTransform != null)
            {
                // Add forward lean when running
                float leanAngle = isRunning ? 15f : 0f;
                Quaternion faceDir = Quaternion.LookRotation(inputDir, Vector3.up);
                Quaternion lean = Quaternion.Euler(leanAngle, 0f, 0f);
                Quaternion targetRot = faceDir * AxisCompensation * lean;
                modelTransform.rotation = Quaternion.RotateTowards(
                    modelTransform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            move = inputDir * currentSpeed;
        }

        // Switch between idle animation and walk animation
        if (isMoving && !wasMoving)
        {
            SetIdleMode(false);
        }
        else if (!isMoving && wasMoving)
        {
            SetIdleMode(true);
        }
        wasMoving = isMoving;

        if (walkAnimation != null)
        {
            walkAnimation.SetMoving(isMoving);
            walkAnimation.SetRunning(isRunning);
        }

        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }

    private void SetIdleMode(bool idle)
    {
        if (animator == null || idleController == null) return;

        if (idle)
        {
            if (walkAnimation != null)
                walkAnimation.enabled = false;
            animator.runtimeAnimatorController = idleController;
            animator.enabled = true;
        }
        else
        {
            animator.enabled = false;
            animator.runtimeAnimatorController = null;
            if (walkAnimation != null)
                walkAnimation.enabled = true;
        }
    }
}
