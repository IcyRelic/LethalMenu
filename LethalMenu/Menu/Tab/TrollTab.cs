using LethalMenu.Menu.Core;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Tab
{
    internal class TrollTab : MenuTab
    {

        private Vector2 scrollPos = Vector2.zero;
        public TrollTab() : base("Troll") { }

        public override void Draw()
        {
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
            TrollMenuContent();
            GUILayout.EndVertical();
        }

        private void TrollMenuContent()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            
            UI.Hack(Hack.ToggleShipHorn, "Toggle Ship Horn");
            UI.Hack(Hack.ToggleShipLights, "Toggle Ship Lights");
            UI.Hack(Hack.ToggleFactoryLights, "Toggle Factory Lights (Host)");
            UI.Hack(Hack.FlickerLights, "Flicker Factory Lights (Host)");
            UI.Hack(Hack.ForceBridgeFall, "Make Bridge Fall (Host, Vow)");
            UI.Hack(Hack.BlowUpAllLandmines, "Blow Up All Landmines");
            UI.Hack(Hack.ToggleAllLandmines, "Toggle All Landmines");
            UI.Hack(Hack.ToggleAllTurrets, "Toggle All Turrets");
            UI.Hack(Hack.TeleportAllItems, "Teleport All Items (Client Only)");
            UI.Hack(Hack.ForceTentacleAttack, "Force Tentacle Attack");
            UI.Hack(Hack.FixAllValves, "Fix All Steam Valves");
            UI.Hack(Hack.SpawnMaskedEnemy, "Turn All Masks Into Mimics");

            GUILayout.EndScrollView();

        }

    }
}
