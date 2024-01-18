using LethalMenu.Language;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Tab
{
    internal class VisualsTab : MenuTab
    {
        private Vector2 scrollPos = Vector2.zero;

        public VisualsTab() : base(Localization.Localize("VisualsTab.Title")) { }

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

            UI.Hack(Hack.ToggleAllESP, "Toggle All");
            UI.Toggle("Use Scrap Tiers", ref Settings.b_useScrapTiers);
            UI.Toggle("VC Display", ref Settings.b_VCDisplay);
            UI.Hack(Hack.ObjectESP, "Object ESP");
            UI.Hack(Hack.EnemyESP, "Enemy ESP");
            UI.Hack(Hack.PlayerESP, "Player ESP");
            UI.Hack(Hack.DoorESP, "Enterance/Exit ESP");
            UI.Hack(Hack.LandmineESP, "Landmine ESP");
            UI.Hack(Hack.TurretESP, "Turret ESP");
            UI.Hack(Hack.ShipESP, "Ship ESP");
            UI.Hack(Hack.SteamHazardESP, "Steam Hazard ESP");
            UI.Hack(Hack.BigDoorESP, "Big Door ESP");
            UI.Hack(Hack.DoorLockESP, "Locked Door ESP");
            UI.Hack(Hack.BreakerESP, "Breaker Box ESP");

            GUILayout.EndScrollView();
        }

        private void OtherVisualsContent()
        {
            UI.Header("Other Visuals");
            UI.Hack(Hack.AlwaysShowClock, "Always Show Clock");
            UI.Hack(Hack.SimpleClock, "Simple Clock");
            UI.Hack(Hack.Crosshair, "Crosshair");
            UI.Hack(Hack.Breadcrumbs, "Breadcrumbs");
            UI.Hack(Hack.NoFog, "No Fog");
        }
    }
}
