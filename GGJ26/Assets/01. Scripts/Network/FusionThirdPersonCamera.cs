using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class FusionThirdPersonCamera : MonoBehaviour
{
    [Header("Camera Binding")]
    [SerializeField] private string cameraObjectName = "PlayerFollowCamera";
    [SerializeField] private bool bindAllVirtualCameras = false;
    [SerializeField] private Transform cameraTarget;

    [Header("Cinemachine")]
    [SerializeField] private float topClamp = 70.0f;
    [SerializeField] private float bottomClamp = -30.0f;
    [SerializeField] private float cameraAngleOverride = 0.0f;
    [SerializeField] private bool lockCameraPosition = false;

    private StarterAssetsInputs input;
    private PlayerInput playerInput;
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private bool isBound;

    private const float Threshold = 0.01f;

    private bool IsCurrentDeviceMouse
    {
        get
        {
            return playerInput != null && playerInput.currentControlScheme == "KeyboardMouse";
        }
    }

    private void Awake()
    {
        input = GetComponent<StarterAssetsInputs>();
        playerInput = GetComponent<PlayerInput>();

        if (cameraTarget == null)
        {
            var targetTransform = transform.Find("PlayerCameraRoot");
            if (targetTransform != null)
            {
                cameraTarget = targetTransform;
            }
        }

        if (cameraTarget != null)
        {
            cinemachineTargetYaw = cameraTarget.rotation.eulerAngles.y;
        }
    }

    private void OnEnable()
    {
        BindCameraTargets();
    }

    private void LateUpdate()
    {
        if (isBound == false)
        {
            BindCameraTargets();
        }

        if (cameraTarget == null || input == null)
        {
            return;
        }

        if (input.look.sqrMagnitude >= Threshold && lockCameraPosition == false)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            cinemachineTargetYaw += input.look.x * deltaTimeMultiplier;
            cinemachineTargetPitch += input.look.y * deltaTimeMultiplier;
        }

        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, bottomClamp, topClamp);

        cameraTarget.rotation = Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride, cinemachineTargetYaw, 0.0f);
    }

    private void BindCameraTargets()
    {
        var networkObject = GetComponent<Fusion.NetworkObject>();
        if (networkObject != null && networkObject.HasInputAuthority == false)
        {
            return;
        }

        if (cameraTarget == null)
        {
            return;
        }

        if (bindAllVirtualCameras)
        {
            var cameras = FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.None);
            foreach (var virtualCamera in cameras)
            {
                virtualCamera.Follow = cameraTarget;
                virtualCamera.LookAt = cameraTarget;
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
            return;
        }

        var followCamera = cameraObject.GetComponent<CinemachineVirtualCamera>();
        if (followCamera == null)
        {
            return;
        }

        followCamera.Follow = cameraTarget;
        followCamera.LookAt = cameraTarget;
        isBound = true;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
        {
            angle += 360f;
        }

        if (angle > 360f)
        {
            angle -= 360f;
        }

        return Mathf.Clamp(angle, min, max);
    }
}
