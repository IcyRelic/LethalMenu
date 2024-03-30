namespace LethalMenu.Cheats;

internal class FOV : Cheat
{
    public override void Update()
    {
        if (!LethalMenu.localPlayer) return;

        LethalMenu.localPlayer.gameplayCamera.fieldOfView = Settings.f_fov;

        if (LethalMenu.localPlayer.inTerminalMenu)
            LethalMenu.localPlayer.gameplayCamera.fieldOfView = 66f;
    }
}