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
            UI.Hack(Hack.ToggleDepositDeskDoorSound, "TrollTab.ToggleDepositDeskDoorSound");
            UI.Hack(Hack.ToggleShipLights, "TrollTab.ShipLights");
            UI.Hack(Hack.ToggleFactoryLights, ["TrollTab.FactoryLights","General.HostTag"]);
            UI.Hack(Hack.ForceBridgeFall, ("TrollTab.ForceBridgeFall"));
            UI.Hack(Hack.ForceSmallBridgeFall, ("TrollTab.ForceSmallBridgeFall"));
            UI.Hack(Hack.BlowUpAllLandmines, "TrollTab.BlowAllMines");
            UI.Hack(Hack.ToggleAllLandmines, "TrollTab.ToggleMines");
            UI.Hack(Hack.ToggleAllTurrets, "TrollTab.ToggleTurrets");
            UI.Hack(Hack.BerserkAllTurrets, "TrollTab.BerserkAllTurrets");
            UI.Hack(Hack.OpenShipDoorSpace, "TrollTab.OpenShipDoorSpace");
            UI.Hack(Hack.ForceTentacleAttack, "TrollTab.TentacleAttack");
            UI.Hack(Hack.SpawnMaskedEnemy, "TrollTab.SpawnMasks");
            UI.Hack(Hack.EjectEveryone, ["TrollTab.EjectEveryone", "General.HostTag"]);
            UI.HackSlider(Hack.PJSpammer, "TrollTab.PJSpammer", Settings.f_pjSpamSpeed.ToString("0.00"), ref Settings.f_pjSpamSpeed, 0f, 1f);
            UI.Hack(Hack.SpawnHoardingBugInfestation, ["TrollTab.SpawnHoardingBugInfestation", "General.HostTag"]);
            UI.Hack(Hack.ToggleMineshaftElevator, "TrollTab.ToggleMineshaftElevator");
            UI.Hack(Hack.ToggleVehicleMagnet, "TrollTab.ToggleVehicleMagnet");
            UI.Hack(Hack.SpamShootAllShotguns, "TrollTab.SpamShootAllShotguns");
            UI.Hack(Hack.ShootAllShotguns, "TrollTab.ShootAllShotguns");
            UI.Hack(Hack.ExplodeCruiser, "TrollTab.ExplodeCruiser");

            GUILayout.EndScrollView();
        }
    }
}
