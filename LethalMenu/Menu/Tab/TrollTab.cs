using LethalMenu.Menu.Core;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Tab
{
    internal class TrollTab : MenuTab
    {
        private Vector2 scrollPos = Vector2.zero;
        public TrollTab() : base("TrollTab.Title") { }

        public override void Draw()
        {
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
            TrollMenuContent();
            GUILayout.EndVertical();
        }

        private void TrollMenuContent()
        {
            UI.Header("TrollTab.Title");

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            
            UI.Hack(Hack.ToggleShipHorn, "TrollTab.ShipHorn");
            UI.Hack(Hack.ToggleCarHorn, "TrollTab.CarHorn");
            UI.Hack(Hack.ToggleTerminalSound, "TrollTab.TerminalSound");
            UI.Hack(Hack.ToggleShipLights, "TrollTab.ShipLights");
            UI.Hack(Hack.ToggleFactoryLights, ["TrollTab.FactoryLights","General.HostTag"]);
            UI.Hack(Hack.FlickerLights, ["TrollTab.FlickerFactoryLights", "General.HostTag"]);
            UI.Hack(Hack.ForceBridgeFall, ("TrollTab.ForceBridgeFall"));
            UI.Hack(Hack.ForceSmallBridgeFall, ("TrollTab.ForceSmallBridgeFall"));
            UI.Hack(Hack.BlowUpAllLandmines, "TrollTab.BlowAllMines");
            UI.Hack(Hack.ToggleAllLandmines, "TrollTab.ToggleMines");
            UI.Hack(Hack.ToggleAllTurrets, "TrollTab.ToggleTurrets");
            UI.Hack(Hack.BerserkAllTurrets, "TrollTab.BerserkAllTurrets");
            UI.Hack(Hack.OpenShipDoorSpace, "TrollTab.OpenShipDoorSpace");
            UI.Hack(Hack.ForceTentacleAttack, "TrollTab.TentacleAttack");
            UI.Hack(Hack.FixAllValves, "TrollTab.FixValves");
            UI.Hack(Hack.SpawnMaskedEnemy, "TrollTab.SpawnMasks");
            UI.Hack(Hack.SellEverything, "TrollTab.SellEverything");
            UI.Hack(Hack.TeleportAllItems, "TrollTab.TeleportAllItems");
            UI.Hack(Hack.TeleportOneItem, "TrollTab.TeleportOneItem");
            UI.Hack(Hack.EjectEveryone, ["TrollTab.EjectEveryone", "General.HostTag"]);
            UI.Hack(Hack.DeleteTerminal, "TrollTab.DeleteTerminal");

            GUILayout.EndScrollView();
        }
    }
}
