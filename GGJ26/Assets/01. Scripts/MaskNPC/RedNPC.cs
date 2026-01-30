using UnityEngine;
using UnityEngine.AI;

namespace MaskNPC
{
    /// <summary>
    /// BaseNPC를 상속받아, 맵의 랜덤한 지점을 계속 걸어다니는 행동을 정의합니다.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(WanderPointProvider))]
    public class RedNPC : BaseNPC
    {
        [Header("RedNPC 설정")]
        [Tooltip("목적지에 얼마나 가까워지면 다음 목적지를 찾을지 결정")]
        public float stoppingDistance = 1.5f;

        private NavMeshAgent agent;
        private WanderPointProvider wanderProvider;

        /// <summary>
        /// 부모의 Awake()를 호출하여 기본 컴포넌트를 초기화하고,
        /// RedNPC에 필요한 추가 컴포넌트들을 초기화합니다.
        /// </summary>
        protected override void Awake()
        {
            base.Awake(); // 부모 클래스의 Awake() 실행 (NpcController 초기화)
            
            agent = GetComponent<NavMeshAgent>();
            wanderProvider = GetComponent<WanderPointProvider>();

            // NavMeshAgent가 캐릭터의 위치나 회전을 직접 제어하지 않도록 설정합니다.
            // 모든 실제 움직임은 NPCController가 담당합니다.
            agent.updatePosition = false;
            agent.updateRotation = false;
            agent.stoppingDistance = stoppingDistance;
        }

        private void Start()
        {
            // AI 행동 시작을 위해 첫 목적지를 설정합니다.
            SetNewWanderDestination();
        }

        /// <summary>
        /// BaseNPC로부터 물려받은 행동 로직 메서드입니다.
        /// 매 프레임 실행되며 RedNPC의 행동을 정의합니다.
        /// </summary>
        protected override void ExecuteBehavior()
        {
            if (NpcController == null || agent == null) return;

            // NavMeshAgent의 내부 위치를 실제 캐릭터의 현재 위치로 계속 업데이트해줍니다.
            agent.nextPosition = transform.position;

            // 경로 계산이 진행 중이 아닐 때, 그리고 목적지 도착 시 새로운 목적지를 설정합니다.
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                SetNewWanderDestination();
            }

            // NavMeshAgent가 계산한 다음 경로(desiredVelocity)로 걷도록 NpcController에 명령합니다.
            // isSprinting을 false로 하여 "걷도록" 합니다.
            NpcController.SetMovement(agent.desiredVelocity.normalized, false);
        }
        
        /// <summary>
        /// WanderPointProvider를 사용해 새로운 목적지를 찾고, NavMeshAgent에 설정합니다.
        /// </summary>
        private void SetNewWanderDestination()
        {
            if (wanderProvider.GetRandomNavMeshPoint(out Vector3 destination))
            {
                agent.SetDestination(destination);
            }
        }
    }
}
