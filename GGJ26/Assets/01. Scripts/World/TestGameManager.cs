using System.Collections;
using UnityEngine;
using System.Threading.Tasks; // Required for Task and Task.Delay

public class TestGameManager : MonoBehaviour
{
    [SerializeField]
    private VoidEventChannelSO startDiscoEvent;
    [SerializeField]
    private VoidEventChannelSO stopDiscoEvent; // Now acts as "request stop disco"
    [SerializeField]
    private VoidEventChannelSO confirmStopDiscoEvent; // To confirm stop after effect
    [SerializeField]
    private float discoDuration = 10f; // 디스코볼 효과가 지속될 시간 (초)

    [Header("Mask Effect Settings")]
    [SerializeField] private MaskEffect maskEffectPrefab;
    [SerializeField] private Transform effectSpawnPoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 디스코 시퀀스 테스트 시작
        StartCoroutine(TestDiscoSequence());
    }

    // 디스코 시작/중지 이벤트를 발행하는 코루틴
    private IEnumerator TestDiscoSequence() // Removed 'async'
    {
        // --- NEW: Play MaskEffect at Disco Start ---
        if (maskEffectPrefab != null && effectSpawnPoint != null)
        {
            MaskEffect effectInstance = Instantiate(maskEffectPrefab, effectSpawnPoint.position, effectSpawnPoint.rotation);
            Debug.Log("[TestGameManager] Playing MaskEffect at disco start.", this);
            yield return effectInstance.PlayEffectSequence().AsCoroutine(); // Use AsCoroutine
        }
        else
        {
            Debug.LogWarning("MaskEffectPrefab 또는 EffectSpawnPoint가 할당되지 않았습니다. TestGameManager.");
        }
        // --- END NEW ---

        // 시작 이벤트 발행
        if (startDiscoEvent != null)
        {
            Debug.Log("[TestGameManager] startDiscoEvent RaiseEvent.", this);
            startDiscoEvent.RaiseEvent();
        }
        else
        {
            Debug.LogWarning("startDiscoEvent가 할당되지 않았습니다. TestGameManager.");
        }

        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(discoDuration);

        // 중지 이벤트 발행 (이제 이것은 "종료 요청" 신호입니다)
        if (stopDiscoEvent != null)
        {
            Debug.Log("[TestGameManager] stopDiscoEvent (Request Stop) RaiseEvent.", this);
            stopDiscoEvent.RaiseEvent();
        }
        else
        {
            Debug.LogWarning("stopDiscoEvent가 할당되지 않았습니다. TestGameManager (종료 요청).");
        }

        // --- NEW LOGIC: Play MaskEffect at Disco Stop and then confirm ---
        if (maskEffectPrefab != null && effectSpawnPoint != null)
        {
            MaskEffect effectInstance = Instantiate(maskEffectPrefab, effectSpawnPoint.position, effectSpawnPoint.rotation);
            Debug.Log("[TestGameManager] Playing MaskEffect at disco stop.", this);
            yield return effectInstance.PlayEffectSequence().AsCoroutine(); // Use AsCoroutine
        }
        else
        {
            Debug.LogWarning("MaskEffectPrefab 또는 EffectSpawnPoint가 할당되지 않았습니다. TestGameManager.");
        }

        // NOW, the TestGameManager confirms the stop directly, as it just played its effect.
        if (confirmStopDiscoEvent != null)
        {
            Debug.Log("[TestGameManager] confirmStopDiscoEvent RaiseEvent (after TestGameManager's effect).", this);
            confirmStopDiscoEvent.RaiseEvent();
        }
        else
        {
            Debug.LogWarning("confirmStopDiscoEvent가 할당되지 않았습니다. TestGameManager (종료 확정).");
        }
        // --- END NEW LOGIC ---

        Debug.Log("[TestGameManager] Disco test sequence finished.", this);
    }
}

// Task를 IEnumerator로 변환해주는 확장 메서드 (TestGameManager.cs 파일 내부에 추가)
public static class TaskExtensions
{
    public static IEnumerator AsCoroutine(this Task task)
    {
        while (!task.IsCompleted)
        {
            yield return null;
        }
        if (task.IsFaulted)
        {
            throw task.Exception;
        }
    }

    public static void Forget(this Task task)
    {
        // This method is intentionally empty.
        // It allows calling async methods in a fire-and-forget manner
        // without compiler warnings about unawaited tasks.
        // Be cautious, as unhandled exceptions will crash the app if not caught within the task.
    }
}
