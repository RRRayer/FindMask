using System.Collections.Generic;
using Fusion;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NetworkObject))]
public class FusionPlayerAvatar : NetworkBehaviour
{
    [SerializeField] private bool requestStateAuthorityOnSpawn = true;
    [SerializeField] private bool debugAuthority = false;
    [SerializeField] private UnityEngine.Behaviour[] localOnlyBehaviours;
    [SerializeField] private GameObject[] localOnlyObjects;

    private bool? lastIsLocal;
    private int lastDebugFrame = -9999;
    private PlayerInput playerInput;

    private void Awake()
    {
        var hasNetworkMotor = GetComponent<FusionThirdPersonMotor>() != null;

        if (localOnlyBehaviours == null || localOnlyBehaviours.Length == 0)
        {
            var behaviours = new List<UnityEngine.Behaviour>();

            var controller = GetComponent<ThirdPersonController>();
            if (controller != null)
            {
                behaviours.Add(controller);
            }

            var cameraController = GetComponent<FusionThirdPersonCamera>();
            if (cameraController != null)
            {
                behaviours.Add(cameraController);
            }

            var inputs = GetComponent<StarterAssetsInputs>();
            if (inputs != null)
            {
                behaviours.Add(inputs);
            }

            var playerInput = GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                behaviours.Add(playerInput);
            }

            localOnlyBehaviours = behaviours.ToArray();
        }

        playerInput = GetComponent<PlayerInput>();

        if (hasNetworkMotor)
        {
            var thirdPerson = GetComponent<ThirdPersonController>();
            if (thirdPerson != null)
            {
                thirdPerson.enabled = false;
            }
        }
    }

    public override void Spawned()
    {
        ApplyAuthorityState();
    }

    private void OnEnable()
    {
        if (debugAuthority)
        {
            Debug.Log("[FusionPlayerAvatar] OnEnable", this);
        }
    }

    private void Start()
    {
        if (debugAuthority)
        {
            Debug.Log("[FusionPlayerAvatar] Start", this);
        }
    }

    private void Update()
    {
        if (debugAuthority && Time.frameCount - lastDebugFrame >= 60)
        {
            lastDebugFrame = Time.frameCount;
            Debug.Log(
                $"[FusionPlayerAvatar] Update tick local={Object.HasInputAuthority} inputAuth={Object.InputAuthority} stateAuth={Object.HasStateAuthority}",
                this);
        }
        ApplyAuthorityState();
    }

    private void ApplyAuthorityState()
    {
        if (Object == null)
        {
            return;
        }

        var isLocal = Object.HasInputAuthority || (Runner != null && Object.InputAuthority == Runner.LocalPlayer);
        if (lastIsLocal.HasValue && lastIsLocal.Value == isLocal)
        {
            return;
        }

        lastIsLocal = isLocal;

        if (debugAuthority)
        {
            Debug.Log(
                $"[FusionPlayerAvatar] local={isLocal} inputAuth={Object.InputAuthority} hasInput={Object.HasInputAuthority} hasState={Object.HasStateAuthority} localPlayer={Runner?.LocalPlayer}",
                this);
        }

        if (isLocal && requestStateAuthorityOnSpawn && Object.HasStateAuthority == false)
        {
            Object.Flags |= NetworkObjectFlags.AllowStateAuthorityOverride;
            Object.RequestStateAuthority();
        }

        if (localOnlyBehaviours != null)
        {
            foreach (var behaviour in localOnlyBehaviours)
            {
                if (behaviour != null)
                {
                    behaviour.enabled = isLocal;
                }
            }
        }

        if (localOnlyObjects != null)
        {
            foreach (var obj in localOnlyObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(isLocal);
                }
            }
        }

        if (isLocal && playerInput != null)
        {
            if (playerInput.enabled == false)
            {
                playerInput.enabled = true;
            }

            playerInput.ActivateInput();
            if (playerInput.currentActionMap == null || playerInput.currentActionMap.name != "Player")
            {
                playerInput.SwitchCurrentActionMap("Player");
            }
        }


        if (isLocal && Time.timeScale == 0f)
        {
            Debug.LogWarning("Time.timeScale is 0 in Game scene. Movement and gravity will not update.");
        }
    }

}
