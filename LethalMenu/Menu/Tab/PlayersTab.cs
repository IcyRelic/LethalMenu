using GameNetcodeStuff;
using LethalMenu.Cheats;
using LethalMenu.Handler;
using LethalMenu.Language;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System;
using UnityEngine;

namespace LethalMenu.Menu.Tab
{
    internal class PlayersTab : MenuTab
    {
        public static int selectedPlayer = 0;
        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;
        public PlayersTab() : base("PlayerTab.Title") { }

        public override void Draw()
        {
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.3f - HackMenu.Instance.spaceFromLeft));
            PlayersList();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.7f - HackMenu.Instance.spaceFromLeft));
            scrollPos2 = GUILayout.BeginScrollView(scrollPos2);
            GeneralActions();
            PlayerActions();
            GUILayout.EndScrollView(); 
            GUILayout.EndVertical();
        }

        private void PlayersList()
        {
            float width = HackMenu.Instance.contentWidth * 0.3f - HackMenu.Instance.spaceFromLeft * 2;
            float height = HackMenu.Instance.contentHeight - 20;

            Rect rect = new Rect(0, 0, width, height);
            GUI.Box(rect, Localization.Localize("PlayerTab.PlayerList"));

            GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));

            GUILayout.Space(25);
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            foreach (PlayerControllerB player in LethalMenu.players)
            {
                if (player == null || !player.IsRealPlayer()) continue;

                if (selectedPlayer == -1) selectedPlayer = (int)player.playerClientId;

                if (selectedPlayer == (int)player.playerClientId) GUI.contentColor = Settings.c_playerESP.GetColor();

                name = player.playerUsername;

                if (LethalMenu.Instance.LMUsers.ContainsKey(player.playerSteamId.ToString()) && Settings.b_DisplayLMUsers)
                    name = $"[Lethal Menu {LethalMenu.Instance.LMUsers[player.playerSteamId.ToString()]}] {player.playerUsername}";

                if (GUILayout.Button(name, GUI.skin.label)) selectedPlayer = (int)player.playerClientId;

                GUI.contentColor = Settings.c_menuText.GetColor();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void GeneralActions()
        {
            UI.Header("General.GeneralActions");
            UI.Hack(Hack.DeathNotifications, "PlayerTab.DeathNotifications");
            UI.Hack(Hack.FreeCam, "PlayerTab.FreeCam");

            if (Hack.SpectatePlayer.IsEnabled())
                UI.Button("PlayerTab.StopSpectating", () => Hack.SpectatePlayer.SetToggle(false), "General.Stop");

            if (Hack.MiniCam.IsEnabled())
                UI.Button("PlayerTab.StopMiniCam", () => Hack.MiniCam.SetToggle(false), "General.Stop");

            UI.Button("PlayerTab.KillEveryone", () => LethalMenu.players.ForEach(p => Hack.KillPlayer.Execute(p)));
            UI.Button("PlayerTab.KillEveryoneElse", () => LethalMenu.players.FindAll(p => p.playerClientId != GameNetworkManager.Instance.localPlayerController.playerClientId).ForEach(p => Hack.KillPlayer.Execute(p)));
        }

        private void PlayerActions()
        {
            PlayerControllerB player = LethalMenu.players.Find(p => (int)p.playerClientId == selectedPlayer);

            if (player == null || !player.IsRealPlayer()) return;

            string name = player.playerUsername;

            if (player.isPlayerDead && player.deadBody != null)
                name = $"{Settings.c_deadPlayer.AsString("PlayerTab.DeadPrefix")} {name} ({Settings.c_causeOfDeath.AsString(player.deadBody.causeOfDeath.ToString())})";

            JetpackItem jetpackItem = null;
            if (player.currentlyHeldObjectServer is JetpackItem heldJetpack) jetpackItem = heldJetpack;

            UI.Header(name);
            UI.Header("PlayerTab.PlayerInfo");

            UI.Label("PlayerTab.SteamId", player.playerSteamId.ToString());
            UI.Label("PlayerTab.PlayerId", player.playerClientId.ToString());
            UI.Label("PlayerTab.PlayerStatus", player.isPlayerDead ? "PlayerTab.DeadPrefix" : "PlayerTab.AlivePrefix");
            UI.Label("PlayerTab.PlayerHealth", player.health.ToString());
            UI.Label("PlayerTab.Insanity", player.insanityLevel.ToString());
            UI.Label("PlayerTab.IsHost", player.IsHost().ToString());
            UI.Label("PlayerTab.IsInFactory", player.isInsideFactory.ToString());
            UI.Label("PlayerTab.IsInShip", player.isInHangarShipRoom.ToString());
            UI.Label("PlayerTab.UsingJetpack", (jetpackItem != null).ToString());

            UI.Header("PlayerTab.Inventory", true);
            foreach (GrabbableObject item in player.ItemSlots)
            {
                if (item == null) continue;
                UI.Label("", $"Name: {item.name} | Value: {item.scrapValue} | Weight: {item.itemProperties.weight} | Pocketed: {item.isPocketed} | Deactivated: {item.deactivated}");
            }

            UI.Header("General.GeneralActions", true);
            UI.Hack(Hack.Teleport, "PlayerTab.TeleportTo", player.transform.position, player.isInElevator, player.isInHangarShipRoom, player.isInsideFactory);
            if (player != LethalMenu.localPlayer)
            {
                UI.Hack(Hack.DemiGod, "PlayerTab.DemiGod", player);
                HackExtensions.SetToggle(Hack.DemiGod, DemiGodCheat.DemiGodPlayers.Contains(player));
            }
            UI.Hack(Hack.KillPlayer, "PlayerTab.Kill", player);
            UI.Hack(Hack.HealPlayer, "PlayerTab.Heal", player);
            UI.Hack(Hack.ExplodeJetpack, "PlayerTab.ExplodeJetpack", jetpackItem);
            UI.Hack(Hack.LightningStrikePlayer, ["PlayerTab.Strike", "General.HostStormyTag"], player);
            UI.Hack(Hack.SpiderWebPlayer, "PlayerTab.SpiderWeb", player);
            UI.Hack(Hack.TeleportAllEnemies, "PlayerTab.TeleportAllEnemies", player, LethalMenu.enemies.ToArray());
            UI.Hack(Hack.LureAllEnemies, "PlayerTab.Lure", player);
            UI.Hack(Hack.ExplodeClosestMine, "PlayerTab.ExplodeMine", player);

            if (player.playerClientId != GameNetworkManager.Instance.localPlayerController.playerClientId)
            {
                string btnText = SpectatePlayer.isSpectatingPlayer(player) ? "General.Stop" : "PlayerTab.Spectate";

                Action startAction = () =>
                {
                    Hack.SpectatePlayer.SetToggle(true);
                    Hack.SpectatePlayer.Invoke(player);
                };
                Action stopAction = () =>
                {
                    Hack.SpectatePlayer.SetToggle(false);
                };

                Action action = SpectatePlayer.isSpectatingPlayer(player) ? stopAction : startAction;

                UI.Button("PlayerTab.Spectate", action, btnText);

                btnText = (int)player.playerClientId == SpectatePlayer.camPlayer ? "General.Stop" : "General.View";

                startAction = () =>
                {
                    Hack.MiniCam.SetToggle(true);
                    Hack.MiniCam.Invoke(player);
                };
                stopAction = () =>
                {
                    Hack.MiniCam.SetToggle(false);
                };

                action = SpectatePlayer.isCamPlayer(player) ? stopAction : startAction;

                UI.Button("PlayerTab.MiniCam", action, btnText);
            }
        }
    }
}