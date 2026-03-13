using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpectatorController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float verticalSpeed = 4f;
    [SerializeField] private float lookSensitivity = 0.15f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [Header("Cinemachine")]
    [SerializeField] private string cameraObjectName = "PlayerFollowCamera";
    [SerializeField] private bool bindAllVirtualCameras = false;
    [SerializeField] private bool matchMainCameraFieldOfView = true;
    [Header("Spectator View UI")]
    [SerializeField] private string selectPanelName = "SelectPanel";
    [SerializeField] private string viewLabelName = "Instruction";
    [SerializeField] private float targetRefreshInterval = 0.5f;

    private Camera mainCamera;
    private float yaw;
    private float pitch;
    private CinemachineVirtualCamera boundCamera;
    private Transform previousFollow;
    private Transform previousLookAt;
    private float previousFieldOfView = -1f;
    private TMP_Text viewLabel;
    private bool uiBound;
    private float nextRefreshTime;
    private readonly List<ViewEntry> viewEntries = new();
    private int currentViewIndex;
    private int currentTargetPlayerId = -1;
    private Cinemachine3rdPersonFollow spectatorThirdPersonFollow;
    private bool spectatorCameraProfileCached;
    private float spectatorDefaultCameraDistance;
    private float spectatorDefaultCameraSide;
    private Vector3 spectatorDefaultShoulderOffset;
    private float spectatorDefaultVerticalArmLength;

    private sealed class ViewEntry
    {
        public string Label;
        public int PlayerId;
        public Transform ViewTarget;
        public bool IsFree;
        public FusionThirdPersonCamera SourceCamera;
    }

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        if (mainCamera != null)
        {
            transform.position = mainCamera.transform.position;
            transform.rotation = mainCamera.transform.rotation;
        }

        BindCameraTargets();
        ApplyCurrentCameraFieldOfView();
        RefreshViewEntries();
        SetCurrentViewByPlayerId(-1);
        TryBindSelectionUi();

        var forward = transform.forward;
        yaw = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        pitch = 0f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        UnbindSelectionUi();
        RestoreCameraTargets();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        if (uiBound == false)
        {
            TryBindSelectionUi();
        }

        if (Time.time >= nextRefreshTime)
        {
            nextRefreshTime = Time.time + targetRefreshInterval;
            RefreshViewEntries();
            ValidateCurrentView();
        }

        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.leftArrowKey.wasPressedThisFrame)
            {
                StepView(-1);
            }
            else if (keyboard.rightArrowKey.wasPressedThisFrame)
            {
                StepView(1);
            }
        }

        if (IsFreeView() == false)
        {
            SyncToTargetView();
            return;
        }

        if (keyboard == null)
        {
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return;
            }
        }

        var mouse = Mouse.current;
        if (mouse != null)
        {
            Vector2 delta = mouse.delta.ReadValue() * lookSensitivity;
            yaw += delta.x;
            pitch -= delta.y;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        Vector3 input = Vector3.zero;
        if (keyboard.wKey.isPressed) input += Vector3.forward;
        if (keyboard.sKey.isPressed) input += Vector3.back;
        if (keyboard.aKey.isPressed) input += Vector3.left;
        if (keyboard.dKey.isPressed) input += Vector3.right;

        Vector3 vertical = Vector3.zero;
        if (keyboard.qKey.isPressed) vertical += Vector3.up;
        if (keyboard.eKey.isPressed) vertical += Vector3.down;

        Vector3 camForward = transform.forward;
        camForward.y = 0f;
        camForward.Normalize();
        Vector3 camRight = transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 move = (camForward * input.z + camRight * input.x) * moveSpeed;
        move += vertical * verticalSpeed;

        transform.position += move * Time.deltaTime;
    }

    private void BindCameraTargets()
    {
        if (boundCamera == null)
        {
            boundCamera = GetComponent<CinemachineVirtualCamera>();
            if (boundCamera != null)
            {
                previousFollow = boundCamera.Follow;
                previousLookAt = boundCamera.LookAt;
                previousFieldOfView = boundCamera.m_Lens.FieldOfView;
                boundCamera.Follow = null;
                boundCamera.LookAt = null;
                spectatorThirdPersonFollow = boundCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
                CacheSpectatorDefaultCameraProfile();
                return;
            }
        }

        if (bindAllVirtualCameras)
        {
            var cameras = FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.None);
            foreach (var virtualCamera in cameras)
            {
                virtualCamera.Follow = transform;
                virtualCamera.LookAt = transform;
            }
            return;
        }

        if (string.IsNullOrWhiteSpace(cameraObjectName))
        {
            return;
        }

        var cameraObject = GameObject.Find(cameraObjectName);
        if (cameraObject == null)
        {
            if (boundCamera == null)
            {
                boundCamera = GetComponent<CinemachineVirtualCamera>();
                if (boundCamera != null)
                {
                    previousFollow = boundCamera.Follow;
                    previousLookAt = boundCamera.LookAt;
                    previousFieldOfView = boundCamera.m_Lens.FieldOfView;
                    boundCamera.Follow = null;
                    boundCamera.LookAt = null;
                    spectatorThirdPersonFollow = boundCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
                    CacheSpectatorDefaultCameraProfile();
                }
            }
            return;
        }

        boundCamera = cameraObject.GetComponent<CinemachineVirtualCamera>();
        if (boundCamera == null)
        {
            return;
        }

        previousFollow = boundCamera.Follow;
        previousLookAt = boundCamera.LookAt;
        previousFieldOfView = boundCamera.m_Lens.FieldOfView;
        boundCamera.Follow = transform;
        boundCamera.LookAt = transform;
        spectatorThirdPersonFollow = boundCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        CacheSpectatorDefaultCameraProfile();
    }

    private void ApplyCurrentCameraFieldOfView()
    {
        if (matchMainCameraFieldOfView == false || boundCamera == null)
        {
            return;
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            return;
        }

        boundCamera.m_Lens.FieldOfView = mainCamera.fieldOfView;
    }

    private void RestoreCameraTargets()
    {
        if (boundCamera == null)
        {
            return;
        }

        boundCamera.Follow = previousFollow;
        boundCamera.LookAt = previousLookAt;
        if (previousFieldOfView > 0f)
        {
            boundCamera.m_Lens.FieldOfView = previousFieldOfView;
        }

        boundCamera = null;
        spectatorThirdPersonFollow = null;
        previousFollow = null;
        previousLookAt = null;
        previousFieldOfView = -1f;
    }

    private void CacheSpectatorDefaultCameraProfile()
    {
        if (spectatorCameraProfileCached || spectatorThirdPersonFollow == null)
        {
            return;
        }

        spectatorDefaultCameraDistance = spectatorThirdPersonFollow.CameraDistance;
        spectatorDefaultCameraSide = spectatorThirdPersonFollow.CameraSide;
        spectatorDefaultShoulderOffset = spectatorThirdPersonFollow.ShoulderOffset;
        spectatorDefaultVerticalArmLength = spectatorThirdPersonFollow.VerticalArmLength;
        spectatorCameraProfileCached = true;
    }

    private void RefreshViewEntries()
    {
        viewEntries.Clear();
        viewEntries.Add(new ViewEntry
        {
            IsFree = true,
            Label = "Free",
            PlayerId = -1,
            ViewTarget = transform
        });

        var eliminations = FindObjectsByType<PlayerElimination>(FindObjectsSortMode.None);
        List<ViewEntry> alivePlayers = new();
        for (int i = 0; i < eliminations.Length; i++)
        {
            var elimination = eliminations[i];
            if (elimination == null)
            {
                continue;
            }

            var networkObject = elimination.GetComponent<Fusion.NetworkObject>();
            if (networkObject == null || networkObject.IsValid == false)
            {
                continue;
            }

            if (networkObject.HasInputAuthority)
            {
                continue;
            }

            if (elimination.IsEliminated)
            {
                continue;
            }

            alivePlayers.Add(new ViewEntry
            {
                IsFree = false,
                Label = $"Player {networkObject.InputAuthority.RawEncoded}",
                PlayerId = networkObject.InputAuthority.RawEncoded,
                ViewTarget = ResolveViewTarget(elimination.transform),
                SourceCamera = elimination.GetComponent<FusionThirdPersonCamera>()
            });
        }

        alivePlayers.Sort((a, b) => a.PlayerId.CompareTo(b.PlayerId));
        viewEntries.AddRange(alivePlayers);
    }

    private static Transform ResolveViewTarget(Transform playerRoot)
    {
        if (playerRoot == null)
        {
            return null;
        }

        var target = playerRoot.Find("PlayerCameraRoot");
        return target != null ? target : playerRoot;
    }

    private void TryBindSelectionUi()
    {
        Transform selectPanel = FindSceneObjectByName(selectPanelName);
        if (selectPanel == null)
        {
            return;
        }

        viewLabel = FindChildComponentByName<TMP_Text>(selectPanel, viewLabelName);

        if (viewLabel == null)
        {
            return;
        }

        uiBound = true;
        UpdateViewLabel();
    }

    private void UnbindSelectionUi()
    {
        uiBound = false;
        viewLabel = null;
    }

    private void StepView(int direction)
    {
        RefreshViewEntries();
        if (viewEntries.Count == 0)
        {
            return;
        }

        int count = viewEntries.Count;
        int nextIndex = ((currentViewIndex + direction) % count + count) % count;
        currentViewIndex = nextIndex;
        ApplyCurrentView();
    }

    private void SetCurrentViewByPlayerId(int playerId)
    {
        currentViewIndex = 0;
        for (int i = 0; i < viewEntries.Count; i++)
        {
            if (viewEntries[i].PlayerId == playerId)
            {
                currentViewIndex = i;
                break;
            }
        }

        ApplyCurrentView();
    }

    private void ApplyCurrentView()
    {
        if (viewEntries.Count == 0)
        {
            currentViewIndex = 0;
            currentTargetPlayerId = -1;
            ApplyBoundCameraTarget(null);
            UpdateViewLabel();
            return;
        }

        currentViewIndex = Mathf.Clamp(currentViewIndex, 0, viewEntries.Count - 1);
        var entry = viewEntries[currentViewIndex];
        currentTargetPlayerId = entry.PlayerId;

        if (entry.IsFree)
        {
            ApplyBoundCameraTarget(null);
            RestoreSpectatorCameraProfile();
            var forward = transform.forward;
            yaw = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
            pitch = transform.eulerAngles.x;
            if (pitch > 180f)
            {
                pitch -= 360f;
            }
        }
        else
        {
            ApplyBoundCameraTarget(entry.ViewTarget != null ? entry.ViewTarget : transform);
            SyncToTargetView();
            ApplySpectatedCameraProfile(entry);
        }

        UpdateViewLabel();
    }

    private void ValidateCurrentView()
    {
        if (IsFreeView())
        {
            UpdateViewLabel();
            return;
        }

        for (int i = 0; i < viewEntries.Count; i++)
        {
            if (viewEntries[i].PlayerId == currentTargetPlayerId)
            {
                currentViewIndex = i;
                UpdateViewLabel();
                return;
            }
        }

        StepToNextAliveViewAfterTargetLoss();
    }

    private void StepToNextAliveViewAfterTargetLoss()
    {
        if (viewEntries.Count <= 1)
        {
            currentViewIndex = 0;
            ApplyCurrentView();
            return;
        }

        int nextIndex = 0;
        for (int i = 1; i < viewEntries.Count; i++)
        {
            if (viewEntries[i].PlayerId > currentTargetPlayerId)
            {
                nextIndex = i;
                break;
            }
        }

        currentViewIndex = nextIndex;
        ApplyCurrentView();
    }

    private bool IsFreeView()
    {
        if (viewEntries.Count == 0)
        {
            return true;
        }

        currentViewIndex = Mathf.Clamp(currentViewIndex, 0, viewEntries.Count - 1);
        return viewEntries[currentViewIndex].IsFree;
    }

    private void SyncToTargetView()
    {
        if (viewEntries.Count == 0 || currentViewIndex <= 0 || currentViewIndex >= viewEntries.Count)
        {
            return;
        }

        var entry = viewEntries[currentViewIndex];
        if (entry.ViewTarget == null)
        {
            return;
        }

        transform.SetPositionAndRotation(entry.ViewTarget.position, entry.ViewTarget.rotation);
        var euler = transform.eulerAngles;
        yaw = euler.y;
        pitch = euler.x > 180f ? euler.x - 360f : euler.x;
    }

    private void ApplyBoundCameraTarget(Transform target)
    {
        if (boundCamera == null)
        {
            return;
        }

        boundCamera.Follow = target;
        boundCamera.LookAt = target;
    }

    private void ApplySpectatedCameraProfile(ViewEntry entry)
    {
        if (spectatorThirdPersonFollow == null)
        {
            return;
        }

        if (entry.SourceCamera != null && entry.SourceCamera.TryGetActiveCameraProfile(out var profile) && profile.IsValid)
        {
            spectatorThirdPersonFollow.CameraDistance = profile.CameraDistance;
            spectatorThirdPersonFollow.CameraSide = profile.CameraSide;
            spectatorThirdPersonFollow.ShoulderOffset = profile.ShoulderOffset;
            spectatorThirdPersonFollow.VerticalArmLength = profile.VerticalArmLength;
            return;
        }

        RestoreSpectatorCameraProfile();
    }

    private void RestoreSpectatorCameraProfile()
    {
        if (spectatorThirdPersonFollow == null)
        {
            return;
        }

        CacheSpectatorDefaultCameraProfile();
        if (spectatorCameraProfileCached == false)
        {
            return;
        }

        spectatorThirdPersonFollow.CameraDistance = spectatorDefaultCameraDistance;
        spectatorThirdPersonFollow.CameraSide = spectatorDefaultCameraSide;
        spectatorThirdPersonFollow.ShoulderOffset = spectatorDefaultShoulderOffset;
        spectatorThirdPersonFollow.VerticalArmLength = spectatorDefaultVerticalArmLength;
    }

    private void UpdateViewLabel()
    {
        if (viewLabel == null)
        {
            return;
        }

        if (viewEntries.Count == 0)
        {
            viewLabel.text = "Free";
            return;
        }

        currentViewIndex = Mathf.Clamp(currentViewIndex, 0, viewEntries.Count - 1);
        viewLabel.text = viewEntries[currentViewIndex].Label;
    }

    private static Transform FindSceneObjectByName(string objectName)
    {
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return null;
        }

        var active = GameObject.Find(objectName);
        if (active != null)
        {
            return active.transform;
        }

        var transforms = Resources.FindObjectsOfTypeAll<Transform>();
        for (int i = 0; i < transforms.Length; i++)
        {
            var current = transforms[i];
            if (current == null || current.gameObject.scene.IsValid() == false)
            {
                continue;
            }

            if (current.name == objectName)
            {
                return current;
            }
        }

        return null;
    }

    private static T FindChildComponentByName<T>(Transform root, string objectName) where T : Component
    {
        if (root == null || string.IsNullOrWhiteSpace(objectName))
        {
            return null;
        }

        var components = root.GetComponentsInChildren<T>(true);
        for (int i = 0; i < components.Length; i++)
        {
            var component = components[i];
            if (component != null && component.name == objectName)
            {
                return component;
            }
        }

        return null;
    }
}


