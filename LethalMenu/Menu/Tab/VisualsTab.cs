using LethalMenu.Menu.Core;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Tab;

internal class VisualsTab : MenuTab
{
    private Vector2 scrollPos = Vector2.zero;

    public VisualsTab() : base("VisualsTab.Title")
    {
    }

    public override void Draw()
    {
        GUILayout.BeginVertical(
            GUILayout.Width(HackMenu.Instance.ContentWidth * 0.5f - HackMenu.SpaceFromLeft));
        ESPMenuContent();
        GUILayout.EndVertical();
        GUILayout.BeginVertical(
            GUILayout.Width(HackMenu.Instance.ContentWidth * 0.5f - HackMenu.SpaceFromLeft));
        OtherVisualsContent();
        GUILayout.EndVertical();
    }

    private void ESPMenuContent()
    {
        UI.Header("ESP");

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        UI.Hack(Hack.ToggleAllEsp, "VisualsTab.ToggleAll");
        UI.Toggle("VisualsTab.UseScrapTiers", ref Settings.b_useScrapTiers, "General.Disable", "General.Enable");
        UI.Toggle("VisualsTab.VCDisplay", ref Settings.b_VCDisplay, "General.Disable", "General.Enable");
        UI.Hack(Hack.ObjectEsp, "VisualsTab.ObjectESP");
        UI.Hack(Hack.EnemyEsp, "VisualsTab.EnemyESP");
        UI.Hack(Hack.PlayerEsp, "VisualsTab.PlayerESP");
        UI.Hack(Hack.DoorEsp, "VisualsTab.EntExtDoorsESP");
        UI.Hack(Hack.LandmineEsp, "VisualsTab.LandmineESP");
        UI.Hack(Hack.TurretEsp, "VisualsTab.TurretESP");
        UI.Hack(Hack.ShipEsp, "VisualsTab.ShipESP");
        UI.Hack(Hack.SteamHazardEsp, "VisualsTab.SteamHazardESP");
        UI.Hack(Hack.BigDoorEsp, "VisualsTab.BigDoorESP");
        UI.Hack(Hack.DoorLockEsp, "VisualsTab.LockedDoorESP");
        UI.Hack(Hack.BreakerEsp, "VisualsTab.BreakerESP");

        GUILayout.EndScrollView();
    }

    private void OtherVisualsContent()
    {
        UI.Header("VisualsTab.OtherVisuals");
        UI.Hack(Hack.AlwaysShowClock, "VisualsTab.ShowClock");
        UI.Hack(Hack.SimpleClock, "VisualsTab.SimpleClock");
        UI.Hack(Hack.Crosshair, "VisualsTab.Crosshair");
        UI.Hack(Hack.Breadcrumbs, "VisualsTab.Breadcrumbs");
        UI.Hack(Hack.NoFog, "VisualsTab.NoFog");
        UI.Hack(Hack.NoVisor, "VisualsTab.NoVisor");
        UI.Hack(Hack.NoFieldOfDepth, "VisualsTab.NoFieldOfDepth");
        UI.SliderAction("VisualsTab.FOV", Settings.f_fov.ToString("0"), ref Settings.f_fov, 10f, 180f, 66f);
    }
}