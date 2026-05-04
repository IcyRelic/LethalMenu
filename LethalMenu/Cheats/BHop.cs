using GameNetcodeStuff;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class BHop : Cheat
    {
        private bool isInAir;
        private Vector3 airVelocity;
        private Vector3 lastForwardDirection;
        private float jumpTimer;

        public override void Update()
        {
            PlayerControllerB? localPlayer = LethalMenu.localPlayer;
            if (localPlayer == null) return;
            if (!Hack.BHop.IsEnabled())
            {
                isInAir = false;
                jumpTimer = 0f;
                airVelocity = Vector3.Lerp(airVelocity, Vector3.zero, Time.deltaTime * 4.2f);
                lastForwardDirection = localPlayer.transform.forward;
                return;
            }
            if (localPlayer.playerBodyAnimator.GetBool("Jumping") && jumpTimer < 0.1f)
            {
                localPlayer.fallValue = localPlayer.jumpForce;
                jumpTimer += Time.deltaTime * 10f;
            }
            if (!localPlayer.thisController.isGrounded && !localPlayer.isClimbingLadder)
            {
                if (!isInAir)
                {
                    isInAir = true;
                    Vector3 velocity = localPlayer.thisController.velocity;
                    velocity.y = 0f;
                    airVelocity += velocity * 0.006f;
                }
                airVelocity.y = 0f;
                localPlayer.thisController.Move(localPlayer.transform.forward * airVelocity.magnitude);
                Vector3 forward = localPlayer.transform.forward;
                if ((forward - lastForwardDirection).magnitude > 0.01f) airVelocity += Vector3.one * 0.0005f;
                lastForwardDirection = forward;
            }
            else
            {
                airVelocity = Vector3.Lerp(airVelocity, Vector3.zero, Time.deltaTime * 4.2f);
                isInAir = false;
                jumpTimer = 0f;
            }     
        }
    }
}