using GameNetcodeStuff;
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using Steamworks;
using Steamworks.Data;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;


namespace LethalMenu.Menu.Tab
{
    internal class DebugTab : MenuTab
    {
        private Vector2 scrollPos = Vector2.zero;
        public DebugTab() : base("Debug") { }

        public override void Draw()
        {
            GUILayout.BeginVertical();
            MenuContent();
            GUILayout.EndVertical();
        }

        private void MenuContent()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            if (GUILayout.Button("Clear Debug Message")) Settings.debugMessage = "";
            GUILayout.TextArea(Settings.debugMessage, GUILayout.Height(100));

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
            GUILayout.Label("Debug NetworkObjectReferences");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                NetworkManager.Singleton.SpawnManager.SpawnedObjects.ToList().ForEach(s =>
                {
                    Debug.Log($"Name: {s.Value.gameObject.name} ID: {s.Key}");
                });
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Debug GetSpawnableMapObjects");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                GameUtil.GetSpawnableMapObjects().ToList().ForEach(o => Debug.Log(o.prefabToSpawn.name));
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
            GUILayout.Label("Debug all layers");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                List<int> layers = new List<int>();
                GameObject.FindObjectsOfType<GameObject>().Where(o => o != null).ToList().ForEach(o =>
                {
                    if (!layers.Contains(o.layer)) layers.Add(o.layer);
                    o.GetComponents<Component>().Where(c => c != null).ToList().ForEach(c =>
                    {
                        if (!layers.Contains(c.gameObject.layer)) layers.Add(c.gameObject.layer);
                    });
                });
                layers.ForEach(i => Debug.Log($"Layer: {LayerMask.LayerToName(i)} ID: {i}"));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Raycast Colliders");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                Settings.debugMessage = ""; 
                foreach (RaycastHit hit in CameraManager.ActiveCamera.transform.SphereCastForward())
                {
                    Collider collider = hit.collider;
                    Settings.debugMessage += $"Hit: {collider.name} => {collider.gameObject.name} => Layer {LayerMask.LayerToName(collider.gameObject.layer)} {collider.gameObject.layer}\n";
                }
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Garage");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                LethalMenu.animatedTriggers.Where(t => t.name == "Cube" && t.transform.parent.name == "Cutscenes").ToList().ForEach(t => t.triggerAnimator?.SetTrigger("doorFall"));
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Debug all components of held item");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                LethalMenu.localPlayer?.currentlyHeldObjectServer?.GetComponents<Component>().Where(c => c != null).ToList().ForEach(c => Debug.Log($"{c.GetType().FullName}"));
            }
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }

        private async void Leaderboard()
        {
            Leaderboard? leaderboardAsync = await SteamUserStats.FindOrCreateLeaderboardAsync(string.Format("challenge{0}", GameNetworkManager.Instance.GetWeekNumber()), LeaderboardSort.Descending, LeaderboardDisplay.Numeric);

            LeaderboardUpdate? nullable = await leaderboardAsync.Value.ReplaceScore(int.MaxValue);

            Settings.debugMessage = (nullable.Value.OldGlobalRank + " => " + nullable.Value.NewGlobalRank);
        }
    }
}