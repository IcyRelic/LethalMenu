using LethalMenu.Language;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Tab
{
    internal class VisualsTab : MenuTab
    {
        private Vector2 scrollPos = Vector2.zero;
        public VisualsTab() : base("VisualsTab.Title") { }

        public override void Draw()
        {
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
            ESPMenuContent();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
            OtherVisualsContent();
            GUILayout.EndVertical();
        }

        private void ESPMenuContent()
        {
            UI.Header("ESP");

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            UI.Hack(Hack.ToggleAllESP, "VisualsTab.ToggleAll");
            UI.Toggle("VisualsTab.UseScrapTiers", ref Settings.b_useScrapTiers, "General.Disable", "General.Enable");
            UI.Toggle("VisualsTab.VCDisplay", ref Settings.b_VCDisplay, "General.Disable", "General.Enable");
            UI.Toggle("VisualsTab.PlayerHPDisplay", ref Settings.b_PlayerHPDisplay, "General.Disable", "General.Enable");
            UI.Hack(Hack.ObjectESP, "VisualsTab.ObjectESP");
            UI.Hack(Hack.EnemyESP, "VisualsTab.EnemyESP");
            UI.Hack(Hack.PlayerESP, "VisualsTab.PlayerESP");
            UI.Hack(Hack.BodyESP, "VisualsTab.BodyESP");
            UI.Hack(Hack.DoorESP, "VisualsTab.EntExtDoorsESP");
            UI.Hack(Hack.LandmineESP, "VisualsTab.LandmineESP");
            UI.Hack(Hack.TurretESP, "VisualsTab.TurretESP");
            UI.Hack(Hack.ShipESP, "VisualsTab.ShipESP");
            UI.Hack(Hack.SteamHazardESP, "VisualsTab.SteamHazardESP");
            UI.Hack(Hack.BigDoorESP, "VisualsTab.BigDoorESP");
            UI.Hack(Hack.DoorLockESP, "VisualsTab.LockedDoorESP");
            UI.Hack(Hack.BreakerESP, "VisualsTab.BreakerESP");
            UI.Hack(Hack.SpikeRoofTrapESP, "VisualsTab.SpikeRoofTrapESP");
            UI.Hack(Hack.MineshaftElevatorESP, "VisualsTab.MineshaftElevatorESP");
            UI.Hack(Hack.EnemyVentESP, "VisualsTab.EnemyVentESP");
            UI.Hack(Hack.VainShroudESP, "VisualsTab.VainShroudESP");
            UI.Hack(Hack.CruiserESP, "VisualsTab.CruiserESP");
            UI.Hack(Hack.ItemDropShipESP, "VisualsTab.ItemDropShipESP");

            GUILayout.EndScrollView();
        }

        private void OtherVisualsContent()
        {
            UI.Header("VisualsTab.OtherVisuals");
            UI.Hack(Hack.AlwaysShowClock, "VisualsTab.ShowClock");
            UI.Hack(Hack.SimpleClock, "VisualsTab.SimpleClock");
            UI.Hack(Hack.Crosshair, "VisualsTab.Crosshair");
            UI.Hack(Hack.Breadcrumbs, "VisualsTab.Breadcrumbs");
            UI.Hack(Hack.HPDisplay, "VisualsTab.HPDisplay");
            UI.Hack(Hack.NoFog, "VisualsTab.NoFog");
            UI.Hack(Hack.NoVisor, "VisualsTab.NoVisor");
            UI.Hack(Hack.NoFieldOfDepth, "VisualsTab.NoFieldOfDepth");
            UI.HackSlider(Hack.FOV, "VisualsTab.FOV", Settings.f_fov.ToString("0.0"), ref Settings.f_fov, 10f, 180f);
        }
    }
}
