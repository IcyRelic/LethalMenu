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
        private List<Collider> colliders = new List<Collider>();

        public override void Update()
        {
           

            if (Hack.NoClip.IsEnabled())
            {
                PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;

                Object.FindObjectsOfType<Collider>().ToList().ForEach(c => DisableCollider(c));
                DisableCollider(player.GetComponent<Collider>());


                Vector3 vector3 = new Vector3();
                if (Keyboard.current.wKey.isPressed) vector3 += player.transform.forward;
                if (Keyboard.current.sKey.isPressed) vector3 -= player.transform.forward;
                if (Keyboard.current.aKey.isPressed) vector3 -= player.transform.right;
                if (Keyboard.current.dKey.isPressed) vector3 += player.transform.right;
                if (Keyboard.current.spaceKey.isPressed) vector3 += player.transform.up;
                if (Keyboard.current.ctrlKey.isPressed) vector3 -= player.transform.up;

                //vector3.Normalize();

                Vector3 localPosition = player.transform.localPosition;
                if (vector3.Equals(Vector3.zero)) return;

                Vector3 vector3_2 = localPosition + vector3 * (Settings.f_noclipSpeed * Time.deltaTime);
                player.transform.localPosition = vector3_2;



            } 
            else
            {
                colliders.FindAll(c => c != null).ForEach(c => c.enabled = true);
                colliders.Clear();
                //EnableCollider(player.GetComponent<Collider>());

            }
        }

        private void DisableCollider(Collider collider)
        {
            if (collider == null || colliders.Contains(collider)) return;

            collider.enabled = false;
            colliders.Add(collider);
        }

    }
}
