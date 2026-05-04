using GameNetcodeStuff;
using UnityEngine.InputSystem;

namespace LethalMenu.Cheats
{
    internal class MinigunShotgun : Cheat
    {
        public override void Update()
        {
            PlayerControllerB? localPlayer = LethalMenu.localPlayer;
            if (!Hack.MinigunShotgun.IsEnabled() || localPlayer == null) return;
            ShotgunItem? shotgun = localPlayer.currentlyHeldObjectServer as ShotgunItem;
            if (shotgun == null || !Mouse.current.leftButton.isPressed) return;
            shotgun.ShootGunServerRpc(localPlayer.transform.position - localPlayer.gameplayCamera.transform.up * 0.45f, localPlayer.gameplayCamera.transform.forward);
        }
    }
}
