using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class LocalNPCController : MonoBehaviour
{
    [Header("NPC Movement")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [Range(0.0f, 0.3f)]
    public float RotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    public AudioClip LandingAudioClip;
    public AudioClip[] FootstepAudioClips;
    [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

    [Header("SFX Channel (optional)")]
    [SerializeField] private AudioCueEventChannelSO sfxEventChannel;
    [SerializeField] private AudioConfigurationSO sfxConfiguration;
    [SerializeField] private AudioCueSO landingSfxCue;
    [SerializeField] private AudioCueSO[] footstepSfxCues;

    [Space(10)]
    [Tooltip("The height the NPC can jump")]
    public float JumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("NPC Grounded Check")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    // movement
    private float speed;
    private float animationBlend;
    private float targetRotation;
    private float rotationVelocity;
    private float verticalVelocity;
    private float terminalVelocity = 53.0f;
    private Vector3 moveDirection = Vector3.zero;
    private bool isSprinting;
    private bool shouldJump;

    // timeout
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    // animation IDs
    private int animIDSpeed;
    private int animIDGrounded;
    private int animIDJump;
    private int animIDFreeFall;
    private int animIDMotionSpeed;
    private int animIDStartDance;
    private int animIDStopDance;
    private int animIDDanceIndex;
    private int animIDDead;

    // components
    private Animator animator;
    private CharacterController controller;
    private NavMeshAgent agent;

    private bool hasDestination;
    private Vector3 destination;
    private bool isStopped;
    private bool isDancing;
    private int lastJumpCounter;
    private int jumpCounter;
    private bool snappedToGroundOnDeath;

    public bool IsDead { get; private set; }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        AssignAnimationIDs();

        jumpTimeoutDelta = JumpTimeout;
        fallTimeoutDelta = FallTimeout;

        ConfigureNavMeshAgent();
    }

    private void Update()
    {
        if (IsDead)
        {
            if (agent != null)
            {
                agent.isStopped = true;
            }
            isStopped = true;
            hasDestination = false;
            moveDirection = Vector3.zero;
            verticalVelocity = 0f;
            isDancing = false;
            return;
        }

        ApplyLocalCommands();

        float deltaTime = Time.deltaTime;
        JumpAndGravity(deltaTime);
        GroundedCheck();
        Move(deltaTime);
    }

    private void LateUpdate()
    {
        if (IsDead == false || snappedToGroundOnDeath)
        {
            return;
        }

        Vector3 origin = transform.position + Vector3.up * 1f;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 20f, GroundLayers, QueryTriggerInteraction.Ignore))
        {
            transform.position = hit.point;
            snappedToGroundOnDeath = true;
        }
    }

    #region Public Methods for AI Control

    public void SetMovement(Vector3 direction, bool sprinting = false)
    {
        moveDirection = direction;
        isSprinting = sprinting;
    }

    public bool TryQueueJump()
    {
        if (Grounded && jumpTimeoutDelta <= 0.0f)
        {
            jumpCounter++;
            return true;
        }

        return false;
    }

    public void SetCommandDestination(Vector3 newDestination)
    {
        if (IsDead || isDancing)
        {
            hasDestination = false;
            return;
        }

        if (agent != null)
        {
            if (agent.isOnNavMesh == false)
            {
                hasDestination = false;
                return;
            }

            NavMeshHit hit;
            if (NavMesh.SamplePosition(newDestination, out hit, 2.0f, NavMesh.AllAreas))
            {
                newDestination = hit.position;
            }
            else
            {
                hasDestination = false;
                return;
            }
        }

        destination = newDestination;
        hasDestination = true;
        if (agent != null)
        {
            agent.SetDestination(newDestination);
        }
    }

    public void SetCommandStopped(bool stopped)
    {
        isStopped = stopped;
        if (agent != null && agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = stopped;
        }
    }

    public void SetCommandSprinting(bool sprinting)
    {
        isSprinting = sprinting;
    }

    public bool TriggerJump()
    {
        if (Grounded && jumpTimeoutDelta <= 0.0f)
        {
            shouldJump = true;
            return true;
        }
        return false;
    }

    public void StartDance(int danceIndex)
    {
        if (IsDead)
        {
            return;
        }

        isDancing = true;
        isStopped = true;
        hasDestination = false;
        SetMovement(Vector3.zero, false);

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (animator == null)
        {
            return;
        }

        animator.ResetTrigger(animIDStopDance);
        animator.SetInteger(animIDDanceIndex, danceIndex);
        animator.SetTrigger(animIDStartDance);
    }

    public void StopDance()
    {
        if (IsDead)
        {
            return;
        }

        isDancing = false;
        isStopped = false;
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (animator == null)
        {
            return;
        }

        animator.ResetTrigger(animIDStartDance);
        animator.SetTrigger(animIDStopDance);
    }

    public void TriggerDead()
    {
        IsDead = true;
        isDancing = false;
        snappedToGroundOnDeath = false;
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (animator == null)
        {
            return;
        }

        animator.SetTrigger(animIDDead);
    }

    #endregion

    private void ApplyLocalCommands()
    {
        if (IsDead || isDancing)
        {
            if (agent != null)
            {
                agent.isStopped = true;
            }
            moveDirection = Vector3.zero;
            return;
        }

        if (jumpCounter != lastJumpCounter)
        {
            lastJumpCounter = jumpCounter;
            TriggerJump();
        }

        if (agent != null && agent.enabled)
        {
            agent.isStopped = isStopped;
            if (hasDestination)
            {
                agent.SetDestination(destination);
            }
            agent.nextPosition = transform.position;
        }

        if (agent != null && agent.enabled && agent.hasPath && agent.isStopped == false)
        {
            Vector3 desired = agent.desiredVelocity;
            if (desired.sqrMagnitude < 0.0001f)
            {
                Vector3 toSteer = agent.steeringTarget - transform.position;
                if (toSteer.sqrMagnitude > 0.0001f)
                {
                    desired = toSteer;
                }
            }
            SetMovement(desired.normalized, isSprinting);
        }
        else
        {
            SetMovement(Vector3.zero, false);
        }
    }

    private void AssignAnimationIDs()
    {
        animIDSpeed = Animator.StringToHash("Speed");
        animIDGrounded = Animator.StringToHash("Grounded");
        animIDJump = Animator.StringToHash("Jump");
        animIDFreeFall = Animator.StringToHash("FreeFall");
        animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        animIDStartDance = Animator.StringToHash("StartDance");
        animIDStopDance = Animator.StringToHash("StopDance");
        animIDDanceIndex = Animator.StringToHash("DanceIndex");
        animIDDead = Animator.StringToHash("Dead");
    }

    private void ConfigureNavMeshAgent()
    {
        if (agent == null)
        {
            return;
        }

        agent.updatePosition = false;
        agent.updateRotation = false;

        if (agent.isOnNavMesh == false)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
        else
        {
            agent.Warp(transform.position);
        }
    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

        animator.SetBool(animIDGrounded, Grounded);
    }

    private void Move(float deltaTime)
    {
        float targetSpeed = isSprinting ? SprintSpeed : MoveSpeed;
        if (moveDirection == Vector3.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = speed;
        float desiredSpeed = 0f;
        if (agent != null && agent.enabled)
        {
            desiredSpeed = agent.desiredVelocity.magnitude;
            currentHorizontalSpeed = desiredSpeed;
        }
        float speedOffset = 0.1f;
        float inputMagnitude = moveDirection.magnitude;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, deltaTime * SpeedChangeRate);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else
        {
            speed = targetSpeed;
        }

        float animTargetSpeed = targetSpeed;
        if (agent != null && agent.enabled)
        {
            animTargetSpeed = desiredSpeed;
            inputMagnitude = Mathf.Clamp01(desiredSpeed / Mathf.Max(SprintSpeed, 0.01f));
        }
        animationBlend = Mathf.Lerp(animationBlend, animTargetSpeed, deltaTime * SpeedChangeRate);
        if (animationBlend < 0.01f) animationBlend = 0f;

        Vector3 inputDirection = moveDirection.normalized;

        if (moveDirection != Vector3.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        controller.Move(targetDirection.normalized * (speed * deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * deltaTime);

        animator.SetFloat(animIDSpeed, animationBlend);
        animator.SetFloat(animIDMotionSpeed, inputMagnitude);
    }

    private void JumpAndGravity(float deltaTime)
    {
        if (Grounded)
        {
            fallTimeoutDelta = FallTimeout;

            animator.SetBool(animIDJump, false);
            animator.SetBool(animIDFreeFall, false);

            if (verticalVelocity < 0.0f)
            {
                verticalVelocity = -2f;
            }

            if (shouldJump && jumpTimeoutDelta <= 0.0f)
            {
                verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                animator.SetBool(animIDJump, true);
            }

            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= deltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = JumpTimeout;

            if (fallTimeoutDelta >= 0.0f)
            {
                fallTimeoutDelta -= deltaTime;
            }
            else
            {
                animator.SetBool(animIDFreeFall, true);
            }
        }

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += Gravity * deltaTime;
        }

        shouldJump = false;
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (TryPlayFootstepSfx())
            {
                return;
            }

            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(controller.center), FootstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (TryPlayLandingSfx())
            {
                return;
            }

            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(controller.center), FootstepAudioVolume);
        }
    }

    private bool TryPlayFootstepSfx()
    {
        if (sfxEventChannel == null || sfxConfiguration == null || footstepSfxCues == null || footstepSfxCues.Length == 0)
        {
            return false;
        }

        var index = Random.Range(0, footstepSfxCues.Length);
        var cue = footstepSfxCues[index];
        if (cue == null)
        {
            return false;
        }

        sfxEventChannel.RaisePlayEvent(cue, sfxConfiguration, transform.TransformPoint(controller.center));
        return true;
    }

    private bool TryPlayLandingSfx()
    {
        if (sfxEventChannel == null || sfxConfiguration == null || landingSfxCue == null)
        {
            return false;
        }

        sfxEventChannel.RaisePlayEvent(landingSfxCue, sfxConfiguration, transform.TransformPoint(controller.center));
        return true;
    }
}
