using GameNetcodeStuff;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

namespace LethalMenu.Cheats
{
    internal class NoClip : Cheat
    {

        public override void Update()
        {
            if(!LethalMenu.localPlayer) return;

            Collider collider = LethalMenu.localPlayer.GetComponent<CharacterController>();

            if (Hack.NoClip.IsEnabled())
            {
                PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;

              
                collider.enabled = false;

                Vector3 vector3 = new Vector3();
                if (Keyboard.current.wKey.isPressed) vector3 += player.transform.forward;
                if (Keyboard.current.sKey.isPressed) vector3 -= player.transform.forward;
                if (Keyboard.current.aKey.isPressed) vector3 -= player.transform.right;
                if (Keyboard.current.dKey.isPressed) vector3 += player.transform.right;
                if (Keyboard.current.spaceKey.isPressed) vector3 += player.transform.up;
                if (Keyboard.current.ctrlKey.isPressed) vector3 -= player.transform.up;

                if (vector3.Equals(Vector3.zero)) return;

                player.transform.position += vector3 * (Settings.f_noclipSpeed * Time.deltaTime);




            } 
            else collider.enabled = true;
        }

    }
}
