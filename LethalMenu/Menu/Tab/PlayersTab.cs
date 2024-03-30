using LethalMenu.Cheats;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Tab;

internal class PlayersTab : MenuTab
{
    public static int selectedPlayer;
    private Vector2 scrollPos = Vector2.zero;
    private Vector2 scrollPos2 = Vector2.zero;

    public PlayersTab() : base("PlayerTab.Title")
    {
    }

    public override void Draw()
    {
        GUILayout.BeginVertical(
            GUILayout.Width(HackMenu.Instance.ContentWidth * 0.3f - HackMenu.SpaceFromLeft));
        PlayersList();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(
            GUILayout.Width(HackMenu.Instance.ContentWidth * 0.7f - HackMenu.SpaceFromLeft));
        scrollPos2 = GUILayout.BeginScrollView(scrollPos2);
        GeneralActions();
        PlayerActions();
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void PlayersList()
    {
        var width = HackMenu.Instance.ContentWidth * 0.3f - HackMenu.SpaceFromLeft * 2;
        var height = HackMenu.Instance.ContentHeight - 20;

        var rect = new Rect(0, 0, width, height);
        GUI.Box(rect, "Player List");

        GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));

        GUILayout.Space(25);
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        foreach (var player in LethalMenu.players)
        {
            if (player.disconnectedMidGame || !player.IsSpawned) continue;

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
        UI.Header("General.GeneralActions");
        UI.Hack(Hack.DeathNotifications, "PlayerTab.DeathNotifications");
        UI.Hack(Hack.FreeCam, "PlayerTab.FreeCam");

        if (Hack.SpectatePlayer.IsEnabled())
            UI.Button("PlayerTab.StopSpectating", () => Hack.SpectatePlayer.SetToggle(false), "General.Stop");

        if (Hack.MiniCam.IsEnabled())
            UI.Button("PlayerTab.StopMiniCam", () => Hack.MiniCam.SetToggle(false), "General.Stop");

        UI.Button("PlayerTab.KillEveryone", () => LethalMenu.players.ForEach(p => Hack.KillPlayer.Execute(p)));
        UI.Button("PlayerTab.KillEveryoneElse",
            () => LethalMenu.players
                .FindAll(p => p.playerClientId != GameNetworkManager.Instance.localPlayerController.playerClientId)
                .ForEach(p => Hack.KillPlayer.Execute(p)));
    }

    private void PlayerActions()
    {
        var player = LethalMenu.players.Find(p => (int)p.playerClientId == selectedPlayer);

        if (player == null || player.playerUsername.StartsWith("Player #") || player.disconnectedMidGame) return;

        var name = player.playerUsername;

        if (player.isPlayerDead && player.deadBody != null)
            name =
                $"{Settings.c_deadPlayer.AsString("PlayerTab.DeadPrefix")} {name} ({Settings.c_causeOfDeath.AsString(player.deadBody.causeOfDeath.ToString())})";


        UI.Header(name);
        UI.Header("PlayerTab.PlayerInfo");

        UI.Label("PlayerTab.SteamId", player.playerSteamId.ToString());
        UI.Label("PlayerTab.PlayerId", player.playerClientId.ToString());
        UI.Label("PlayerTab.PlayerStatus", player.isPlayerDead ? "PlayerTab.DeadPrefix" : "PlayerTab.AlivePrefix");
        UI.Label("PlayerTab.PlayerHealth", player.health.ToString());
        UI.Label("PlayerTab.IsInFactory", player.isInsideFactory.ToString());
        UI.Label("PlayerTab.IsInShip", player.isInHangarShipRoom.ToString());
        UI.Label("PlayerTab.Insanity", player.insanityLevel.ToString());

        //get the inventory of the player
        var items = player.ItemSlots;
        //show the inventory
        UI.Header("PlayerTab.Inventory", true);
        foreach (var item in items)
        {
            if (item == null) continue;

            UI.Label("", item.name);
        }


        UI.Header("General.GeneralActions", true);
        UI.Hack(Hack.Teleport, "PlayerTab.TeleportTo", player.transform.position, player.isInElevator,
            player.isInHangarShipRoom, player.isInsideFactory);
        UI.Hack(Hack.KillPlayer, "PlayerTab.Kill", player);
        UI.Hack(Hack.HealPlayer, "PlayerTab.Heal", player);
        UI.Hack(Hack.LightningStrikePlayer, ["PlayerTab.Strike", "General.HostStormyTag"], player);
        UI.Hack(Hack.SpiderWebPlayer, "PlayerTab.SpiderWeb", player);
        UI.Hack(Hack.TeleportEnemy, "PlayerTab.TeleportAllEnemies", player, LethalMenu.enemies.ToArray());
        UI.Hack(Hack.LureAllEnemies, "PlayerTab.Lure", player);
        UI.Hack(Hack.ExplodeClosestMine, "PlayerTab.ExplodeMine", player);


        if (player.playerClientId != GameNetworkManager.Instance.localPlayerController.playerClientId)
        {
            var btnText = SpectatePlayer.isSpectatingPlayer(player) ? "General.Stop" : "PlayerTab.Spectate";

            var startAction = () =>
            {
                Hack.SpectatePlayer.SetToggle(true);
                Hack.SpectatePlayer.Invoke(player);
            };
            var stopAction = () => { Hack.SpectatePlayer.SetToggle(false); };

            var action = SpectatePlayer.isSpectatingPlayer(player) ? stopAction : startAction;

            UI.Button("PlayerTab.Spectate", action, btnText);


            btnText = (int)player.playerClientId == SpectatePlayer.camPlayer ? "General.Stop" : "General.View";

            startAction = () =>
            {
                Hack.MiniCam.SetToggle(true);
                Hack.MiniCam.Invoke(player);
            };
            stopAction = () => { Hack.MiniCam.SetToggle(false); };

            action = SpectatePlayer.isCamPlayer(player) ? stopAction : startAction;

            UI.Button("PlayerTab.MiniCam", action, btnText);
        }
    }
}