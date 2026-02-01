using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// BaseNPC瑜??곸냽諛쏆븘, '?щ━湲?? '?湲?瑜?諛섎났?섎뒗 媛硫??됰룞???뺤쓽?⑸땲??
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(WanderPointProvider))]
public class RedNPC : BaseNPC
{
    [Header("RedNPC ?ㅼ젙")]
    [Tooltip("紐⑹쟻吏???쇰쭏??媛源뚯썙吏硫??ㅼ쓬 紐⑹쟻吏瑜?李얠쓣吏 寃곗젙")]
    public float stoppingDistance = 1.5f;

    [Header("媛硫??됰룞 ?ㅼ젙")]
    [Tooltip("?щ━湲??곹깭瑜??좎????쒓컙 (理쒖냼, 理쒕?)")]
    public float[] RunDuration = new float[] { 2f, 5f };
    [Tooltip("?湲??곹깭瑜??좎????쒓컙 (理쒖냼, 理쒕?)")]
    public float[] IdleDuration = new float[] { 2f, 5f };

    private enum MaskState { Running, Idling }
    private MaskState currentMaskState;
    private float maskStateTimer;
    
    protected override void Awake()
    {
        base.Awake();
        agent.stoppingDistance = stoppingDistance;
    }

    private void Start()
    {
        if (HasStateAuthority == false)
        {
            return;
        }

        // 珥덇린 ?곹깭瑜??쒕뜡?쇰줈 ?ㅼ젙?⑸땲??
        if (Random.value < 0.5f) // 50% ?뺣쪧濡?Running, 50% ?뺣쪧濡?Idling
        {
            currentMaskState = MaskState.Running;
            maskStateTimer = RandomRangePicker(RunDuration);
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                SetNewWanderDestination();
            }
            if (NpcController != null)
            {
                NpcController.SetCommandStopped(false);
                NpcController.SetCommandSprinting(true);
            }
        }
        else
        {
            currentMaskState = MaskState.Idling;
            maskStateTimer = RandomRangePicker(IdleDuration);
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            } // Ensure agent stops if starting with idling
            if (NpcController != null)
            {
                NpcController.SetCommandStopped(true);
                NpcController.SetCommandSprinting(false);
            }
        }
    }

    /// <summary>
    /// 留??꾨젅???ㅽ뻾?섎ŉ '?щ━湲?? '?湲? ?곹깭???곕씪 ?됰룞??寃곗젙?⑸땲??
    /// </summary>
    protected override void ExecuteMaskBehavior()
    {
        if (NpcController == null || agent == null) return;

        // NavMeshAgent???꾩튂瑜?罹먮┃?곗쓽 ?ㅼ젣 ?꾩튂濡?怨꾩냽 ?낅뜲?댄듃?⑸땲??
        if (IsAgentReady == false)
        {
            return;
        }

        agent.nextPosition = transform.position;

        // ?꾩옱 ?곹깭????대㉧瑜?媛먯냼?쒗궎怨? ?쒓컙?????섎㈃ ?곹깭瑜?蹂寃쏀빀?덈떎.
        maskStateTimer -= GetDeltaTime();
        if (maskStateTimer <= 0)
        {
            SwitchMaskState();
        }

        // ?꾩옱 ?곹깭???곕Ⅸ ?됰룞???ㅽ뻾?⑸땲??
        if (currentMaskState == MaskState.Running)
        {
            // 紐⑹쟻吏???꾩갑?섎㈃ ?덈줈??紐⑹쟻吏瑜??ㅼ젙?⑸땲??
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                SetNewWanderDestination();
            }

            // NPCController??'?щ━湲?瑜?紐낅졊?⑸땲??
            if (NpcController != null)
            {
                NpcController.SetCommandStopped(false);
                NpcController.SetCommandSprinting(true);
            }
        }
        else // Idling ?곹깭??寃쎌슦
        {
            // NPCController??'?뺤?'瑜?紐낅졊?⑸땲??
            if (NpcController != null)
            {
                NpcController.SetCommandStopped(true);
                NpcController.SetCommandSprinting(false);
            }
        }
    }

    /// <summary>
    /// '?щ━湲?? '?湲? ?곹깭瑜??꾪솚?⑸땲??
    /// </summary>
    private void SwitchMaskState()
    {
        if (currentMaskState == MaskState.Running)
        {
            // '?湲? ?곹깭濡?蹂寃?
            currentMaskState = MaskState.Idling;
            maskStateTimer = RandomRangePicker(IdleDuration);
            if (IsAgentReady)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
            if (NpcController != null)
            {
                NpcController.SetCommandStopped(true);
                NpcController.SetCommandSprinting(false);
            }
        }
        else // Idling ?곹깭??ㅻ㈃
        {
            // '?щ━湲? ?곹깭濡?蹂寃?
            currentMaskState = MaskState.Running;
            maskStateTimer = RandomRangePicker(RunDuration);
            if (IsAgentReady)
            {
                agent.isStopped = false;
                SetNewWanderDestination();
            }
            if (NpcController != null)
            {
                NpcController.SetCommandStopped(false);
                NpcController.SetCommandSprinting(true);
            }
        }
    }
    
    /// <summary>
    /// WanderPointProvider瑜??ъ슜???덈줈??紐⑹쟻吏瑜?李얘퀬, NavMeshAgent???ㅼ젙?⑸땲??
    /// </summary>
    private void SetNewWanderDestination()
    {
        if (IsAgentReady == false)
        {
            return;
        }

        if (wanderProvider.GetRandomNavMeshPoint(out Vector3 destination))
        {
            if (NpcController != null)
            {
                NpcController.SetCommandDestination(destination);
            }
            else
            {
                agent.SetDestination(destination);
            }
        }
    }

}

