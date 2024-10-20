using GameNetcodeStuff;
using LethalMenu.Handler;
using LethalMenu.Language;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;


namespace LethalMenu.Menu.Tab
{
    internal class EnemyTab : MenuTab
    {
        private readonly string[] tabs = ["EnemyTab.EnemyList", "EnemyTab.SpawnEnemies"];

        public static int selectedEnemy = -1;
        public static int selectedEnemyType = -1;
        private int selectedTab = 0;

        private string s_spawnAmount = "1";
        private bool b_spawnOutside = false;

        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;
        private Vector2 scrollPos3 = Vector2.zero;
        public EnemyTab() : base("EnemyTab.Title") { }

        public override void Draw()
        {
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth - HackMenu.Instance.spaceFromLeft));
            selectedTab = GUILayout.Toolbar(selectedTab, Localization.LocalizeArray(tabs));


            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.3f - HackMenu.Instance.spaceFromLeft));
            EnemyList();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.7f - HackMenu.Instance.spaceFromLeft));
            scrollPos2 = GUILayout.BeginScrollView(scrollPos2);

            switch (selectedTab)
            {
                case 0:
                    GeneralActions();
                    EnemyActions();
                    break;
                case 1:
                    EnemySpawnerContent();
                    break;
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            
        }

        private void DrawList<T>(string title, IEnumerable<T> objects, Func<T, bool> conditional, Func<T, string> label, ref Vector2 scroll, ref int instanceID) where T : Object
        {
            float width = HackMenu.Instance.contentWidth * 0.3f - HackMenu.Instance.spaceFromLeft * 2;
            float height = HackMenu.Instance.contentHeight - 45;

            Rect rect = new Rect(0, 30, width, height);
            GUI.Box(rect, Localization.Localize(title));

            GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));
            GUILayout.Space(25);
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            foreach (T item in objects)
            {
                if (conditional(item)) continue;

                if (instanceID == -1) instanceID = item.GetInstanceID();

                if (instanceID == item.GetInstanceID()) GUI.contentColor = Settings.c_enemyESP.GetColor();

                if (GUILayout.Button(label(item), GUI.skin.label)) instanceID = item.GetInstanceID();

                GUI.contentColor = Settings.c_menuText.GetColor();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        private void EnemyList()
        {      
            switch (selectedTab)
            {
                case 0:
                    if (!LethalMenu.enemies.Exists(e => e.GetInstanceID() == selectedEnemy)) selectedEnemy = -1;
                    DrawList<EnemyAI>("EnemyTab.EnemyList", LethalMenu.enemies.OrderBy(e => e.enemyType.name).ToList(), e => e.isEnemyDead, e => e.enemyType.name, ref scrollPos, ref selectedEnemy);
                    break;
                case 1:
                    if (!GameUtil.GetEnemyTypes().Exists(e => e.GetInstanceID() == selectedEnemyType)) selectedEnemyType = -1;
                    DrawList<EnemyType>("EnemyTab.EnemyTypes", GameUtil.GetEnemyTypes().OrderBy(e => e.name).ToList(), _ => false, e => e.name, ref scrollPos3, ref selectedEnemyType);
                    break;
            }
        }

        private void GeneralActions()
        {
            UI.Header("General.GeneralActions");
            UI.Hack(Hack.EnemyDeathNotifications, "PlayerTab.EnemyDeathNotifications");
            UI.Hack(Hack.KillAllEnemies, "EnemyTab.KillAllEnemies");
            UI.HackSlider(Hack.KillNearbyEnemies, "EnemyTab.KillNearbyEnemies", Settings.f_enemyKillDistance.ToString("0") + "m", ref Settings.f_enemyKillDistance, 5, 50, (int) Settings.f_enemyKillDistance);
            UI.Hack(Hack.StunAllEnemies, "EnemyTab.StunAllEnemies");

            if (LethalMenu.enemies.Exists(e => e is SandSpiderAI))
                UI.Hack(Hack.BreakAllWebs, "EnemyTab.BreakAllSpiderWeb");

            if (Hack.EnemyControl.IsEnabled())
                UI.Button("EnemyTab.StopEnemyControl", () => { Hack.EnemyControl.SetToggle(false); });
        }

        private void EnemyActions()
        {
            UI.Header("EnemyTab.EnemyStatus", true);

            if (selectedEnemy == -1) { UI.Label(Settings.c_error.AsString("EnemyTab.NoEnemy")); return; }

            PlayerControllerB selectedPlayer = LethalMenu.players.Find(p => (int)p.playerClientId == PlayersTab.selectedPlayer);
            EnemyAI enemy = GetSelectedEnemy();

            string s_target = selectedPlayer == null ? "None" : selectedPlayer.playerUsername;

            if(Enum.TryParse(enemy.currentBehaviourStateIndex.ToString(), out Handler.Behaviour behavior))
                UI.Label("EnemyTab.Behaviour", behavior.ToString());

            UI.Label("EnemyTab.SelectedPlayer", Settings.c_playerESP.AsString(s_target));

            UI.Label("EnemyTab.Targeting", (enemy.targetPlayer == null ? "None" : enemy.targetPlayer.playerUsername));

            if(enemy is DressGirlAI girl)
                UI.Label("EnemyTab.Haunting", (girl.hauntingPlayer == null ? "None" : girl.hauntingPlayer.playerUsername));

            UI.Header("EnemyTab.EnemyActions", true);

            if (enemy is HoarderBugAI bug) UI.Button("EnemyTab.StealItems", () => { bug.StealAllItems(); });
            if (enemy is SandSpiderAI spider) UI.Button("EnemyTab.SpawnWeb", () => { spider.SpawnWeb(spider.abdomen.position); });

            UI.Button("EnemyTab.KillEnemy", () => { enemy.Handle().Kill(); });
            UI.Button("EnemyTab.StunEnemy", () => { enemy.Handle().Stun(); });
            UI.Button("EnemyTab.TargetSelectedPlayer", () => { enemy.Handle().TargetPlayer(selectedPlayer); });
            
            if(enemy.Handle().HasInstaKill())
                UI.Button("EnemyTab.KillSelectedPlayer", () => { enemy.Handle().KillPlayer(selectedPlayer); });

            UI.Hack(Hack.TeleportEnemy, "EnemyTab.TeleportSelectedPlayer", selectedPlayer, enemy);
            UI.Hack(Hack.EnemyControl, "EnemyTab.ControlEnemy", enemy);
        }

        private void EnemySpawnerContent()
        {
            if (!(bool)StartOfRound.Instance || LethalMenu.localPlayer == null) return;

            if (!LethalMenu.localPlayer.IsHost) { UI.Label("General.HostRequired", Settings.c_error); return; }
            if(selectedEnemyType == -1) { UI.Label("EnemyTab.NoEnemyType", Settings.c_error); return; }

            EnemyType type = GameUtil.GetEnemyTypes().Find(x => x.GetInstanceID() == selectedEnemyType);

            UI.Label("EnemyTab.SelectedType", type.name, Settings.c_enemyESP);
            UI.Textbox("EnemyTab.SpawnAmount", ref s_spawnAmount, @"[^0-9]");

            if (type.enemyName != "Bush Wolf") UI.Checkbox("EnemyTab.SpawnOutside", ref b_spawnOutside);
            UI.Button("EnemyTab.Spawn", () => HackExecutor.SpawnEnemy(type, int.Parse(s_spawnAmount), b_spawnOutside));
        }

        private EnemyAI GetSelectedEnemy()
        {
            return LethalMenu.enemies.Find(x => x.GetInstanceID() == selectedEnemy);
        }
    }
}
