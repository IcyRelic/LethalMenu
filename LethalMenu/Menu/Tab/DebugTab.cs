using System.Linq;
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;


namespace LethalMenu.Menu.Tab;

internal class DebugTab() : MenuTab("Debug")
{
    private readonly string[] _modes = ["Mode 1", "Mode 2", "Mode 3"];

    private Vector2 _scrollPosition = Vector2.zero;

    private int _selectedMode;

    public override void Draw()
    {
        GUILayout.BeginVertical();
        MenuContent();
        GUILayout.EndVertical();
    }

    private async void Leaderboard()
    {
        var weekNumb = GameNetworkManager.Instance.GetWeekNumber();
        var leaderboardAsync = await SteamUserStats.FindOrCreateLeaderboardAsync(
            $"challenge{weekNumb}", LeaderboardSort.Descending, LeaderboardDisplay.Numeric);

        if (leaderboardAsync == null) return;

        var nullable = await leaderboardAsync.Value.ReplaceScore(int.MaxValue);

        if (nullable != null)
            LethalMenu.DebugMessage = nullable.Value.OldGlobalRank + " => " + nullable.Value.NewGlobalRank;
    }

    private void MenuContent()
    {
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        if (GUILayout.Button("Clear Debug Message"))
        {
            LethalMenu.DebugMessage = "";
            LethalMenu.DebugMessage2 = "";
        }

        GUILayout.TextArea(LethalMenu.DebugMessage, GUILayout.Height(50));
        GUILayout.TextArea(LethalMenu.DebugMessage2, GUILayout.Height(50));

        UI.IndexSelect("Message Mode: ", ref _selectedMode, _modes);
        UI.Label("Selected Mode: " + _modes[_selectedMode]);


        UI.Button("LookAt Closest Item", () =>
        {
            var item = LethalMenu.Items.Where(i => !i.isInShipRoom).OrderBy(
                i => Vector3.Distance(i.transform.position, LethalMenu.LocalPlayer.transform.position)
            ).FirstOrDefault();

            if (item == null) return;

            LethalMenu.LocalPlayer.transform.LookAt(item.transform.position);
        });


        UI.Button("LookAt Closest Player", () =>
        {
            var player = LethalMenu.Players.Where(p => p != LethalMenu.LocalPlayer).OrderBy(
                p => Vector3.Distance(p.transform.position, LethalMenu.LocalPlayer.transform.position)
            ).FirstOrDefault();

            if (player == null) return;

            LethalMenu.LocalPlayer.transform.LookAt(player.transform.position);
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
            LethalMenu.LocalPlayer.TeleportPlayer(StartOfRound.Instance.notSpawnedPosition.position);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Raycast Colliders");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Execute"))
            foreach (var hit in CameraManager.ActiveCamera.transform.SphereCastForward())
            {
                var collider = hit.collider;

                LethalMenu.DebugMessage += "Hit: " + collider.name + " =>" + collider.gameObject.name + "\n";
            }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Garage");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Execute"))
            LethalMenu.InteractTriggers.ForEach(t =>
            {
                if (t == null || t.name != "Cube" || t.transform.parent.name != "Cutscenes") return;

                t.randomChancePercentage = 100;
                t.Interact(LethalMenu.LocalPlayer.transform);
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