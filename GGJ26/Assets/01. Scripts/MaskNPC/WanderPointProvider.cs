using UnityEngine;
using UnityEngine.AI;

namespace MaskNPC
{
    /// <summary>
    /// NavMesh 위에서 NPC가 배회할 수 있는 랜덤한 목적지를 찾아 제공하는 역할을 합니다.
    /// </summary>
    public class WanderPointProvider : MonoBehaviour
    {
        [Tooltip("NPC가 배회할 활동 반경")]
        public float wanderRadius = 20f;

        /// <summary>
        /// 현재 위치를 기준으로 wanderRadius 반경 내에서 유효한 NavMesh 위의 랜덤 지점을 찾습니다.
        /// </summary>
        /// <param name="result">찾은 위치를 저장할 Vector3 변수입니다.</param>
        /// <returns>유효한 위치를 찾았으면 true, 아니면 false를 반환합니다.</returns>
        public bool GetRandomNavMeshPoint(out Vector3 result)
        {
            // 1. 현재 게임오브젝트 위치를 중심으로 랜덤한 방향과 거리를 정합니다.
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * wanderRadius;
            
            NavMeshHit hit;

            // 2. NavMesh.SamplePosition을 사용해 randomPoint에서 가장 가까운, 실제로 걸어갈 수 있는 지점을 찾습니다.
            // (wanderRadius를 검색 반경으로 사용하여 너무 먼 곳은 탐색하지 않도록 합니다.)
            if (NavMesh.SamplePosition(randomPoint, out hit, wanderRadius, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }

            // 3. 유효한 위치를 찾지 못했다면, 기본값(Vector3.zero)을 설정하고 false를 반환합니다.
            result = Vector3.zero;
            return false;
        }
        
        // 디버깅을 위해 에디터에서 배회 반경을 시각적으로 표시합니다.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, wanderRadius);
        }
    }
}
