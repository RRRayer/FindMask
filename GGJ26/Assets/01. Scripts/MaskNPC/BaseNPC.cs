using Fusion;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 모든 NPC ?�크립트가 ?�속받을 부�?추상 ?�래?�입?�다.
/// NPC??공통 기능�??�동 로직???�한 뼈�?�??�공?�니??
/// </summary>
[RequireComponent(typeof(NPCController))]
public abstract class BaseNPC : MonoBehaviour
{
    // ?�식 ?�래?�들???�근?????�도�?protected�?NPCController 참조�??�공?�니??
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
    protected bool IsAgentReady
    {
        get
        {
            return agent != null && agent.enabled && agent.isOnNavMesh;
        }
    }

    [Header("?�벤??채널 - Listening to")]
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

    private ActionState currentState;/// <summary>
    /// Awake??컴포?�트 참조�?초기?�하?????�용?�니??
    /// ?�식 ?�래?�에??Awake�??�정??override)??경우, base.Awake()�??�출?�야 ?�니??
    /// </summary>
    protected virtual void Awake()
    {
        // ???�크립트가 붙�? 게임 ?�브?�트?�서 NPCController 컴포?�트�?찾습?�다.
        NpcController = GetComponent<NPCController>();
        agent = GetComponent<NavMeshAgent>();
        wanderProvider = GetComponent<WanderPointProvider>();

        // NavMeshAgent가 캐릭?�의 ?�치???�전??직접 ?�어?��? ?�도�??�정?�니??
        // 모든 ?�제 ?�직임?� NPCController가 ?�당?�니??
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

    /// <summary>
    /// Unity??Update 루프?�니??
    /// �??�레?�마???�식 ?�래?��? 구체?�으�??�의???�동 로직???�행?�니??
    /// </summary>
        private void Update()
    {
        if (HasStateAuthority == false)
        {
            return;
        }

        if (NpcController != null && NpcController.Runner != null && NpcController.Runner.IsRunning)
        {
            return;
        }

        if (currentState == ActionState.MaskBehavior)
        {
            ExecuteMaskBehavior();
        }
    }

    public void NetworkTick()
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

    /// <summary>
    /// �?그룹??가�??�동
    /// </summary>
    protected abstract void ExecuteMaskBehavior();


    /// <summary>
    /// �?그룹�?가�??�스. 그룹 별로 같�? �?
    /// </summary>
    /// <param name="isStart"></param>
    protected void ExecuteMaskDance(int danceIndex)
    {
        if (NpcController == null)
        {
            return;
        }

        if (currentState == ActionState.GroupDance)
        {
            // ?�체 ?�스 중일 ?�는 가�??�스�??�환?��? ?�음
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

    /// <summary>
    /// ?�체 ?�스. 모두가 ?�께 �? ?��? ?�덤
    /// </summary>
    /// <param name="isStart"></param>
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
