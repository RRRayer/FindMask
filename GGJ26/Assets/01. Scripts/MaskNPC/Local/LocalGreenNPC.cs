using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(WanderPointProvider))]
public class LocalGreenNPC : LocalBaseNPC
{
    [Header("Mask Behavior Settings")]
    [Tooltip("Walk state duration (min, max)")]
    public float[] WalkDuration = { 3f, 6f };
    [Tooltip("Jump count (min, max)")]
    public int[] JumpCount = { 1, 4 };

    private enum MaskState { Walking, Jumping }
    private MaskState currentMaskState;

    private float walkTimer;
    private int jumpsRemaining;

    protected override void ExecuteMaskBehavior()
    {
        if (NpcController == null || agent == null) return;

        if (agent.isOnNavMesh == false)
        {
            return;
        }

        agent.nextPosition = transform.position;

        if (currentMaskState == MaskState.Walking)
        {
            walkTimer -= GetDeltaTime();
            if (walkTimer <= 0)
            {
                SetState(MaskState.Jumping);
                return;
            }
            
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (wanderProvider.GetRandomNavMeshPoint(out Vector3 dest))
                {
                    NpcController.SetCommandDestination(dest);
                }
            }
            NpcController.SetMovement(agent.desiredVelocity.normalized, false);
        }
        else
        {
            NpcController.SetMovement(Vector3.zero, false);
            
            if (jumpsRemaining > 0)
            {
                if (NpcController.TriggerJump())
                {
                    jumpsRemaining--;
                }
            }
            else
            {
                SetState(MaskState.Walking);
            }
        }
    }

    private void SetState(MaskState newState)
    {
        currentMaskState = newState;
        if (newState == MaskState.Walking)
        {
            walkTimer = RandomRangePicker(WalkDuration);
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                if (wanderProvider.GetRandomNavMeshPoint(out Vector3 dest))
                {
                    NpcController.SetCommandDestination(dest);
                }
            }
        }
        else
        {
            jumpsRemaining = RandomRangePicker(JumpCount);
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
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
            SetState(MaskState.Walking);
        }
        else
        {
            SetState(MaskState.Jumping);
        }
    }
}
