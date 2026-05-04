using GameNetcodeStuff;
using LethalMenu.Components;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class NoClip : Cheat
    {
        private KBInput? movement;
        private bool noclipJustEnabled;

        public override void Update()
        {
            PlayerControllerB? localPlayer = LethalMenu.localPlayer;
            if (localPlayer == null) return;
            CharacterController collider = localPlayer.GetComponent<CharacterController>();
            if (Hack.NoClip.IsEnabled())
            {
                noclipJustEnabled = true;
                if (movement == null) movement = localPlayer.gameObject.AddComponent<KBInput>();
                collider.enabled = false;
                localPlayer.transform.position = movement.transform.position;
            }
            else
            {
                collider.enabled = true;
                if (movement != null)
                {
                    Object.Destroy(movement);
                    movement = null;
                }
                if (noclipJustEnabled)
                {
                    noclipJustEnabled = false;
                    localPlayer.ResetFallGravity();
                }
            }
        }
    }
}
