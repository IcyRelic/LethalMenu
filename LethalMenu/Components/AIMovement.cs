using System.Linq;
using GameNetcodeStuff;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalMenu.Components;

internal class AIMovement : MonoBehaviour
{
    // Movement constants
    private const float WalkingSpeed = 0.5f; // Walking speed when left control is held
    private const float SprintDuration = 0.0f; // Duration sprint key must be held for sprinting (adjust as needed)
    private const float JumpForce = 9.2f;
    private const float Gravity = 18.0f;
    private CharacterController CharacterController;

    internal float CharacterSpeed = 5.0f;
    internal float CharacterSprintSpeed = 2.8f;
    private bool IsSprintHeld;
    private Keyboard Keyboard = Keyboard.current;
    private KBInput NoClipInput;
    private float SprintTimer;

    // Components and state variables
    private float VelocityY;

    // used to sync with the enemy to make sure it plays the correct animation when it is moving
    internal bool IsMoving { get; private set; }
    internal bool IsSprinting { get; private set; }


    private void Awake()
    {
        Keyboard = Keyboard.current;
        NoClipInput = gameObject.AddComponent<KBInput>();
        CharacterController = gameObject.AddComponent<CharacterController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (NoClipInput is { enabled: true }) return;
        if (CharacterController is { enabled: false }) return;

        var moveInput = new Vector2(
            Keyboard.dKey.ReadValue() - Keyboard.aKey.ReadValue(),
            Keyboard.wKey.ReadValue() - Keyboard.sKey.ReadValue()
        ).normalized;

        IsMoving = moveInput.magnitude > 0.0f;

        var speedModifier = Keyboard.leftCtrlKey.isPressed
            ? WalkingSpeed
            : 1.0f;

        // Calculate movement direction relative to character's forward direction
        var forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
        var right = Vector3.ProjectOnPlane(transform.right, Vector3.up);
        var moveDirection = forward * moveInput.y + right * moveInput.x;

        // Apply speed and sprint modifiers
        moveDirection *= speedModifier * (
            IsSprinting
                ? CharacterSpeed * CharacterSprintSpeed
                : CharacterSpeed
        );

        // Apply gravity
        ApplyGravity();

        // Attempt to move
        _ = CharacterController?.Move(moveDirection * Time.deltaTime);

        // Jump if jump key is pressed
        if (Keyboard.spaceKey.wasPressedThisFrame) Jump();

        // Sprinting mechanic: Hold to sprint
        if (Keyboard.leftShiftKey.isPressed)
        {
            if (!IsSprintHeld)
            {
                SprintTimer = 0f;
                IsSprintHeld = true;
            }

            if (!IsSprinting && SprintTimer >= SprintDuration) IsSprinting = true;

            SprintTimer += Time.deltaTime;
        }

        else
        {
            IsSprintHeld = false;
            IsSprinting = false;
        }
    }

    internal void SetNoClipMode(bool enabled)
    {
        if (NoClipInput is null) return;
        NoClipInput.enabled = enabled;
    }

    internal void Init()
    {
        if (LethalMenu.localPlayer is not PlayerControllerB localPlayer) return;
        gameObject.layer = localPlayer.gameObject.layer;
    }

    internal void SetPosition(Vector3 newPosition)
    {
        if (CharacterController is null) return;

        CharacterController.enabled = false;
        transform.position = newPosition;
        CharacterController.enabled = true;
    }


    internal void CalibrateCollision(EnemyAI enemy)
    {
        if (CharacterController is null) return;

        CharacterController.height = 1.0f;
        CharacterController.radius = 0.5f;
        CharacterController.center = new Vector3(0.0f, 0.5f, 0.0f);

        const float maxStepOffset = 0.25f;
        CharacterController.stepOffset = Mathf.Min(CharacterController.stepOffset, maxStepOffset);

        enemy.GetComponentsInChildren<Collider>()
            .Where(collider => collider != CharacterController).ToList()
            .ForEach(collider => Physics.IgnoreCollision(CharacterController, collider));
    }

    // Apply gravity to the character controller
    private void ApplyGravity()
    {
        VelocityY = CharacterController is { isGrounded: false }
            ? VelocityY - Gravity * Time.deltaTime
            : 0.0f;

        Vector3 motion = new(0.0f, VelocityY, 0.0f);
        _ = CharacterController?.Move(motion * Time.deltaTime);
    }

    // Jumping action
    private void Jump()
    {
        VelocityY = JumpForce;
    }
}