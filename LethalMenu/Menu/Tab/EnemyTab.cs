using GameNetcodeStuff;
using LethalMenu.Handler;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


namespace LethalMenu.Menu.Tab
{
    internal class EnemyTab : MenuTab
    {
        private string[] tabs = new string[] { "Enemy List", "Spawn Enemies" };

        public static int selectedEnemy = -1;
        public static int selectedEnemyType = -1;
        private int selectedTab = 0;

        private string s_spawnAmount = "1";
        private bool b_spawnOutside = false;

        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;
        private Vector2 scrollPos3 = Vector2.zero;
        public EnemyTab() : base("Enemy") { }

        public override void Draw()
        {
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth - HackMenu.Instance.spaceFromLeft));
            selectedTab = GUILayout.Toolbar(selectedTab, tabs);


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

        private void DrawList<T>(IEnumerable<T> objects, Func<T, bool> conditional, Func<T, string> label, ref Vector2 scroll, ref int instanceID) where T : Object
        {
            float width = HackMenu.Instance.contentWidth * 0.3f - HackMenu.Instance.spaceFromLeft * 2;
            float height = HackMenu.Instance.contentHeight - 45;

            Rect rect = new Rect(0, 30, width, height);
            GUI.Box(rect, GUIContent.none);

            GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));
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
                    DrawList<EnemyAI>(LethalMenu.enemies, e => e.isEnemyDead, e => e.enemyType.name, ref scrollPos, ref selectedEnemy);
                    break;
                case 1:
                    if(!GameUtil.GetEnemyTypes().Exists(e => e.GetInstanceID() == selectedEnemyType)) selectedEnemyType = -1;
                    DrawList<EnemyType>(GameUtil.GetEnemyTypes(), _ => false, e => e.name, ref scrollPos3, ref selectedEnemyType);
                    break;
            }

                
        }
        


        private void GeneralActions()
        {
            UI.Header("General Actions");
            UI.Hack(Hack.KillAllEnemies, "Kill All Enemies");
            UI.HackSlider(Hack.KillNearbyEnemies, "Kill Nearby Enemies", Settings.f_enemyKillDistance.ToString() + "m", ref Settings.f_enemyKillDistance, 5, 50);
            UI.Hack(Hack.StunAllEnemies, "Stun All Enemies");

            if(LethalMenu.enemies.Exists(e => e is SandSpiderAI))
                UI.Hack(Hack.BreakAllWebs, "Break All Spider Web");

            if (Hack.EnemyControl.IsEnabled())
                UI.Button("Stop Controlling Enemy", () => { Hack.EnemyControl.SetToggle(false); });


        }

        private void EnemyActions()
        {
            UI.Header("Enemy Actions");

            if (selectedEnemy == -1) { UI.Label(Settings.c_error.AsString("No Enemy Selected")); return; }


            PlayerControllerB selectedPlayer = LethalMenu.players.Find(p => (int)p.playerClientId == PlayersTab.selectedPlayer);
            EnemyAI enemy = GetSelectedEnemy();

            string s_target = selectedPlayer == null ? "None" : selectedPlayer.playerUsername;


            UI.Label("Selected Player: ", Settings.c_playerESP.AsString(s_target));

            UI.Label("Targeting: ", (enemy.targetPlayer == null ? "None" : enemy.targetPlayer.playerUsername));

            if(enemy is DressGirlAI girl)
                UI.Label("Haunting: ", (girl.hauntingPlayer == null ? "None" : girl.hauntingPlayer.playerUsername));

            if (enemy is HoarderBugAI bug) UI.Button("Steal Items", () => { bug.StealAllItems(); });
            if (enemy is SandSpiderAI spider) UI.Button("Spawn Web", () => { spider.SpawnWeb(spider.abdomen.position); });


            UI.Button("Kill", () => { enemy.Handle().Kill(); });
            UI.Button("Target Selected Player", () => { enemy.Handle().TargetPlayer(selectedPlayer); });
            UI.Button("Kill Selected Player", () => { enemy.Handle().KillPlayer(selectedPlayer); });
            UI.Hack(Hack.TeleportEnemy, "Teleport To Selected Player", selectedPlayer, enemy);
            UI.Hack(Hack.EnemyControl, "Control Enemy", enemy);
            UI.Button("Control", () => { Hack.EnemyControl.Execute(enemy); });


        }
        
        private void EnemySpawnerContent()
        {
            if (!(bool)StartOfRound.Instance || LethalMenu.localPlayer == null) return;

            if (!LethalMenu.localPlayer.IsHost) { UI.Label(Settings.c_error.AsString("This feature requires host!")); return; }
            if(selectedEnemyType == -1) { UI.Label(Settings.c_error.AsString("No Enemy Type Selected")); return; }

            EnemyType type = GameUtil.GetEnemyTypes().Find(x => x.GetInstanceID() == selectedEnemyType);

            UI.Label("Selected Enemy: ", Settings.c_enemyESP.AsString(type.name));
            UI.Textbox("Spawn Amount", ref s_spawnAmount, @"[^0-9]");

            UI.Checkbox("Spawn Outside", ref b_spawnOutside);
            UI.Button("Spawn", () => HackExecutor.SpawnEnemy(type, int.Parse(s_spawnAmount), b_spawnOutside));
        }

        private EnemyAI GetSelectedEnemy()
        {
            return LethalMenu.enemies.Find(x => x.GetInstanceID() == selectedEnemy);
        }
    }
}
