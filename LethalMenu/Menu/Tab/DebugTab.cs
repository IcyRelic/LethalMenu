using GameNetcodeStuff;
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using Steamworks;
using Steamworks.Data;
using System.Linq;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;



namespace LethalMenu.Menu.Tab
{
    internal class DebugTab : MenuTab
    {
        public DebugTab() : base("Debug") { }

        private Vector2 scrollPos = Vector2.zero;

        public override void Draw()
        {
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
            MenuContent();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));

            GUILayout.EndVertical();
        }

        private async void Leaderboard()
        {
            int weekNum = GameNetworkManager.Instance.GetWeekNumber();
            Leaderboard? leaderboardAsync = await SteamUserStats.FindOrCreateLeaderboardAsync(
                string.Format("challenge{0}", weekNum), LeaderboardSort.Descending, LeaderboardDisplay.Numeric);

            LeaderboardUpdate? nullable = await leaderboardAsync.Value.ReplaceScore(int.MaxValue);

            LethalMenu.debugMessage = nullable.Value.OldGlobalRank + " => " + nullable.Value.NewGlobalRank;


        }

        private int selectedMode = 0;
        private string[] modes = new string[] { "Mode 1", "Mode 2", "Mode 3" };
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
                GrabbableObject item = LethalMenu.items.Where(i => !i.isInShipRoom).OrderBy(
                    i => Vector3.Distance(i.transform.position, LethalMenu.localPlayer.transform.position)
                ).FirstOrDefault();

                if (item == null) return;

                LethalMenu.localPlayer.transform.LookAt(item.transform.position);
            });


            UI.Button("LookAt Closest Player", () =>
            {
                PlayerControllerB player = LethalMenu.players.Where(p => p != LethalMenu.localPlayer).OrderBy(
                        p => Vector3.Distance(p.transform.position, LethalMenu.localPlayer.transform.position)
                ).FirstOrDefault();

                if (player == null) return;

                LethalMenu.localPlayer.transform.LookAt(player.transform.position);
            });



            GUILayout.Label("Debug Menu");

            GUILayout.BeginHorizontal();
            GUILayout.Label("Leaderboard");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                Leaderboard();
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Goto Not Spawned");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                LethalMenu.localPlayer.TeleportPlayer(StartOfRound.Instance.notSpawnedPosition.position);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Raycast Colliders");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                foreach (RaycastHit hit in CameraManager.ActiveCamera.transform.SphereCastForward())
                {
                    Collider collider = hit.collider;

                    LethalMenu.debugMessage += "Hit: " + collider.name + " =>" + collider.gameObject.name + "\n";
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Garage");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                LethalMenu.interactTriggers.ForEach(t =>
                {
                    if (t == null || t.name != "Cube" || t.transform.parent.name != "Cutscenes") return;

                    t.randomChancePercentage = 100;
                    t.Interact(LethalMenu.localPlayer.transform);

                });
            }
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
}
