using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIGenericButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button button;
    [SerializeField] private bool playClickSfx = true;
    [SerializeField] private AudioClip clickSfxClip;
    [SerializeField, Range(0f, 1f)] private float clickSfxVolume = 1f;
    
    public UnityAction Clicked;
    private static AudioSource sharedUiAudioSource;

    public void Click()
    {
        PlayClickSfx();

        if (Clicked == null)
        {
            Debug.LogWarning($"[UIGenericButton] Clicked listener is not assigned on {name}.", this);
            return;
        }

        Clicked.Invoke();
    }

    public void SetButton(string newText)
    {
        if (buttonText != null)
        {
            buttonText.text = newText;
        }
    }

    private void PlayClickSfx()
    {
        if (playClickSfx == false || clickSfxClip == null)
        {
            return;
        }

        EnsureSharedUiAudioSource();
        if (sharedUiAudioSource != null)
        {
            sharedUiAudioSource.PlayOneShot(clickSfxClip, clickSfxVolume);
        }
    }

    private static void EnsureSharedUiAudioSource()
    {
        if (sharedUiAudioSource != null)
        {
            return;
        }

        var go = new GameObject("UI_ClickSfx_AudioSource");
        DontDestroyOnLoad(go);
        sharedUiAudioSource = go.AddComponent<AudioSource>();
        sharedUiAudioSource.playOnAwake = false;
        sharedUiAudioSource.loop = false;
        sharedUiAudioSource.spatialBlend = 0f;
        sharedUiAudioSource.volume = 1f;
    }
}
