using System;
using System.Linq;
using LethalMenu.Language;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class InfoDisplay : Cheat
    {
        public override void OnGui()
        {
            String info = "";

            if (Hack.DisplayBodyCount.IsEnabled()) info += Localization.Localize(["Cheats.Info.BodyCount"], true) + ": " + GetBodyCount() + "\n";
            if (Hack.DisplayEnemyCount.IsEnabled()) info += Localization.Localize(["Cheats.Info.EnemyCount"], true) + ": " + GetEnemyCount() + "\n";
            if (Hack.DisplayObjectScan.IsEnabled()) info += Localization.Localize(["Cheats.Info.ObjectCount"], true) + ": " + GetObjectCount() + "\n";
            if (Hack.DisplayObjectScan.IsEnabled()) info += Localization.Localize(["Cheats.Info.ObjectValue"], true) + ": " + GetObjectValue() + "\n";
            if (Hack.DisplayShipScan.IsEnabled()) info += Localization.Localize(["Cheats.Info.ShipValue"], true) + ": " + GetShipValue() + "\n";
            if (Hack.DisplayQuota.IsEnabled()) info += Localization.Localize(["Cheats.Info.Quota"], true) + ": " + GetQuota() + "\n";
            if (Hack.DisplayQuota.IsEnabled()) info += Localization.Localize(["Cheats.Info.DaysLeft"], true) + ": " + GetDaysLeft() + "\n";
            if (Hack.DisplayBuyingRate.IsEnabled()) info += Localization.Localize(["Cheats.Info.BuyingRate"], true) + ": " + GetBuyingRate() + "%\n";

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
