using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(WanderPointProvider))]
public class LocalRedNPC : LocalBaseNPC
{
    [Header("RedNPC Settings")]
    [Tooltip("Distance threshold for picking a new destination")]
    public float stoppingDistance = 1.5f;

    [Header("Mask Behavior Settings")]
    [Tooltip("Run state duration (min, max)")]
    public float[] RunDuration = new float[] { 2f, 5f };
    [Tooltip("Idle state duration (min, max)")]
    public float[] IdleDuration = new float[] { 2f, 5f };

    private enum MaskState { Running, Idling }
    private MaskState currentMaskState;
    private float maskStateTimer;
    
    protected override void Awake()
    {
        base.Awake();
        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
        }
    }

    private void Start()
    {
        if (NpcController == null || agent == null)
        {
            return;
        }

        if (Random.value < 0.5f)
        {
            currentMaskState = MaskState.Running;
            maskStateTimer = RandomRangePicker(RunDuration);
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                SetNewWanderDestination();
            }
            NpcController.SetCommandStopped(false);
            NpcController.SetCommandSprinting(true);
        }
        else
        {
            currentMaskState = MaskState.Idling;
            maskStateTimer = RandomRangePicker(IdleDuration);
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
            NpcController.SetCommandStopped(true);
            NpcController.SetCommandSprinting(false);
        }
    }

    protected override void ExecuteMaskBehavior()
    {
        if (NpcController == null || agent == null) return;

        if (agent.isOnNavMesh == false)
        {
            return;
        }

        agent.nextPosition = transform.position;

        maskStateTimer -= GetDeltaTime();
        if (maskStateTimer <= 0)
        {
            SwitchMaskState();
        }

        if (currentMaskState == MaskState.Running)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                SetNewWanderDestination();
            }

            NpcController.SetCommandStopped(false);
            NpcController.SetCommandSprinting(true);
        }
        else
        {
            NpcController.SetCommandStopped(true);
            NpcController.SetCommandSprinting(false);
        }
    }

    private void SwitchMaskState()
    {
        if (currentMaskState == MaskState.Running)
        {
            currentMaskState = MaskState.Idling;
            maskStateTimer = RandomRangePicker(IdleDuration);
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
            NpcController.SetCommandStopped(true);
            NpcController.SetCommandSprinting(false);
        }
        else
        {
            currentMaskState = MaskState.Running;
            maskStateTimer = RandomRangePicker(RunDuration);
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                SetNewWanderDestination();
            }
            NpcController.SetCommandStopped(false);
            NpcController.SetCommandSprinting(true);
        }
    }
    
    private void SetNewWanderDestination()
    {
        if (agent.isOnNavMesh == false)
        {
            return;
        }

        if (wanderProvider.GetRandomNavMeshPoint(out Vector3 dest))
        {
            NpcController.SetCommandDestination(dest);
        }
    }
}
