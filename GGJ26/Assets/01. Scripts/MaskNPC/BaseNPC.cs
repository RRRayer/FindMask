using Fusion;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NPCController))]
public abstract class BaseNPC : MonoBehaviour
{
    protected NPCController NpcController { get; private set; }
    protected NavMeshAgent agent;
    protected WanderPointProvider wanderProvider;
    protected bool HasStateAuthority
    {
        get
        {
            return NpcController != null && NpcController.Object != null && NpcController.Object.HasStateAuthority;
        }
    }

    [Header("Listening to")]
    [SerializeField] private BoolEventChannelSO OnGroupDanceStart;
    [SerializeField] public IntEventChannelSO DanceIndexChannel;
    
    public enum ActionState
    {
        // 가�??�동
        MaskBehavior,

        // 가�??�스
        MaskDance,

        // ?�체 ?�스
        GroupDance
    }

    private ActionState currentState;
    
    protected virtual void Awake()
    {
        NpcController = GetComponent<NPCController>();
        agent = GetComponent<NavMeshAgent>();
        wanderProvider = GetComponent<WanderPointProvider>();
        
        agent.updatePosition = false;
        agent.updateRotation = false;
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
        if (HasStateAuthority == false)
        {
            return;
        }

        if (currentState == ActionState.MaskBehavior)
        {
            ExecuteMaskBehavior();
        }
    }

    protected float GetDeltaTime()
    {
        if (NpcController != null)
        {
            return NpcController.GetDeltaTime();
        }

        return Time.deltaTime;
    }

    protected abstract void ExecuteMaskBehavior();
    
    /// <param name="isStart"></param>
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

        int danceIndex = Random.Range(0, 5); // 0~4 (Group Dance includes Crazy)
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
        // For integers, Random.Range's upper bound is exclusive, so add 1 to make it inclusive.
        return Random.Range(range[0], range[1] + 1);
    }
}
