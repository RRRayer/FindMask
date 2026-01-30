using UnityEngine;

namespace MaskNPC
{
    /// <summary>
    /// 모든 NPC 스크립트가 상속받을 부모 추상 클래스입니다.
    /// NPC의 공통 기능과 행동 로직을 위한 뼈대를 제공합니다.
    /// </summary>
    [RequireComponent(typeof(NPCController))]
    public abstract class BaseNPC : MonoBehaviour
    {
        // 자식 클래스들이 접근할 수 있도록 protected로 NPCController 참조를 제공합니다.
        protected NPCController NpcController { get; private set; }

        /// <summary>
        /// Awake는 컴포넌트 참조를 초기화하는 데 사용됩니다.
        /// 자식 클래스에서 Awake를 재정의(override)할 경우, base.Awake()를 호출해야 합니다.
        /// </summary>
        protected virtual void Awake()
        {
            // 이 스크립트가 붙은 게임 오브젝트에서 NPCController 컴포넌트를 찾습니다.
            NpcController = GetComponent<NPCController>();
        }

        /// <summary>
        /// Unity의 Update 루프입니다.
        /// 매 프레임마다 자식 클래스가 구체적으로 정의한 행동 로직을 실행합니다.
        /// </summary>
        private void Update()
        {
            ExecuteBehavior();
        }

        /// <summary>
        /// 자식 NPC 클래스가 반드시 구현해야 하는 추상 메서드입니다.
        /// 각 NPC의 고유한 행동 로직이 이 메서드 안에 작성됩니다.
        /// </summary>
        protected abstract void ExecuteBehavior();
    }
}
