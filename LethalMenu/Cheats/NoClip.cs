using LethalMenu.Components;
using UnityEngine;

namespace LethalMenu.Cheats;

internal class NoClip : Cheat
{
    private KbInput movement;

    public override void Update()
    {
        if (!LethalMenu.LocalPlayer) return;

        Collider collider = LethalMenu.LocalPlayer.GetComponent<CharacterController>();

        if (Hack.NoClip.IsEnabled())
        {
            var player = GameNetworkManager.Instance.localPlayerController;

            if (!movement) movement = player.gameObject.AddComponent<KbInput>();

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