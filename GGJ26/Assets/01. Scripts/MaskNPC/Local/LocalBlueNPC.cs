using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(WanderPointProvider))]
public class LocalBlueNPC : LocalBaseNPC
{
    [Header("BlueNPC Settings")]
    [Tooltip("Distance threshold for picking a new destination")]
    public float stoppingDistance = 1.5f;

    [Header("Mask Behavior Settings")]
    [Tooltip("Walk state duration (min, max)")]
    public float[] WalkDuration = new float[] { 2f, 5f };
    [Tooltip("Run state duration (min, max)")]
    public float[] RunDuration = new float[] { 2f, 5f };

    private enum MaskState { Walking, Running }
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
        if (NpcController == null)
        {
            return;
        }

        if (Random.value < 0.5f)
        {
            SetState(MaskState.Running);
        }
        else
        {
            SetState(MaskState.Walking);
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
            SetState(currentMaskState == MaskState.Running ? MaskState.Walking : MaskState.Running);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            SetNewWanderDestination();
        }

        bool sprint = (currentMaskState == MaskState.Running);
        NpcController.SetCommandStopped(false);
        NpcController.SetCommandSprinting(sprint);
    }

    private void SetState(MaskState newState)
    {
        currentMaskState = newState;
        NpcController.SetCommandStopped(false);

        if (newState == MaskState.Running)
        {
            maskStateTimer = RandomRangePicker(RunDuration);
            NpcController.SetCommandSprinting(true);
        }
        else
        {
            maskStateTimer = RandomRangePicker(WalkDuration);
            NpcController.SetCommandSprinting(false);
        }

        SetNewWanderDestination();
    }

    private void SetNewWanderDestination()
    {
        if (wanderProvider.GetRandomNavMeshPoint(out Vector3 dest))
        {
            NpcController.SetCommandDestination(dest);
        }
    }
}
