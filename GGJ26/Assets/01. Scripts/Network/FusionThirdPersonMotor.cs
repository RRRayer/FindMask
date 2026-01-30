using Fusion;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FusionThirdPersonMotor : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float sprintSpeed = 6f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private bool debugInput = false;

    [Networked] private float VerticalVelocity { get; set; }

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority == false)
        {
            return;
        }

        if (controller == null)
        {
            return;
        }

        if (GetInput(out PlayerInputData input) == false)
        {
            if (debugInput)
            {
                Debug.Log("[FusionThirdPersonMotor] No input for tick", this);
            }
            ApplyGravity(Vector3.zero);
            return;
        }

        Vector3 move = new Vector3(input.Move.x, 0f, input.Move.y);
        if (debugInput)
        {
            Debug.Log($"[FusionThirdPersonMotor] move={move} jump={input.Jump} sprint={input.Sprint}", this);
        }
        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        float speed = input.Sprint ? sprintSpeed : moveSpeed;
        Vector3 horizontal = move * speed;

        if (move != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(move);
        }

        ApplyGravity(horizontal, input.Jump);
    }

    private void ApplyGravity(Vector3 horizontal, bool jump = false)
    {
        if (controller.isGrounded)
        {
            if (jump)
            {
                VerticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else if (VerticalVelocity < 0f)
            {
                VerticalVelocity = -2f;
            }
        }

        VerticalVelocity += gravity * Runner.DeltaTime;

        Vector3 motion = new Vector3(horizontal.x, VerticalVelocity, horizontal.z) * Runner.DeltaTime;
        controller.Move(motion);
    }
}
