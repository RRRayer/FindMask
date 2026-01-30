using Fusion;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FusionThirdPersonMotor : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float rotationSmoothTime = 0.12f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private bool debugInput = false;
    [SerializeField] private bool updateAnimator = true;

    [Networked] private float VerticalVelocity { get; set; }

    private CharacterController controller;
    private Animator animator;
    private bool hasAnimator;
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDMotionSpeed;
    private float rotationVelocity;
    private float fallTimeout = 0.15f;
    private float jumpTimeout = 0.3f;
    private float fallTimeoutDelta;
    private float jumpTimeoutDelta;
    private float lastJumpPressedTime = -10f;
    private float lastGroundedTime = -10f;
    private Camera mainCamera;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            var cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (cameraObject != null)
            {
                mainCamera = cameraObject.GetComponent<Camera>();
            }
        }
        if (updateAnimator)
        {
            hasAnimator = TryGetComponent(out animator);
            animIDSpeed = Animator.StringToHash("Speed");
            animIDGrounded = Animator.StringToHash("Grounded");
            animIDJump = Animator.StringToHash("Jump");
            animIDFreeFall = Animator.StringToHash("FreeFall");
            animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }
        fallTimeoutDelta = fallTimeout;
        jumpTimeoutDelta = 0f;
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority == false)
        {
            return;
        }

        if (controller == null)
        {
            return;
        }

        if (GetInput(out PlayerInputData input) == false)
        {
            if (debugInput)
            {
                Debug.Log("[FusionThirdPersonMotor] No input for tick", this);
            }
            ApplyGravity(Vector3.zero);
            return;
        }

        Vector3 move = new Vector3(input.Move.x, 0f, input.Move.y);
        if (debugInput)
        {
            Debug.Log($"[FusionThirdPersonMotor] move={move} jump={input.Jump} sprint={input.Sprint}", this);
        }
        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        float speed = input.Sprint ? sprintSpeed : moveSpeed;
        float inputMagnitude = move == Vector3.zero ? 0f : 1f;
        float cameraYaw = mainCamera != null ? mainCamera.transform.eulerAngles.y : transform.eulerAngles.y;
        float targetRotation = transform.eulerAngles.y;
        if (move != Vector3.zero)
        {
            targetRotation = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cameraYaw;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime, Mathf.Infinity, Runner.DeltaTime);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward;
        Vector3 horizontal = targetDirection * speed * inputMagnitude;

        if (input.Jump)
        {
            lastJumpPressedTime = Runner.SimulationTime;
        }

        if (controller.isGrounded)
        {
            lastGroundedTime = Runner.SimulationTime;
        }

        bool wantsJump = (Runner.SimulationTime - lastJumpPressedTime) <= jumpBufferTime;
        bool canCoyote = (Runner.SimulationTime - lastGroundedTime) <= coyoteTime;
        bool doJump = wantsJump && canCoyote;

        ApplyGravity(horizontal, doJump);
        UpdateAnimatorState(horizontal, inputMagnitude, input.Jump);
    }

    private void ApplyGravity(Vector3 horizontal, bool jump = false)
    {
        if (controller.isGrounded)
        {
            if (jump)
            {
                VerticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else if (VerticalVelocity < 0f)
            {
                VerticalVelocity = -2f;
            }
        }

        VerticalVelocity += gravity * Runner.DeltaTime;

        Vector3 motion = new Vector3(horizontal.x, VerticalVelocity, horizontal.z) * Runner.DeltaTime;
        controller.Move(motion);
    }

    private void UpdateAnimatorState(Vector3 horizontal, float inputMagnitude, bool jumpPressed)
    {
        if (hasAnimator == false || updateAnimator == false)
        {
            return;
        }

        bool grounded = controller.isGrounded;
        animator.SetBool(animIDGrounded, grounded);

        if (grounded)
        {
            fallTimeoutDelta = fallTimeout;
            animator.SetBool(animIDFreeFall, false);

            if (jumpPressed && jumpTimeoutDelta <= 0f)
            {
                animator.SetBool(animIDJump, true);
            }
            else
            {
                animator.SetBool(animIDJump, false);
            }

            if (jumpTimeoutDelta >= 0f)
            {
                jumpTimeoutDelta -= Runner.DeltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = jumpTimeout;
            if (fallTimeoutDelta >= 0f)
            {
                fallTimeoutDelta -= Runner.DeltaTime;
            }
            else
            {
                animator.SetBool(animIDFreeFall, true);
            }
        }

        float speed = new Vector3(horizontal.x, 0f, horizontal.z).magnitude;
        animator.SetFloat(animIDSpeed, speed);
        animator.SetFloat(animIDMotionSpeed, inputMagnitude);
    }
}
