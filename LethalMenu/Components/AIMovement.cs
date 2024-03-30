using System.Linq;
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
    private CharacterController _characterController;
    private bool _isSprintHeld;
    private Keyboard _keyboard = Keyboard.current;
    private KbInput _noClipInput;
    private float _sprintTimer;

    // Components and state variables
    private float _velocityY;

    internal float CharacterSpeed = 5.0f;
    internal float CharacterSprintSpeed = 2.8f;

    // used to sync with the enemy to make sure it plays the correct animation when it is moving
    internal bool IsMoving { get; private set; }
    internal bool IsSprinting { get; private set; }


    private void Awake()
    {
        _keyboard = Keyboard.current;
        _noClipInput = gameObject.AddComponent<KbInput>();
        _characterController = gameObject.AddComponent<CharacterController>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (_noClipInput is { enabled: true }) return;
        if (_characterController is { enabled: false }) return;

        var moveInput = new Vector2(
            _keyboard.dKey.ReadValue() - _keyboard.aKey.ReadValue(),
            _keyboard.wKey.ReadValue() - _keyboard.sKey.ReadValue()
        ).normalized;

        IsMoving = moveInput.magnitude > 0.0f;

        var speedModifier = _keyboard.leftCtrlKey.isPressed
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
        _ = _characterController?.Move(moveDirection * Time.deltaTime);

        // Jump if jump key is pressed
        if (_keyboard.spaceKey.wasPressedThisFrame) Jump();

        // Sprinting mechanic: Hold to sprint
        if (_keyboard.leftShiftKey.isPressed)
        {
            if (!_isSprintHeld)
            {
                _sprintTimer = 0f;
                _isSprintHeld = true;
            }

            if (!IsSprinting && _sprintTimer >= SprintDuration) IsSprinting = true;

            _sprintTimer += Time.deltaTime;
        }

        else
        {
            _isSprintHeld = false;
            IsSprinting = false;
        }
    }

    internal void SetNoClipMode(bool enabled)
    {
        if (_noClipInput is null) return;
        _noClipInput.enabled = enabled;
    }

    internal void Init()
    {
        if (LethalMenu.LocalPlayer is not { } localPlayer) return;
        gameObject.layer = localPlayer.gameObject.layer;
    }

    internal void SetPosition(Vector3 newPosition)
    {
        if (_characterController is null) return;

        _characterController.enabled = false;
        transform.position = newPosition;
        _characterController.enabled = true;
    }


    internal void CalibrateCollision(EnemyAI enemy)
    {
        if (_characterController is null) return;

        _characterController.height = 1.0f;
        _characterController.radius = 0.5f;
        _characterController.center = new Vector3(0.0f, 0.5f, 0.0f);

        const float maxStepOffset = 0.25f;
        _characterController.stepOffset = Mathf.Min(_characterController.stepOffset, maxStepOffset);

        enemy.GetComponentsInChildren<Collider>()
            .Where(collider => collider != _characterController).ToList()
            .ForEach(collider => Physics.IgnoreCollision(_characterController, collider));
    }

    // Apply gravity to the character controller
    private void ApplyGravity()
    {
        _velocityY = _characterController is { isGrounded: false }
            ? _velocityY - Gravity * Time.deltaTime
            : 0.0f;

        Vector3 motion = new(0.0f, _velocityY, 0.0f);
        _ = _characterController?.Move(motion * Time.deltaTime);
    }

    // Jumping action
    private void Jump()
    {
        _velocityY = JumpForce;
    }
}