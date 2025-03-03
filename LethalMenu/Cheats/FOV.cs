namespace LethalMenu.Cheats
{
    internal class FOV : Cheat
    {
        public override void Update()
        {
            if (LethalMenu.localPlayer == null || LethalMenu.localPlayer.gameplayCamera == null) return;
            if (LethalMenu.localPlayer.inTerminalMenu) LethalMenu.localPlayer.gameplayCamera.fieldOfView = 66f;
            if (!Hack.FOV.IsEnabled() && LethalMenu.localPlayer.gameplayCamera.fieldOfView != 66f) LethalMenu.localPlayer.gameplayCamera.fieldOfView = 66f;
            if (Hack.FOV.IsEnabled() && !LethalMenu.localPlayer.inTerminalMenu && LethalMenu.localPlayer.gameplayCamera.fieldOfView != Settings.f_fov) LethalMenu.localPlayer.gameplayCamera.fieldOfView = Settings.f_fov;
        }
    }
}
