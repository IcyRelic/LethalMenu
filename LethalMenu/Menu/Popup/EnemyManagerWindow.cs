using GameNetcodeStuff;
using LethalMenu.Cheats;
using LethalMenu.Handler;
using LethalMenu.Menu.Core;
using LethalMenu.Menu.Tab;
using LethalMenu.Util;
using System.Collections.Generic;
using UnityEngine;

namespace LethalMenu.Menu.Popup
{
    internal class EnemyManagerWindow : PopupMenu
    {
        private string[] tabs = new string[] { "Enemy List", "Spawn Enemies" };
        private Vector2 scrollPos = Vector2.zero;
        private Dictionary<EnemyType, string> amounts = new Dictionary<EnemyType, string>();

        public EnemyManagerWindow(int id) : base("Enemy Manager", new Rect(50f, 50f, 600f, 300f), id)
        {

        }

        public override void DrawContent(int windowID)
        {
            selectedTab = GUILayout.Toolbar(selectedTab, tabs);
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            switch (selectedTab)
            {
                case 0:
                    EnemyListContent();
                    break;
                case 1:
                    EnemySpawnerContent();
                    break;
            }
            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0.0f, 0.0f, 10000f, 45f));
        }

        private void EnemyListContent()
        {
            if (Hack.EnemyControl.IsEnabled())
            {
                if (GUILayout.Button("Stop Controlling Enemy")) Hack.EnemyControl.SetToggle(false);
            }

            foreach (EnemyAI enemy in LethalMenu.enemies)
            {
                if (enemy.isEnemyDead) continue;
                GUILayout.BeginHorizontal();
                GUILayout.Label(enemy.name);
                GUILayout.Space(10f);
                GUILayout.Label(enemy.targetPlayer == null ? "" : enemy.targetPlayer.name);

                GUILayout.FlexibleSpace();
                //if (GUILayout.Button("Despawn")) GameUtil.KillEnemy(enemy, true);
                if (GUILayout.Button("Kill")) enemy.Handle().Kill();
                if (GUILayout.Button("Target"))
                {
                    PlayerControllerB target = StartOfRound.Instance.allPlayerScripts[PlayersTab.selectedPlayer];
                    enemy.Handle().TargetPlayer(target);
                }
                if (GUILayout.Button("Teleport")) Hack.TeleportEnemy.Execute(StartOfRound.Instance.allPlayerScripts[PlayersTab.selectedPlayer], new EnemyAI[] { enemy });
                if (GUILayout.Button("Control")) Hack.EnemyControl.Execute(enemy);
                GUILayout.EndHorizontal();
            }
        }

        private void EnemySpawnerContent()
        {
            if (!LethalMenu.localPlayer.IsHost)
            {
                GUILayout.Label("<color=#DD0B0B>This feature requires host!</color>");
                return;
            }

            foreach (var enemy in GameUtil.GetEnemyTypes())
            {
                if (!amounts.ContainsKey(enemy)) amounts.Add(enemy, "1");

                GUILayout.BeginHorizontal();
                GUILayout.Label(enemy.name);
                GUILayout.FlexibleSpace();
                amounts[enemy] = GUILayout.TextField(amounts[enemy], 8, GUILayout.Width(Settings.GUISize.GetTextboxWidth()));
                amounts[enemy] = System.Text.RegularExpressions.Regex.Replace(amounts[enemy], @"[^0-9]", "");
                if (GUILayout.Button("Outside")) HackExecutor.SpawnEnemy(enemy, int.Parse(amounts[enemy]), true);
                if (GUILayout.Button("Inside")) HackExecutor.SpawnEnemy(enemy, int.Parse(amounts[enemy]), false);
                GUILayout.EndHorizontal();
            }
        }
    }
}
