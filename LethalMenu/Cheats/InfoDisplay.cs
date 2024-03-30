using System.Linq;
using UnityEngine;

namespace LethalMenu.Cheats;

internal class InfoDisplay : Cheat
{
    public override void OnGui()
    {
        var info = "";

        if (Hack.DisplayBodyCount.IsEnabled()) info += "Dead Body Count: " + GetBodyCount() + "\n";
        if (Hack.DisplayEnemyCount.IsEnabled()) info += "Enemy Count: " + GetEnemyCount() + "\n";
        if (Hack.DisplayObjectScan.IsEnabled()) info += "Object Count: " + GetObjectCount() + "\n";
        if (Hack.DisplayObjectScan.IsEnabled()) info += "Object Value: " + GetObjectValue() + "\n";
        if (Hack.DisplayShipScan.IsEnabled()) info += "Ship Value: " + GetShipValue() + "\n";
        if (Hack.DisplayQuota.IsEnabled()) info += "Quota: " + GetQuota() + "\n";
        if (Hack.DisplayQuota.IsEnabled()) info += "Days Left: " + GetDaysLeft() + "\n";
        if (Hack.DisplayBuyingRate.IsEnabled()) info += "Company Buys At: " + GetBuyingRate() + "%\n";

        GUI.color = Color.white;
        GUI.Label(new Rect(Screen.width - 160 - 0, 0, 160f, 180f), info,
            new GUIStyle(GUI.skin.label) { fontSize = 14 });
    }

    private static int GetBodyCount()
    {
        return LethalMenu.Players.FindAll(player => player.isPlayerDead).Count();
    }

    private static int GetEnemyCount()
    {
        return LethalMenu.Enemies.FindAll(enemy => !enemy.isEnemyDead).Count();
    }

    private static int GetShipValue()
    {
        return LethalMenu.Items.FindAll(item => item.isInShipRoom && !item.isHeld && !item.isPocketed)
            .Sum(item => item.scrapValue);
    }

    private static int GetObjectValue()
    {
        return LethalMenu.Items.FindAll(item => !item.isInShipRoom && !item.isHeld && !item.isPocketed)
            .Sum(item => item.scrapValue);
    }

    private static int GetObjectCount()
    {
        return LethalMenu.Items.FindAll(item => !item.isInShipRoom && !item.isHeld && !item.isPocketed).Count();
    }

    private static int GetQuota()
    {
        if (!(bool)StartOfRound.Instance) return 0;
        return TimeOfDay.Instance.profitQuota;
    }

    private static int GetDaysLeft()
    {
        if (!(bool)StartOfRound.Instance) return 0;
        return TimeOfDay.Instance.daysUntilDeadline;
    }

    private static float GetBuyingRate()
    {
        if (!(bool)StartOfRound.Instance) return 0f;
        return StartOfRound.Instance.companyBuyingRate * 100;
    }
}