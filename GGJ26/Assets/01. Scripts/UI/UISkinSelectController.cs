using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UISkinSelectController : MonoBehaviour
{
    [System.Serializable]
    private class SkinEntry
    {
        public string skinName = "Skin";
        public GameObject previewRoot;
    }

    [Header("Skin Data")]
    [SerializeField] private SkinEntry[] skins = new SkinEntry[0];
    [SerializeField] private int defaultSkinIndex = 0;

    [Header("UI")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button equipButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI skinNameText;
    [SerializeField] private TextMeshProUGUI equipButtonText;
    [SerializeField] private GameObject previewActorRoot;
    [SerializeField] private string previewActorName = "PreviewSeekerModel";

    [Header("Input")]
    [SerializeField] private KeyCode prevKey = KeyCode.LeftArrow;
    [SerializeField] private KeyCode nextKey = KeyCode.RightArrow;
    [SerializeField] private KeyCode prevAltKey = KeyCode.A;
    [SerializeField] private KeyCode nextAltKey = KeyCode.D;
    [SerializeField] private KeyCode closeKey = KeyCode.Escape;

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private float runtimeApplyRetryInterval = 0.5f;
    [SerializeField] private float runtimeApplyTimeout = 15f;

    private int currentIndex;
    private int equippedIndex;
    private bool isOpen;
    private PlayerAppearance previewAppearance;
    private bool pendingRuntimeApply;
    private float runtimeApplyDeadline;
    private float nextRuntimeApplyTime;

    private void Awake()
    {
        equippedIndex = SeekerSkinSelection.LoadSelectedSkinIndex(defaultSkinIndex);
        currentIndex = Mathf.Clamp(equippedIndex, 0, Mathf.Max(0, GetSkinCount() - 1));

        if (panelRoot == null)
        {
            panelRoot = gameObject;
        }

        EnsurePreviewActorResolved();
        if (previewActorRoot != null)
        {
            previewActorRoot.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (prevButton != null) prevButton.onClick.AddListener(Prev);
        if (nextButton != null) nextButton.onClick.AddListener(Next);
        if (equipButton != null) equipButton.onClick.AddListener(EquipCurrent);
        if (closeButton != null) closeButton.onClick.AddListener(Close);
        ApplyPreview();
    }

    private void OnDisable()
    {
        if (prevButton != null) prevButton.onClick.RemoveListener(Prev);
        if (nextButton != null) nextButton.onClick.RemoveListener(Next);
        if (equipButton != null) equipButton.onClick.RemoveListener(EquipCurrent);
        if (closeButton != null) closeButton.onClick.RemoveListener(Close);
    }

    private void Update()
    {
        if (pendingRuntimeApply && Time.unscaledTime >= nextRuntimeApplyTime)
        {
            bool applied = ApplyEquippedSkinToLocalSeeker();
            if (applied)
            {
                pendingRuntimeApply = false;
            }
            else if (Time.unscaledTime > runtimeApplyDeadline)
            {
                pendingRuntimeApply = false;
                if (enableDebugLogs)
                {
                    Debug.LogWarning("[SkinSelect] Runtime apply timed out. Keeping saved value only.");
                }
            }
            else
            {
                nextRuntimeApplyTime = Time.unscaledTime + Mathf.Max(0.1f, runtimeApplyRetryInterval);
            }
        }

        if (isOpen == false)
        {
            return;
        }

        if (WasPressedThisFrame(prevKey) || WasPressedThisFrame(prevAltKey))
        {
            Prev();
        }

        if (WasPressedThisFrame(nextKey) || WasPressedThisFrame(nextAltKey))
        {
            Next();
        }

        if (WasPressedThisFrame(closeKey))
        {
            Close();
        }
    }

    public void Open()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }

        equippedIndex = SeekerSkinSelection.LoadSelectedSkinIndex(defaultSkinIndex);
        currentIndex = Mathf.Clamp(equippedIndex, 0, Mathf.Max(0, GetSkinCount() - 1));
        isOpen = true;
        if (previewActorRoot != null)
        {
            previewActorRoot.SetActive(true);
        }

        var preview = GetPreviewAppearance();
        if (preview != null)
        {
            preview.SetPreviewMode(true);
        }
        ApplyPreview();
    }

    public void Close()
    {
        isOpen = false;
        if (previewAppearance != null)
        {
            previewAppearance.SetPreviewMode(false);
        }

        if (previewActorRoot != null)
        {
            previewActorRoot.SetActive(false);
        }
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
    }

    public void Next()
    {
        int skinCount = GetSkinCount();
        if (skinCount == 0)
        {
            return;
        }

        currentIndex = (currentIndex + 1) % skinCount;
        ApplyPreview();
    }

    public void Prev()
    {
        int skinCount = GetSkinCount();
        if (skinCount == 0)
        {
            return;
        }

        currentIndex = (currentIndex - 1 + skinCount) % skinCount;
        ApplyPreview();
    }

    public void EquipCurrent()
    {
        if (GetSkinCount() == 0)
        {
            if (enableDebugLogs)
            {
                Debug.Log("[SkinSelect] Equip ignored: skin count is 0.");
            }
            return;
        }

        equippedIndex = currentIndex;
        SeekerSkinSelection.SaveSelectedSkinIndex(equippedIndex);
        if (enableDebugLogs)
        {
            Debug.Log($"[SkinSelect] Equip clicked. currentIndex={currentIndex}, savedIndex={equippedIndex}");
        }
        pendingRuntimeApply = true;
        runtimeApplyDeadline = Time.unscaledTime + Mathf.Max(1f, runtimeApplyTimeout);
        nextRuntimeApplyTime = Time.unscaledTime;
        ApplyEquippedSkinToLocalSeeker();
        UpdateEquipLabel();
    }

    private void ApplyPreview()
    {
        int skinCount = GetSkinCount();
        if (skinCount <= 0)
        {
            return;
        }

        for (int i = 0; i < skinCount; i++)
        {
            var preview = i < skins.Length && skins[i] != null ? skins[i].previewRoot : null;
            if (preview != null)
            {
                preview.SetActive(i == currentIndex);
            }
        }

        if (previewAppearance != null)
        {
            previewAppearance.SetPreviewSeekerSkinIndex(currentIndex);
        }

        if (skinNameText != null)
        {
            if (skins != null && currentIndex < skins.Length && string.IsNullOrWhiteSpace(skins[currentIndex].skinName) == false)
            {
                skinNameText.text = skins[currentIndex].skinName;
            }
            else
            {
                skinNameText.text = $"Mask {currentIndex + 1}";
            }
        }

        UpdateEquipLabel();
    }

    private void UpdateEquipLabel()
    {
        if (equipButtonText == null)
        {
            return;
        }

        equipButtonText.text = currentIndex == equippedIndex ? "Equipped" : "Equip";
    }

    private bool WasPressedThisFrame(KeyCode key)
    {
        if (Keyboard.current != null)
        {
            switch (key)
            {
                case KeyCode.LeftArrow:
                    return Keyboard.current.leftArrowKey.wasPressedThisFrame;
                case KeyCode.RightArrow:
                    return Keyboard.current.rightArrowKey.wasPressedThisFrame;
                case KeyCode.A:
                    return Keyboard.current.aKey.wasPressedThisFrame;
                case KeyCode.D:
                    return Keyboard.current.dKey.wasPressedThisFrame;
                case KeyCode.Escape:
                    return Keyboard.current.escapeKey.wasPressedThisFrame;
            }
        }

#if ENABLE_LEGACY_INPUT_MANAGER
        return Input.GetKeyDown(key);
#else
        return false;
#endif
    }

    private int GetSkinCount()
    {
        int dataCount = skins != null ? skins.Length : 0;
        int previewCount = 0;
        var preview = GetPreviewAppearance();
        if (preview != null)
        {
            previewCount = preview.GetSeekerMaskCount();
        }
        return Mathf.Max(dataCount, previewCount);
    }

    private PlayerAppearance GetPreviewAppearance()
    {
        EnsurePreviewActorResolved();

        if (previewAppearance == null && previewActorRoot != null)
        {
            previewAppearance = previewActorRoot.GetComponent<PlayerAppearance>();
        }

        return previewAppearance;
    }

    private void EnsurePreviewActorResolved()
    {
        if (previewActorRoot != null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(previewActorName))
        {
            return;
        }

        Transform[] allTransforms = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < allTransforms.Length; i++)
        {
            Transform tr = allTransforms[i];
            if (tr == null || tr.name != previewActorName)
            {
                continue;
            }

            Scene scene = tr.gameObject.scene;
            if (scene.IsValid() == false || scene.isLoaded == false)
            {
                continue;
            }

            previewActorRoot = tr.gameObject;
            break;
        }
    }

    private bool ApplyEquippedSkinToLocalSeeker()
    {
        var roles = FindObjectsByType<PlayerRole>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        bool requested = false;
        if (enableDebugLogs)
        {
            Debug.Log($"[SkinSelect] Trying runtime apply. roleCount={roles.Length}, targetSkinIndex={equippedIndex}");
        }

        for (int i = 0; i < roles.Length; i++)
        {
            var candidate = roles[i];
            if (candidate == null || candidate.HasRoleAssigned() == false || candidate.IsSeeker == false)
            {
                continue;
            }

            var networkObject = candidate.GetComponent<Fusion.NetworkObject>();
            if (networkObject == null || networkObject.HasInputAuthority == false)
            {
                continue;
            }

            requested = true;
            if (enableDebugLogs)
            {
                Debug.Log($"[SkinSelect] Runtime apply target found: {candidate.name}, hasStateAuth={networkObject.HasStateAuthority}, hasInputAuth={networkObject.HasInputAuthority}");
            }
            candidate.RequestSeekerSkinChange(equippedIndex);
        }

        if (enableDebugLogs && requested == false)
        {
            Debug.Log("[SkinSelect] Runtime apply skipped: no local seeker with input authority found. Save only.");
        }

        return requested;
    }
}
