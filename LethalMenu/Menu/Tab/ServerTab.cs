
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
        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;

        public ServerTab() : base("ServerTab.Title") { }

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
            UI.Header("ServerTab.InfoDisplay");

            scrollPos2 = GUILayout.BeginScrollView(scrollPos2);

            UI.Hack(Hack.DisplayBodyCount, "ServerTab.DisplayBodyCount");
            UI.Hack(Hack.DisplayEnemyCount, "ServerTab.DisplayEnemyCount");
            UI.Hack(Hack.DisplayObjectCount, "ServerTab.DisplayObjectCount");
            UI.Hack(Hack.DisplayObjectValue, "ServerTab.DisplayObjectValue");
            UI.Hack(Hack.DisplayShipObjectCount, "ServerTab.DisplayShipObjectCount");
            UI.Hack(Hack.DisplayShipObjectValue, "ServerTab.DisplayShipObjectValue");
            UI.Hack(Hack.DisplayQuota, "ServerTab.DisplayQuota");
            UI.Hack(Hack.DisplayDeadline, "ServerTab.DisplayDeadline");
            UI.Hack(Hack.DisplayBuyingRate, "ServerTab.DisplayBuyingRate");

            GUILayout.EndScrollView();
        }

        private void ServerMenuContent()
        {
            UI.Header("ServerTab.ServerCheats");

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            UI.TextboxAction("ServerTab.EditCredits", ref s_credits, @"[^0-9]", 50, 
                new UIButton("General.Remove", () => Hack.ModifyCredits.Execute(int.Parse(s_credits), ActionType.Remove)),
                new UIButton("General.Add", () => Hack.ModifyCredits.Execute(int.Parse(s_credits), ActionType.Add)),
                new UIButton("General.Set", () => Hack.ModifyCredits.Execute(int.Parse(s_credits), ActionType.Set))
            );

            UI.TextboxAction(["ServerTab.EditQuota", "General.HostTag"], ref s_quota, @"[^0-9]", 50,
                new UIButton("General.Set", () => Hack.ModifyQuota.Execute(int.Parse(s_quota)))
            );

            UI.TextboxAction(["ServerTab.ScrapAmount", "General.HostTag"], ref s_scrapAmount, @"[^0-9]", 3,
                new UIButton("General.Set", () => Hack.ModifyScrap.Execute(int.Parse(s_scrapAmount), 0))
            );

            UI.TextboxAction(["ServerTab.ScrapValue", "General.HostTag"], ref s_scrapValue, @"[^0-9]", 3,
                new UIButton("General.Set", () => Hack.ModifyScrap.Execute(int.Parse(s_scrapValue), 1))
            );

            UI.Hack(Hack.StartGame, "ServerTab.ForceLand");
            UI.Hack(Hack.EndGame, "ServerTab.ForceLeave");
            UI.Hack(Hack.Disconnect, "ServerTab.Disconnect");
            UI.Hack(Hack.ShowOffensiveLobbyNames, "ServerTab.ShowOffensiveLobbyNames");
            UI.Hack(Hack.ScrapNeverDespawns, "ServerTab.ScrapNeverDespawns");
            UI.Hack(Hack.Shoplifter, "ServerTab.Shoplifter");
            UI.Hack(Hack.SpawnMoreScrap, ["ServerTab.SpawnScrap", "General.HostTag"]);
            UI.Hack(Hack.SpawnMapObjects, ["ServerTab.SpawnRandMines", "General.HostTag"], MapObject.Landmine);
            UI.Hack(Hack.SpawnMapObjects, ["ServerTab.SpawnRandTurrets", "General.HostTag"], MapObject.TurretContainer);
            UI.Hack(Hack.SpawnLandmine, ["ServerTab.SpawnLandmine", "General.HostTag"]);
            UI.Hack(Hack.SpawnTurret, ["ServerTab.SpawnTurret", "General.HostTag"]);
            UI.Hack(Hack.SpawnSpikeRoofTrap, ["ServerTab.SpawnSpikeRoofTrap", "General.HostTag"]);

            GUILayout.EndScrollView();
        }

        private void ManagersContent()
        {
            UI.Header("ServerTab.Managers");
            UI.Toggle("MoonManager.Title", ref HackMenu.Instance.MoonManagerWindow.isOpen, "General.Close", "General.Open");
            UI.Toggle("UnlockableManager.Title", ref HackMenu.Instance.UnlockableManagerWindow.isOpen, "General.Close", "General.Open");
            UI.Toggle("ItemManager.Title", ref HackMenu.Instance.ItemManagerWindow.isOpen, "General.Close", "General.Open");
            UI.Toggle("LootManager.Title", ref HackMenu.Instance.LootManagerWindow.isOpen, "General.Close", "General.Open");
            UI.Toggle("WeatherManager.Title", ref HackMenu.Instance.WeatherManagerWindow.isOpen, "General.Close", "General.Open");
        } 
    }
}
