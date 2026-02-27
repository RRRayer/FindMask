using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIPause : MonoBehaviour
{
    [Header("UI Components")]
    [FormerlySerializedAs("resumeButton")]
    [SerializeField] private Button backButton;
    [SerializeField] private Button settingsButton;
    [FormerlySerializedAs("loadMainMenuButton")]
    [SerializeField] private Button exitButton;

    public event UnityAction BackEvent;
    public event UnityAction ResumeEvent;
    public event UnityAction SettingsEvent;
    public event UnityAction ExitEvent;

    // Legacy events kept to avoid compile breaks in old callers.
    // public event UnityAction RestartEvent;
    // public event UnityAction LoadMainMenuEvent;

    private void OnEnable()
    {
        if (backButton != null) backButton.onClick.AddListener(OnBackButtonClicked);
        if (settingsButton != null) settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        if (exitButton != null) exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnDisable()
    {
        if (backButton != null) backButton.onClick.RemoveListener(OnBackButtonClicked);
        if (settingsButton != null) settingsButton.onClick.RemoveListener(OnSettingsButtonClicked);
        if (exitButton != null) exitButton.onClick.RemoveListener(OnExitButtonClicked);
    }

    private void OnBackButtonClicked()
    {
        BackEvent?.Invoke();
        ResumeEvent?.Invoke();
    }

    private void OnSettingsButtonClicked()
    {
        SettingsEvent?.Invoke();
    }

    private void OnExitButtonClicked()
    {
        ExitEvent?.Invoke();
    }
}
