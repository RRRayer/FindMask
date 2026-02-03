using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LocalNPCController))]
public abstract class LocalBaseNPC : MonoBehaviour
{
    protected LocalNPCController NpcController { get; private set; }
    protected NavMeshAgent agent;
    protected WanderPointProvider wanderProvider;

    [Header("Event Channel - Listening to")]
    [SerializeField] private BoolEventChannelSO OnGroupDanceStart;
    [SerializeField] public IntEventChannelSO DanceIndexChannel;
    
    public enum ActionState
    {
        MaskBehavior,
        MaskDance,
        GroupDance
    }

    private ActionState currentState;

    protected virtual void Awake()
    {
        NpcController = GetComponent<LocalNPCController>();
        agent = GetComponent<NavMeshAgent>();
        wanderProvider = GetComponent<WanderPointProvider>();

        if (agent != null)
        {
            agent.updatePosition = false;
            agent.updateRotation = false;
        }
    }

    private void OnEnable()
    {
        if (OnGroupDanceStart != null)
        {
            OnGroupDanceStart.OnEventRaised += ExecuteGroupDance;
        }

        if (DanceIndexChannel != null)
        {
            DanceIndexChannel.OnEventRaised += ExecuteMaskDance;
        }
    }

    private void OnDisable()
    {
        if (OnGroupDanceStart != null)
        {
            OnGroupDanceStart.OnEventRaised -= ExecuteGroupDance;
        }

        if (DanceIndexChannel != null)
        {
            DanceIndexChannel.OnEventRaised -= ExecuteMaskDance;
        }
    }

    private void OnDestroy()
    {
        if (OnGroupDanceStart != null)
        {
            OnGroupDanceStart.OnEventRaised -= ExecuteGroupDance;
        }

        if (DanceIndexChannel != null)
        {
            DanceIndexChannel.OnEventRaised -= ExecuteMaskDance;
        }
    }

    private void Update()
    {
        if (currentState == ActionState.MaskBehavior)
        {
            ExecuteMaskBehavior();
        }
    }

    protected float GetDeltaTime()
    {
        return Time.deltaTime;
    }

    protected abstract void ExecuteMaskBehavior();

    protected void ExecuteMaskDance(int danceIndex)
    {
        if (NpcController == null)
        {
            return;
        }

        if (currentState == ActionState.GroupDance)
        {
            return;
        }

        if (danceIndex == -1)
        {
            currentState = ActionState.MaskBehavior;
            NpcController.StopDance();
        }
        else
        {
            currentState = ActionState.MaskDance;
            NpcController.StartDance(danceIndex);
        }
    }

    protected void ExecuteGroupDance(bool isStart)
    {
        if (NpcController == null)
        {
            return;
        }

        int danceIndex = Random.Range(0, 5);
        if (isStart)
        {
            currentState = ActionState.GroupDance;
            NpcController.StartDance(danceIndex);
        }
        else
        {
            currentState = ActionState.MaskBehavior;
            NpcController.StopDance();
        }
    }

    public float RandomRangePicker(float[] range)
    {
        if (range.Length != 2)
        {
            return 0f;
        }
        return Random.Range(range[0], range[1]);
    }

    public int RandomRangePicker(int[] range)
    {
        if (range.Length != 2)
        {
            return 0;
        }
        return Random.Range(range[0], range[1] + 1);
    }
}
