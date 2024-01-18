using GameNetcodeStuff;
using LethalMenu.Menu.Core;
using System;
using System.Linq;
using UnityEngine;
using LethalMenu.Util;
using LethalMenu.Language;

namespace LethalMenu.Menu.Tab
{
    internal class PlayersTab : MenuTab
    {
        public static int selectedPlayer = 0;
        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;
        public PlayersTab() : base(Localization.Localize("PlayerTab.Title")) { }

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
            GUI.Box(rect, GUIContent.none);

            GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            foreach (PlayerControllerB player in LethalMenu.players)
            {
                if (player.disconnectedMidGame) continue;

                if (selectedPlayer == -1) selectedPlayer = (int)player.playerClientId;

                if (selectedPlayer == (int)player.playerClientId) GUI.contentColor = Settings.c_playerESP.GetColor();

                if (GUILayout.Button(player.playerUsername, GUI.skin.label)) selectedPlayer = (int)player.playerClientId;

                GUI.contentColor = Settings.c_menuText.GetColor();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void GeneralActions()
        {
            UI.Header("General Actions");
            UI.Hack(Hack.DeathNotifications, "Death Notifications");
            UI.Hack(Hack.FreeCam, "Free Camera Mode");

            if(Hack.SpectatePlayer.IsEnabled())
                UI.Button("Stop Spectating", () => Hack.SpectatePlayer.SetToggle(false), "Stop");

            if (Hack.MiniCam.IsEnabled())
                UI.Button("Stop Mini Cam", () => Hack.MiniCam.SetToggle(false), "Stop");

            UI.Button("Kill Everyone", () => LethalMenu.players.ForEach(p => Hack.KillPlayer.Execute(p)));
            UI.Button("Kill Everyone Except You", () => LethalMenu.players.FindAll(p => p.playerClientId != GameNetworkManager.Instance.localPlayerController.playerClientId).ForEach(p => Hack.KillPlayer.Execute(p)));
           
        }

        private void PlayerActions()
        {

            PlayerControllerB player = LethalMenu.players.Find(p => (int)p.playerClientId == selectedPlayer);

            if (player == null || player.playerUsername.StartsWith("Player #") || player.disconnectedMidGame) return;


            string name = player.playerUsername;

            if (player.isPlayerDead && player.deadBody != null) name = "<color=red>[Dead]</color> " + name + Settings.c_causeOfDeath.AsString("(" + player.deadBody.causeOfDeath + ")");


            UI.Header(name);
            UI.Hack(Hack.Teleport, "Teleport To Them", player.transform.position, player.isInElevator, player.isInHangarShipRoom, player.isInsideFactory);
            UI.Hack(Hack.KillPlayer, "Kill Player", player);
            UI.Hack(Hack.HealPlayer, "Heal Player", player);
            UI.Hack(Hack.LightningStrikePlayer, "Lightning Strike (Host/Stormy)", player);
            UI.Hack(Hack.SpiderWebPlayer, "Spider Web (Requires Spider)", player);
            UI.Hack(Hack.TeleportEnemy, "Teleport All Enemies", player, LethalMenu.enemies.ToArray());
            UI.Hack(Hack.LureAllEnemies, "Lure All Enemies", player);
            UI.Hack(Hack.ExplodeClosestMine, "Explode Closest Landmine", player);


            if (player.playerClientId != GameNetworkManager.Instance.localPlayerController.playerClientId)
            {
                string btnText = Cheats.SpectatePlayer.isSpectatingPlayer(player) ? "Stop" : "Spectate";

                Action startAction = () =>
                {
                    Hack.SpectatePlayer.SetToggle(true);
                    Hack.SpectatePlayer.Invoke(player);
                };
                Action stopAction = () =>
                {
                    Hack.SpectatePlayer.SetToggle(false);
                };

                Action action = Cheats.SpectatePlayer.isSpectatingPlayer(player) ? stopAction : startAction;

                UI.Button("Spectate", action, btnText);


                btnText = (int)player.playerClientId == Cheats.SpectatePlayer.camPlayer ? "Stop" : "View";

                startAction = () =>
                {
                    Hack.MiniCam.SetToggle(true);
                    Hack.MiniCam.Invoke(player);
                };
                stopAction = () =>
                {
                    Hack.MiniCam.SetToggle(false);
                };

                action = Cheats.SpectatePlayer.isCamPlayer(player) ? stopAction : startAction;

                UI.Button("Mini Cam", action, btnText);
            }

        }
    }
}
