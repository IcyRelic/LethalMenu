using System.Text.RegularExpressions;
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using UnityEngine;
using LethalMenu.Util;
using LethalMenu.Types;

namespace LethalMenu.Menu.Tab
{
    internal class ServerTab : MenuTab
    {
        private string s_credits = "";
        private string s_quota = "";
        private string s_scrapAmount = "1";
        private string s_scrapValue = "1";

        public ServerTab() : base("Server") { }

        public override void Draw()
        {
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
            ServerMenuContent();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
            ManagersContent();
            InfoMenuContent();
            GUILayout.EndVertical();
        }

        private void InfoMenuContent()
        {
            UI.Header("Info Display");
            UI.Hack(Hack.DisplayBodyCount, "Dead Body Count");
            UI.Hack(Hack.DisplayEnemyCount, "Enemy Count");
            UI.Hack(Hack.DisplayObjectScan, "Object Value Scan");
            UI.Hack(Hack.DisplayShipScan, "Ship Value Scan");
            UI.Hack(Hack.DisplayQuota, "Display Quota");
            UI.Hack(Hack.DisplayBuyingRate, "Display Buying Rate");
        }

        private void ServerMenuContent()
        {
            UI.Header("Server Cheats");

            UI.TextboxAction("Edit Credits", ref s_credits, @"[^0-9]", 50, 
                new UIButton("Remove", () => Hack.ModifyCredits.Execute(int.Parse(s_credits), ActionType.Remove)),
                new UIButton("Add", () => Hack.ModifyCredits.Execute(int.Parse(s_credits), ActionType.Add)),
                new UIButton("Set", () => Hack.ModifyCredits.Execute(int.Parse(s_credits), ActionType.Set))
            );

            UI.TextboxAction("Edit Quota (Host)", ref s_quota, @"[^0-9]", 50,
                new UIButton("Set", () => Hack.ModifyQuota.Execute(int.Parse(s_quota)))
            );

            UI.TextboxAction("Scrap Amount Multiplier (Host)", ref s_scrapAmount, @"[^0-9]", 3,
                new UIButton("Set", () => Hack.ModifyScrap.Execute(int.Parse(s_scrapAmount), 0))
            );

            UI.TextboxAction("Scrap Value Multiplier (Host)", ref s_scrapValue, @"[^0-9]", 3,
                new UIButton("Set", () => Hack.ModifyScrap.Execute(int.Parse(s_scrapValue), 1))
            );

            UI.Hack(Hack.StartGame, "Force Ship Land");
            UI.Hack(Hack.EndGame, "Force Ship Leave");
            UI.Hack(Hack.SpawnMoreScrap, "Spawn More Scrap (Host Only)");
            UI.Hack(Hack.SpawnMapObjects, "Spawn Random Mines (Host Only)", MapObject.Landmine);
            UI.Hack(Hack.SpawnMapObjects, "Spawn Random Turrets (Host Only)", MapObject.TurretContainer);
            UI.Hack(Hack.SpawnLandmine, "Spawn Landmine (Host Only)");
            UI.Hack(Hack.SpawnTurret, "Spawn Turret (Host Only)");

        }

        private void ManagersContent()
        {
            UI.Header("Managers");
            UI.Toggle("Enemy Manager", ref HackMenu.Instance.enemyManagerWindow.isOpen, "Close", "Open");
            UI.Toggle("Unlockables Manager", ref HackMenu.Instance.unlockableManagerWindow.isOpen, "Close", "Open"); ;
            UI.Toggle("Item Manager", ref HackMenu.Instance.itemManagerWindow.isOpen, "Close", "Open");

        }
    }
}
