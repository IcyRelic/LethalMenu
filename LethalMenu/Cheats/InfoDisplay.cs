using System;
using System.Linq;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class InfoDisplay : Cheat
    {
        public override void OnGui()
        {
            String info = "";

            if (Hack.DisplayBodyCount.IsEnabled()) info += "Dead Body Count: " + GetBodyCount() + "\n";
            if (Hack.DisplayEnemyCount.IsEnabled()) info += "Enemy Count: " + GetEnemyCount() + "\n";
            if (Hack.DisplayObjectScan.IsEnabled()) info += "Object Count: " + GetObjectCount() + "\n";
            if (Hack.DisplayObjectScan.IsEnabled()) info += "Object Value: " + GetObjectValue() + "\n";
            if (Hack.DisplayShipScan.IsEnabled()) info += "Ship Value: " + GetShipValue() + "\n";
            if (Hack.DisplayQuota.IsEnabled()) info += "Quota: " + GetQuota() + "\n";
            if (Hack.DisplayQuota.IsEnabled()) info += "Days Left: " + GetDaysLeft() + "\n";
            if (Hack.DisplayBuyingRate.IsEnabled()) info += "Company Buys At: " + GetBuyingRate() + "%\n";

            GUI.color = Color.white;
            GUI.Label(new Rect(Screen.width - 160 - 0, 0, 160f, 180f), info, new GUIStyle(GUI.skin.label) { fontSize = 14 }); ;
        }

        private int GetBodyCount()
        {
            return LethalMenu.players.FindAll(player => player.isPlayerDead).Count();
        }

        private int GetEnemyCount()
        {
            return LethalMenu.enemies.FindAll(enemy => !enemy.isEnemyDead).Count();
        }

        private int GetShipValue()
        {
            return LethalMenu.items.FindAll(item => item.isInShipRoom && !item.isHeld && !item.isPocketed).Sum(item => item.scrapValue);
        }

        private int GetObjectValue()
        {
            return LethalMenu.items.FindAll(item => !item.isInShipRoom && !item.isHeld && !item.isPocketed).Sum(item => item.scrapValue);
        }

        private int GetObjectCount()
        {
            return LethalMenu.items.FindAll(item => !item.isInShipRoom && !item.isHeld && !item.isPocketed).Count();
        }

        private int GetQuota()
        {
            if (!(bool)StartOfRound.Instance) return 0;
            return TimeOfDay.Instance.profitQuota;
        }

        private int GetDaysLeft()
        {
            if (!(bool)StartOfRound.Instance) return 0;
            return TimeOfDay.Instance.daysUntilDeadline;
        }

        private float GetBuyingRate()
        {
            if (!(bool)StartOfRound.Instance) return 0f;
            return StartOfRound.Instance.companyBuyingRate * 100;
        }

    }
}
