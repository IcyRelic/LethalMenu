namespace LethalMenu.Cheats;

internal class FOV : Cheat
{
    public override void Update()
    {
        if (!LethalMenu.LocalPlayer) return;

        LethalMenu.LocalPlayer.gameplayCamera.fieldOfView = Settings.f_fov;

        if (LethalMenu.LocalPlayer.inTerminalMenu)
            LethalMenu.LocalPlayer.gameplayCamera.fieldOfView = 66f;
    }
}