using System.Linq;
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;


namespace LethalMenu.Menu.Tab;

internal class DebugTab : MenuTab
{
    private readonly string[] modes = { "Mode 1", "Mode 2", "Mode 3" };

    private Vector2 scrollPos = Vector2.zero;

    private int selectedMode;

    public DebugTab() : base("Debug")
    {
    }

    public override void Draw()
    {
        GUILayout.BeginVertical();
        MenuContent();
        GUILayout.EndVertical();
    }

    private async void Leaderboard()
    {
        var weekNum = GameNetworkManager.Instance.GetWeekNumber();
        var leaderboardAsync = await SteamUserStats.FindOrCreateLeaderboardAsync(
            string.Format("challenge{0}", weekNum), LeaderboardSort.Descending, LeaderboardDisplay.Numeric);

        var nullable = await leaderboardAsync.Value.ReplaceScore(int.MaxValue);

        LethalMenu.debugMessage = nullable.Value.OldGlobalRank + " => " + nullable.Value.NewGlobalRank;
    }

    private void MenuContent()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos);

        if (GUILayout.Button("Clear Debug Message"))
        {
            LethalMenu.debugMessage = "";
            LethalMenu.debugMessage2 = "";
        }

        GUILayout.TextArea(LethalMenu.debugMessage, GUILayout.Height(50));
        GUILayout.TextArea(LethalMenu.debugMessage2, GUILayout.Height(50));

        UI.IndexSelect("Message Mode: ", ref selectedMode, modes);
        UI.Label("Selected Mode: " + modes[selectedMode]);


        UI.Button("LookAt Closest Item", () =>
        {
            var item = LethalMenu.items.Where(i => !i.isInShipRoom).OrderBy(
                i => Vector3.Distance(i.transform.position, LethalMenu.localPlayer.transform.position)
            ).FirstOrDefault();

            if (item == null) return;

            LethalMenu.localPlayer.transform.LookAt(item.transform.position);
        });


        UI.Button("LookAt Closest Player", () =>
        {
            var player = LethalMenu.players.Where(p => p != LethalMenu.localPlayer).OrderBy(
                p => Vector3.Distance(p.transform.position, LethalMenu.localPlayer.transform.position)
            ).FirstOrDefault();

            if (player == null) return;

            LethalMenu.localPlayer.transform.LookAt(player.transform.position);
        });


        GUILayout.Label("Debug Menu");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Leaderboard");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Execute")) Leaderboard();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Goto Not Spawned");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Execute"))
            LethalMenu.localPlayer.TeleportPlayer(StartOfRound.Instance.notSpawnedPosition.position);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Raycast Colliders");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Execute"))
            foreach (var hit in CameraManager.ActiveCamera.transform.SphereCastForward())
            {
                var collider = hit.collider;

                LethalMenu.debugMessage += "Hit: " + collider.name + " =>" + collider.gameObject.name + "\n";
            }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Garage");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Execute"))
            LethalMenu.interactTriggers.ForEach(t =>
            {
                if (t == null || t.name != "Cube" || t.transform.parent.name != "Cutscenes") return;

                t.randomChancePercentage = 100;
                t.Interact(LethalMenu.localPlayer.transform);
            });
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Sell All");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Execute"))
        {
        }

        GUILayout.EndHorizontal();


        GUILayout.EndScrollView();
    }
}