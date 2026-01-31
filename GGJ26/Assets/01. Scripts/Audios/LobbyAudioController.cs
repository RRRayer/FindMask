using UnityEngine;

public class LobbyAudioController : MonoBehaviour
{
    [Header("Audio Config")]
    [SerializeField] private AudioConfigurationSO musicConfiguration;

    [Header("Lobby BGM Cue")]
    [SerializeField] private AudioCueSO lobbyBgmCue;

    private AudioManager audioManager;
    private SoundEmitter lobbyEmitter;

    private void Start()
    {
        audioManager = AudioManager.Instance;
        if (audioManager == null || lobbyBgmCue == null || musicConfiguration == null)
        {
            return;
        }

        lobbyEmitter = audioManager.PlayLoopingMusic(lobbyBgmCue, musicConfiguration, transform.position);
    }

    private void OnDisable()
    {
        if (audioManager == null)
        {
            audioManager = AudioManager.Instance;
        }

        if (audioManager != null && lobbyEmitter != null)
        {
            audioManager.StopEmitter(lobbyEmitter);
            lobbyEmitter = null;
        }
    }
}
