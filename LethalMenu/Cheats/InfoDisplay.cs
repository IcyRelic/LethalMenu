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

            if (Hack.DisplayBodyCount.IsEnabled()) info += Localization.Localize("ServerTab.BodyCount") + ": " + GetBodyCount().ToString() + "\n";
            if (Hack.DisplayEnemyCount.IsEnabled()) info += Localization.Localize("ServerTab.EnemyCount") + ": " + GetEnemyCount().ToString() + "\n";
            if (Hack.DisplayObjectCount.IsEnabled()) info += Localization.Localize("ServerTab.ObjectCount") + ": " + GetObjectCount().ToString() + "\n";
            if (Hack.DisplayObjectValue.IsEnabled()) info += Localization.Localize("ServerTab.ObjectValue") + ": " + GetObjectValue().ToString() + "\n";
            if (Hack.DisplayShipScan.IsEnabled()) info += Localization.Localize("ServerTab.ShipScan") + ": " + GetShipValue().ToString() + "\n";
            if (Hack.DisplayQuota.IsEnabled()) info += Localization.Localize("ServerTab.Quota") + ": " + GetQuota() + "\n";
            if (Hack.DisplayDaysLeft.IsEnabled()) info += Localization.Localize("ServerTab.DaysLeft") + ": " + GetDaysLeft() + "\n";
            if (Hack.DisplayBuyingRate.IsEnabled()) info += Localization.Localize("ServerTab.BuyingRate") + ": " + GetBuyingRate().ToString() + "%\n";

            GUI.color = Color.white;
            GUI.Label(new Rect(Screen.width - 160 - 0, 0, 160f, 180f), info, new GUIStyle(GUI.skin.label) { fontSize = 14 });
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
