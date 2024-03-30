using System;
using System.Collections.Generic;
using LethalMenu.Handler;
using LethalMenu.Language;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using UnityEngine;
using Behaviour = LethalMenu.Handler.Behaviour;
using Object = UnityEngine.Object;


namespace LethalMenu.Menu.Tab;

internal class EnemyTab() : MenuTab("EnemyTab.Title")
{
    private static int _selectedEnemy = -1;
    private static int _selectedEnemyType = -1;
    private readonly string[] _tabs = ["EnemyTab.EnemyList", "EnemyTab.SpawnEnemies"];
    private bool _bSpawnOutside;

    private Vector2 _scrollPosition = Vector2.zero;
    private Vector2 _scrollPosition2 = Vector2.zero;
    private Vector2 _scrollPosition3 = Vector2.zero;
    private int _selectedTab;

    private string _sSpawnAmount = "1";

    public override void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.ContentWidth - HackMenu.SpaceFromLeft));
        _selectedTab = GUILayout.Toolbar(_selectedTab, Localization.LocalizeArray(_tabs));


        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical(
            GUILayout.Width(HackMenu.Instance.ContentWidth * 0.3f - HackMenu.SpaceFromLeft));
        EnemyList();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(
            GUILayout.Width(HackMenu.Instance.ContentWidth * 0.7f - HackMenu.SpaceFromLeft));
        _scrollPosition2 = GUILayout.BeginScrollView(_scrollPosition2);

        switch (_selectedTab)
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

    private void DrawList<T>(string title, IEnumerable<T> objects, Func<T, bool> conditional, Func<T, string> label,
        ref Vector2 scroll, ref int instanceID) where T : Object
    {
        var width = HackMenu.Instance.ContentWidth * 0.3f - HackMenu.SpaceFromLeft * 2;
        var height = HackMenu.Instance.ContentHeight - 45;

        var rect = new Rect(0, 30, width, height);
        GUI.Box(rect, title);

        GUILayout.BeginVertical(GUILayout.Width(width), GUILayout.Height(height));
        GUILayout.Space(25);
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        foreach (var item in objects)
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
        switch (_selectedTab)
        {
            case 0:
                if (!LethalMenu.Enemies.Exists(e => e.GetInstanceID() == _selectedEnemy)) _selectedEnemy = -1;
                DrawList("Enemy List", LethalMenu.Enemies, e => e.isEnemyDead, e => e.enemyType.name,
                    ref _scrollPosition,
                    ref _selectedEnemy);
                break;
            case 1:
                if (!GameUtil.GetEnemyTypes().Exists(e => e.GetInstanceID() == _selectedEnemyType))
                    _selectedEnemyType = -1;
                DrawList("Enemy Types", GameUtil.GetEnemyTypes(), _ => false, e => e.name, ref _scrollPosition3,
                    ref _selectedEnemyType);
                break;
        }
    }


    private static void GeneralActions()
    {
        UI.Header("General.GeneralActions");
        UI.Hack(Hack.KillAllEnemies, "EnemyTab.KillAllEnemies");
        UI.HackSlider(Hack.KillNearbyEnemies, "EnemyTab.KillNearbyEnemies",
            Settings.f_enemyKillDistance.ToString("0") + "m", ref Settings.f_enemyKillDistance, 5, 50,
            (int)Settings.f_enemyKillDistance);
        UI.Hack(Hack.StunAllEnemies, "EnemyTab.StunAllEnemies");

        if (LethalMenu.Enemies.Exists(e => e is SandSpiderAI))
            UI.Hack(Hack.BreakAllWebs, "EnemyTab.BreakAllSpiderWeb");

        if (Hack.EnemyControl.IsEnabled())
            UI.Button("EnemyTab.StopEnemyControl", () => { Hack.EnemyControl.SetToggle(false); });
    }

    private void EnemyActions()
    {
        UI.Header("EnemyTab.EnemyStatus", true);

        if (_selectedEnemy == -1)
        {
            UI.Label(Settings.c_error.AsString("EnemyTab.NoEnemy"));
            return;
        }


        var selectedPlayer = LethalMenu.Players.Find(p => (int)p.playerClientId == PlayersTab.SelectedPlayer);
        var enemy = GetSelectedEnemy();

        var sTarget = !selectedPlayer ? "None" : selectedPlayer.playerUsername;

        if (Enum.TryParse(enemy.currentBehaviourStateIndex.ToString(), out Behaviour behavior))
            UI.Label("EnemyTab.Behaviour", behavior.ToString());

        UI.Label("EnemyTab.SelectedPlayer", Settings.c_playerESP.AsString(sTarget));

        UI.Label("EnemyTab.Targeting", !enemy.targetPlayer ? "None" : enemy.targetPlayer.playerUsername);

        if (enemy is DressGirlAI girl)
            UI.Label("EnemyTab.Haunting", !girl.hauntingPlayer ? "None" : girl.hauntingPlayer.playerUsername);

        UI.Header("EnemyTab.EnemyActions", true);

        switch (enemy)
        {
            case HoarderBugAI bug:
                UI.Button("EnemyTab.StealItems", () => { bug.StealAllItems(); });
                break;
            case SandSpiderAI spider:
                UI.Button("EnemyTab.SpawnWeb", () => { spider.SpawnWeb(spider.abdomen.position); });
                break;
        }

        UI.Button("EnemyTab.KillEnemy", () => { enemy.Handle().Kill(); });
        UI.Button("EnemyTab.TargetSelectedPlayer", () => { enemy.Handle().TargetPlayer(selectedPlayer); });

        if (enemy.Handle().HasInstaKill())
            UI.Button("EnemyTab.KillSelectedPlayer", () => { enemy.Handle().KillPlayer(selectedPlayer); });

        UI.Hack(Hack.TeleportEnemy, "EnemyTab.TeleportSelectedPlayer", selectedPlayer, enemy);
        UI.Hack(Hack.EnemyControl, "EnemyTab.ControlEnemy", enemy);
        UI.Button("EnemyTab.Control", () => { Hack.EnemyControl.Execute(enemy); });
    }

    private void EnemySpawnerContent()
    {
        if (!(bool)StartOfRound.Instance || !LethalMenu.LocalPlayer) return;

        if (!LethalMenu.LocalPlayer.IsHost)
        {
            UI.Label("General.HostRequired", Settings.c_error);
            return;
        }

        if (_selectedEnemyType == -1)
        {
            UI.Label("EnemyTab.NoEnemyType", Settings.c_error);
            return;
        }

        var type = GameUtil.GetEnemyTypes().Find(x => x.GetInstanceID() == _selectedEnemyType);

        UI.Label("EnemyTab.SelectedType", type.name, Settings.c_enemyESP);
        UI.Textbox("EnemyTab.SpawnAmount", ref _sSpawnAmount, "[^0-9]");

        UI.Checkbox("EnemyTab.SpawnOutside", ref _bSpawnOutside);
        UI.Button("EnemyTab.Spawn", () => HackExecutor.SpawnEnemy(type, int.Parse(_sSpawnAmount), _bSpawnOutside));
    }

    private EnemyAI GetSelectedEnemy()
    {
        return LethalMenu.Enemies.Find(x => x.GetInstanceID() == _selectedEnemy);
    }
}