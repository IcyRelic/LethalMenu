using GameNetcodeStuff;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalMenu.Components
{
    internal class AIMovement : MonoBehaviour
    {
        private const float WalkingSpeed = 0.5f; 
        private const float SprintDuration = 0.0f; 
        private const float JumpForce = 9.2f;
        private const float Gravity = 18.0f;

        internal float CharacterSpeed = 5.0f;
        internal float CharacterSprintSpeed = 2.8f;

        internal bool IsMoving { get; private set; } = false;
        internal bool IsSprinting { get; private set; } = false;

        private float VelocityY = 0.0f;
        private bool IsSprintHeld = false;
        private float SprintTimer = 0.0f;
        private Keyboard Keyboard = Keyboard.current;
        private KBInput? NoClipInput;
        private CharacterController? CharacterController;

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

            float maxStepOffset = 0.25f;
            CharacterController.stepOffset = Mathf.Min(CharacterController.stepOffset, maxStepOffset);

            enemy.GetComponentsInChildren<Collider>()
                 .Where(collider => collider != CharacterController).ToList()
                 .ForEach(collider => Physics.IgnoreCollision(CharacterController, collider));
        }


        public void Awake()
        {
            Keyboard = Keyboard.current;
            NoClipInput = gameObject.AddComponent<KBInput>();
            CharacterController = gameObject.AddComponent<CharacterController>();
        }

        public void Update()
        {
            if (NoClipInput is { enabled: true }) return;
            if (CharacterController is { enabled: false }) return;

            Vector2 moveInput = new Vector2(
                Keyboard.dKey.ReadValue() - Keyboard.aKey.ReadValue(),
                Keyboard.wKey.ReadValue() - Keyboard.sKey.ReadValue()
            ).normalized;

            IsMoving = moveInput.magnitude > 0.0f;

            float speedModifier = Keyboard.leftCtrlKey.isPressed
                ? WalkingSpeed
                : 1.0f;

            Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            Vector3 right = Vector3.ProjectOnPlane(transform.right, Vector3.up);
            Vector3 moveDirection = forward * moveInput.y + right * moveInput.x;

            moveDirection *= speedModifier * (
                IsSprinting
                    ? CharacterSpeed * CharacterSprintSpeed
                    : CharacterSpeed
                );

            ApplyGravity();

            _ = CharacterController?.Move(moveDirection * Time.deltaTime);

            if (Keyboard.spaceKey.wasPressedThisFrame) Jump();

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

        public void ApplyGravity()
        {
            VelocityY = CharacterController is { isGrounded: false }
                ? VelocityY - Gravity * Time.deltaTime
                : 0.0f;

            Vector3 motion = new(0.0f, VelocityY, 0.0f);
            _ = CharacterController?.Move(motion * Time.deltaTime);
        }

        public void Jump() => VelocityY = JumpForce;
    }
}
