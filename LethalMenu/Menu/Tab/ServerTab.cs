using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Tab;

internal class ServerTab() : MenuTab("ServerTab.Title")
{
    private string _sCredits = "";
    private string _sQuota = "";
    private string _sScrapAmount = "1";
    private string _sScrapValue = "1";

    private Vector2 scrollPos = Vector2.zero;

    public override void Draw()
    {
        GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.ContentWidth * 0.5f - HackMenu.SpaceFromLeft));
        ServerMenuContent();
        GUILayout.EndVertical();

        GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.ContentWidth * 0.5f - HackMenu.SpaceFromLeft));
        ManagersContent();
        InfoMenuContent();
        GUILayout.EndVertical();
    }

    private void InfoMenuContent()
    {
        UI.Header("ServerTab.InfoDisplay");
        UI.Hack(Hack.DisplayBodyCount, "ServerTab.BodyCount");
        UI.Hack(Hack.DisplayEnemyCount, "ServerTab.EnemyCount");
        UI.Hack(Hack.DisplayObjectScan, "ServerTab.ObjectScan");
        UI.Hack(Hack.DisplayShipScan, "ServerTab.ShipScan");
        UI.Hack(Hack.DisplayQuota, "ServerTab.Quota");
        UI.Hack(Hack.DisplayBuyingRate, "ServerTab.BuyingRate");
    }

    private void ServerMenuContent()
    {
        UI.Header("ServerTab.ServerCheats");

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        UI.TextboxAction("ServerTab.EditCredits", ref _sCredits, @"[^0-9]", 50,
            new UIButton("General.Remove", () => Hack.ModifyCredits.Execute(int.Parse(_sCredits), ActionType.Remove)),
            new UIButton("General.Add", () => Hack.ModifyCredits.Execute(int.Parse(_sCredits), ActionType.Add)),
            new UIButton("General.Set", () => Hack.ModifyCredits.Execute(int.Parse(_sCredits), ActionType.Set))
        );

        UI.TextboxAction(["ServerTab.EditQuota", "General.HostTag"], ref _sQuota, @"[^0-9]", 50,
            new UIButton("General.Set", () => Hack.ModifyQuota.Execute(int.Parse(_sQuota)))
        );

        UI.TextboxAction(["ServerTab.ScrapAmount", "General.HostTag"], ref _sScrapAmount, @"[^0-9]", 3,
            new UIButton("General.Set", () => Hack.ModifyScrap.Execute(int.Parse(_sScrapAmount), 0))
        );

        UI.TextboxAction(["ServerTab.ScrapValue", "General.HostTag"], ref _sScrapValue, @"[^0-9]", 3,
            new UIButton("General.Set", () => Hack.ModifyScrap.Execute(int.Parse(_sScrapValue), 1))
        );

        UI.Hack(Hack.StartGame, "ServerTab.ForceLand");
        UI.Hack(Hack.EndGame, "ServerTab.ForceLeave");
        UI.Hack(Hack.NeverLoseScrap, "ServerTab.NeverLoseScrap");
        UI.Hack(Hack.SpawnMoreScrap, ["ServerTab.SpawnScrap", "General.HostTag"]);
        UI.Hack(Hack.SpawnMapObjects, ["ServerTab.SpawnRandMines", "General.HostTag"], MapObject.Landmine);
        UI.Hack(Hack.SpawnMapObjects, ["ServerTab.SpawnRandTurrets", "General.HostTag"], MapObject.TurretContainer);
        UI.Hack(Hack.SpawnLandmine, ["ServerTab.SpawnLandmine", "General.HostTag"]);
        UI.Hack(Hack.SpawnTurret, ["ServerTab.SpawnTurret", "General.HostTag"]);
        UI.Hack(Hack.Shoplifter, "ServerTab.Shoplifter");

        GUILayout.EndScrollView();
    }

    private static void ManagersContent()
    {
        UI.Header("ServerTab.Managers");
        UI.Toggle("MoonManager.Title", ref HackMenu.Instance.MoonManagerWindow.IsOpen, "General.Close", "General.Open");
        UI.Toggle("UnlockableManager.Title", ref HackMenu.Instance.UnlockableManagerWindow.IsOpen, "General.Close",
            "General.Open");
        UI.Toggle("ItemManager.Title", ref HackMenu.Instance.ItemManagerWindow.IsOpen, "General.Close", "General.Open");
        UI.Toggle("LootManager.Title", ref HackMenu.Instance.LootManager.IsOpen, "General.Close", "General.Open");
    }
}