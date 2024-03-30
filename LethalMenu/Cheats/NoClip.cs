using LethalMenu.Components;
using UnityEngine;

namespace LethalMenu.Cheats;

internal class NoClip : Cheat
{
    private KBInput movement;

    public override void Update()
    {
        if (!LethalMenu.localPlayer) return;

        Collider collider = LethalMenu.localPlayer.GetComponent<CharacterController>();

        if (Hack.NoClip.IsEnabled())
        {
            var player = GameNetworkManager.Instance.localPlayerController;

            if (movement == null) movement = player.gameObject.AddComponent<KBInput>();

            collider.enabled = false;

            player.transform.transform.position = movement.transform.position;


            //player.transform.position += vector3 * (Settings.f_noclipSpeed * Time.deltaTime);
        }
        else
        {
            collider.enabled = true;
            Destroy(movement);
            movement = null;
        }
    }
}