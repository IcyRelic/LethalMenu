
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using UnityEngine;
using LethalMenu.Util;
using LethalMenu.Types;
using System.Linq;
using System.Collections.Generic;
using Steamworks;

namespace LethalMenu.Menu.Tab
{
    internal class ServerTab : MenuTab
    {
        private string s_credits = "";
        private string s_quota = "";
        private string s_scrapAmount = "1";
        private string s_scrapValue = "1";
        public static string s_message = "";
        public static int i_messageindex = 0;
        private string s_joinlobbyid = "";
        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;

        public static List<UIOption> options = new List<UIOption>
        {
            new UIOption("ServerTab.System", () => Hack.Message.Execute(s_message, 0, -1)),
            new UIOption("ServerTab.Server", () => Hack.Message.Execute(s_message, 1, -1)),
            new UIOption("ServerTab.Broadcast", () => Hack.Message.Execute(s_message, 2, -1)),
            new UIOption("ServerTab.SignalTranslator", () => Hack.Message.Execute(s_message, 3, -1))
        };

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

            UI.Hack(Hack.ToggleAllDisplays, "ServerTab.ToggleAll");
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

            UI.Select("ServerTab.Message", ref i_messageindex, ref s_message, @"", 50, options.ToArray());

            options.AddRange(LethalMenu.players.Select(player =>
                new UIOption(player.playerUsername, () => Hack.Message.Execute(s_message, 4, (int)player.actualClientId)))
            );

            UI.Hack(Hack.StartGame, "ServerTab.ForceLand");
            UI.Hack(Hack.EndGame, "ServerTab.ForceLeave");

            string lobbyid = Settings.s_lobbyid.ToString();

            UI.TextboxAction("ServerTab.CopyLobbyID", ref lobbyid, @"^$", 20,
                new UIButton("General.Execute", () => GUIUtility.systemCopyBuffer = lobbyid)
            );

            UI.TextboxAction("ServerTab.JoinLobby", ref s_joinlobbyid, @"", 20,
                new UIButton("General.Execute", () => { if (ulong.TryParse(s_joinlobbyid, out var id)) Hack.JoinLobby.Execute(new SteamId { Value = id });
            }));

            UI.Hack(Hack.Disconnect, "ServerTab.Disconnect");
            UI.Hack(Hack.ShowOffensiveLobbyNames, "ServerTab.ShowOffensiveLobbyNames");
            UI.Hack(Hack.NeverLoseScrap, "ServerTab.NeverLoseScrap");
            UI.Hack(Hack.Shoplifter, "ServerTab.Shoplifter");
            UI.Hack(Hack.SpawnMoreScrap, ["ServerTab.SpawnScrap", "General.HostTag"]);
            UI.Hack(Hack.SpawnMapObjects, ["ServerTab.SpawnRandomMines", "General.HostTag"], MapObject.Landmine);
            UI.Hack(Hack.SpawnMapObjects, ["ServerTab.SpawnRandomTurrets", "General.HostTag"], MapObject.TurretContainer);
            UI.Hack(Hack.SpawnLandmine, ["ServerTab.SpawnLandmine", "General.HostTag"]);
            UI.Hack(Hack.SpawnTurret, ["ServerTab.SpawnTurret", "General.HostTag"]);
            UI.Hack(Hack.SpawnSpikeRoofTrap, ["ServerTab.SpawnSpikeRoofTrap", "General.HostTag"]);
            UI.Hack(Hack.ResetShip, ["ServerTab.ResetShip", "General.HostTag"]);

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
            UI.Toggle("SuitManager.Title", ref HackMenu.Instance.SuitManagerWindow.isOpen, "General.Close", "General.Open");
        }

        public static void ClearPlayerOptions()
        {
            i_messageindex = 0;
            options = options.Take(4).ToList();
        }
    }
}
