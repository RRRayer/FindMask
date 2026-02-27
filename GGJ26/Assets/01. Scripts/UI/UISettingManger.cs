using System;
using UnityEngine;

public class UISettingManger : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private UISettingsController settingsPanel;

    [Header("Input")]
    [SerializeField] private InputReader inputReader;

    private Action onClosed;

    public bool IsOpen => settingsPanel != null && settingsPanel.gameObject.activeSelf;

    private void Awake()
    {
        ResolveReferences();
    }

    private void Start()
    {
        if (settingsPanel != null)
        {
            settingsPanel.gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        UnbindEvents();
    }

    public void OpenFromMenu()
    {
        OpenInternal(null);
    }

    public void OpenFromPause(Action onClose = null)
    {
        OpenInternal(onClose);
    }

    public void Close()
    {
        if (settingsPanel == null)
        {
            return;
        }

        if (settingsPanel.gameObject.activeSelf == false)
        {
            InvokeOnClosed();
            return;
        }

        UnbindEvents();
        settingsPanel.gameObject.SetActive(false);
        InvokeOnClosed();
    }

    private void OpenInternal(Action closeCallback)
    {
        if (settingsPanel == null)
        {
            Debug.LogWarning("[UISettingManger] Settings panel is missing.");
            return;
        }

        onClosed = closeCallback;
        BindEvents();
        settingsPanel.gameObject.SetActive(true);
    }

    private void OnCloseRequested()
    {
        Close();
    }

    private void ResolveReferences()
    {
        if (settingsPanel == null)
        {
            settingsPanel = FindObjectByName<UISettingsController>("MainMenuSettingsPanel");
        }

        if (settingsPanel == null)
        {
            settingsPanel = FindFirstObjectByType<UISettingsController>(FindObjectsInactive.Include);
        }

        if (inputReader == null)
        {
            inputReader = FindInputReaderByName("InputReader");
        }
    }

    private void BindEvents()
    {
        UnbindEvents();

        settingsPanel.CloseButtonAction += OnCloseRequested;
        if (inputReader != null)
        {
            inputReader.CancelEvent += OnCloseRequested;
        }
    }

    private void UnbindEvents()
    {
        if (settingsPanel != null)
        {
            settingsPanel.CloseButtonAction -= OnCloseRequested;
        }

        if (inputReader != null)
        {
            inputReader.CancelEvent -= OnCloseRequested;
        }
    }

    private void InvokeOnClosed()
    {
        Action callback = onClosed;
        onClosed = null;
        callback?.Invoke();
    }

    private T FindObjectByName<T>(string objectName) where T : Component
    {
        T[] objects = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < objects.Length; i++)
        {
            T candidate = objects[i];
            if (candidate != null && candidate.gameObject.name == objectName)
            {
                return candidate;
            }
        }

        return null;
    }

    private InputReader FindInputReaderByName(string assetName)
    {
        InputReader[] inputReaders = Resources.FindObjectsOfTypeAll<InputReader>();
        for (int i = 0; i < inputReaders.Length; i++)
        {
            InputReader candidate = inputReaders[i];
            if (candidate != null && candidate.name == assetName)
            {
                return candidate;
            }
        }

        return null;
    }
}
