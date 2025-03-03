using GameNetcodeStuff;
using LethalMenu.Handler;
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LethalMenu.Menu.Tab
{
    internal class ServerTab : MenuTab
    {
        private string s_credits = "";
        private string s_quota = "";
        private string s_scrapAmount = "1";
        private string s_scrapValue = "1";
        private string s_deadlineValue = "3";
        public static string s_message = "Lethal Menu on top!";
        public static int i_messageindex = 0;
        private static int id = -1;
        private string s_joinlobbyid = "";
        private bool MessageSpam = false;
        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;

        private static List<UIOption> options = new List<UIOption>
        {
            new UIOption("ServerTab.System", () => MessageExecute(0, -1)),
            new UIOption("ServerTab.Server", () => MessageExecute(1, -1)),
            new UIOption("ServerTab.Broadcast", () => MessageExecute(2, -1)),
            new UIOption("ServerTab.SignalTranslator", () => MessageExecute(3, -1))
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
            UI.Hack(Hack.DisplayBodies, "ServerTab.DisplayBodyCount");
            UI.Hack(Hack.DisplayEnemies, "ServerTab.DisplayEnemyCount");
            UI.Hack(Hack.DisplayMapObjects, "ServerTab.DisplayMapObjects");
            UI.Hack(Hack.DisplayShipObjects, "ServerTab.DisplayShipObjects");
            UI.Hack(Hack.DisplayQuota, "ServerTab.DisplayQuota");
            UI.Hack(Hack.DisplayDeadline, "ServerTab.DisplayDeadline");
            UI.Hack(Hack.DisplayCredits, "ServerTab.DisplayCredits");

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

            UI.TextboxAction(["ServerTab.EditDeadline", "General.HostTag"], ref s_deadlineValue, @"[^0-9]", 3,
                new UIButton("General.Set", () => Hack.ModifyDeadline.Execute(int.Parse(s_deadlineValue)))
            );

            UI.TextboxAction(["ServerTab.ScrapAmount", "General.HostTag"], ref s_scrapAmount, @"[^0-9]", 3,
                new UIButton("General.Set", () => Hack.ModifyScrap.Execute(int.Parse(s_scrapAmount), 0))
            );

            UI.TextboxAction(["ServerTab.ScrapValue", "General.HostTag"], ref s_scrapValue, @"[^0-9]", 3,
                new UIButton("General.Set", () => Hack.ModifyScrap.Execute(int.Parse(s_scrapValue), 1))
            );

            UpdatePlayerOptions();

            UI.Select("ServerTab.Message", ref i_messageindex, ref s_message, @"", 50, 2, options.ToArray());

            UI.ToggleAction("ServerTab.MessageSpam", ref MessageSpam, "General.Enable", "General.Disable", () => LethalMenu.Instance.StartCoroutine(StartSpamming()));

            UI.Hack(Hack.StartGame, "ServerTab.ForceLand");
            UI.Hack(Hack.EndGame, "ServerTab.ForceLeave");

            string lobbyid = Settings.s_lobbyid.ToString();

            UI.TextboxAction("ServerTab.CopyLobbyID", ref lobbyid, @"^$", 20,
                new UIButton("General.Execute", () => GUIUtility.systemCopyBuffer = lobbyid)
            );

            UI.TextboxAction("ServerTab.JoinLobby", ref s_joinlobbyid, @"[^0-9]", 20,
                new UIButton("General.Execute", () => { if (ulong.TryParse(s_joinlobbyid, out var id)) Hack.JoinLobby.Execute(new SteamId { Value = id });
            }));

            UI.Hack(Hack.Disconnect, "ServerTab.Disconnect");
            UI.Hack(Hack.ShowOffensiveLobbyNames, "ServerTab.ShowOffensiveLobbyNames");
            UI.Hack(Hack.Shoplifter, "ServerTab.Shoplifter");
            UI.Hack(Hack.SpawnMoreScrap, ["ServerTab.SpawnScrap", "General.HostTag"]);
            UI.Hack(Hack.SpawnMapObjects, ["ServerTab.SpawnRandomMines", "General.HostTag"], MapObject.Landmine);
            UI.Hack(Hack.SpawnMapObjects, ["ServerTab.SpawnRandomTurrets", "General.HostTag"], MapObject.TurretContainer);
            UI.Hack(Hack.SpawnLandmine, ["ServerTab.SpawnLandmine", "General.HostTag"]);
            UI.Hack(Hack.SpawnTurret, ["ServerTab.SpawnTurret", "General.HostTag"]);
            UI.Hack(Hack.SpawnSpikeRoofTrap, ["ServerTab.SpawnSpikeRoofTrap", "General.HostTag"]);
            UI.Hack(Hack.ResetShip, ["ServerTab.ResetShip", "General.HostTag"]);
            UI.Hack(Hack.ForceMeteorShower, ["ServerTab.ForceMeteorShower", "General.HostTag"]);
            UI.Hack(Hack.ClearMeteorShower, ["ServerTab.ClearMeteorShower", "General.HostTag"]);

            GUILayout.EndScrollView();
        }

        private void ManagersContent()
        {
            UI.Header("ServerTab.Managers");
            UI.Toggle("MoonManager.Title", ref HackMenu.Instance.MoonManagerWindow.isOpen, "General.Open", "General.Close");
            UI.Toggle("UnlockableManager.Title", ref HackMenu.Instance.UnlockableManagerWindow.isOpen, "General.Open", "General.Close");
            UI.Toggle("ItemManager.Title", ref HackMenu.Instance.ItemManagerWindow.isOpen, "General.Open", "General.Close");
            UI.Toggle("LootManager.Title", ref HackMenu.Instance.LootManagerWindow.isOpen, "General.Open", "General.Close");
            UI.Toggle("WeatherManager.Title", ref HackMenu.Instance.WeatherManagerWindow.isOpen, "General.Open", "General.Close");
            UI.Toggle("SuitManager.Title", ref HackMenu.Instance.SuitManagerWindow.isOpen, "General.Open", "General.Close");
        }

        private static void MessageExecute(int Type, int ID)
        {
            id = ID;  
            Hack.Message.Execute(s_message, Type, ID);
        }

        public static void UpdatePlayerOptions(bool clear = false)
        {
            List<PlayerControllerB> players = LethalMenu.players.Where(p => p != null && p.IsRealPlayer()).ToList();
            options = options.Take(4).ToList();
            if (clear)
            {
                i_messageindex = 0;
                return;
            }
            options.AddRange(players.Select(p =>
                new UIOption(p.playerUsername, () => MessageExecute(4, (int)p.actualClientId)))
            );
        }

        private IEnumerator StartSpamming()
        {
            while (MessageSpam)
            {
                Hack.Message.Execute(s_message, i_messageindex, id);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
