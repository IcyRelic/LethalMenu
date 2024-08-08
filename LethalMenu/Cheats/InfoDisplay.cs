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

            if (Hack.DisplayBodyCount.IsEnabled()) info += Localization.Localize("Cheats.Info.DisplayBodyCount") + " " + GetBodyCount() + "\n";
            if (Hack.DisplayEnemyCount.IsEnabled()) info += Localization.Localize("Cheats.Info.DisplayEnemyCount") + " " + GetEnemyCount() + "\n";
            if (Hack.DisplayObjectCount.IsEnabled()) info += Localization.Localize("Cheats.Info.DisplayObjectCount") + " " + GetObjectCount() + "\n";
            if (Hack.DisplayObjectValue.IsEnabled()) info += Localization.Localize("Cheats.Info.DisplayObjectValue") + " " + GetObjectValue() + "\n";
            if (Hack.DisplayShipObjectCount.IsEnabled()) info += Localization.Localize("Cheats.Info.DisplayShipObjectCount") + " " + GetShipCount() + "\n";
            if (Hack.DisplayShipObjectValue.IsEnabled()) info += Localization.Localize("Cheats.Info.DisplayShipObjectValue") + " " + GetShipValue() + "\n";
            if (Hack.DisplayQuota.IsEnabled()) info += Localization.Localize("Cheats.Info.DisplayQuota") + " " + GetQuota() + "\n";
            if (Hack.DisplayDeadline.IsEnabled()) info += Localization.Localize("Cheats.Info.DisplayDeadline") + " " + GetDeadline() + "\n";
            if (Hack.DisplayBuyingRate.IsEnabled()) info += Localization.Localize("Cheats.Info.DisplayBuyingRate") + " " + GetBuyingRate() + "%\n";

            GUI.color = Settings.c_primary.GetColor();
            GUI.Label(new Rect(Screen.width - 200 - 0, 0, 200f, 180f), info, new GUIStyle(GUI.skin.label) { fontSize = 14 }); ;
        }

        private int GetBodyCount()
        {
            return LethalMenu.players.FindAll(player => player.isPlayerDead).Count();
        }

        private int GetEnemyCount()
        {
            return LethalMenu.enemies.FindAll(enemy => !enemy.isEnemyDead).Count();
        }

        private int GetShipCount()
        {
            return LethalMenu.items.FindAll(item => item.isInShipRoom && !item.isHeld && !item.isPocketed && !GetDefaultShipItem(item)).Count();
        }

        private int GetShipValue()
        {
            return LethalMenu.items.FindAll(item => item.isInShipRoom && !item.isHeld && !item.isPocketed && !GetDefaultShipItem(item)).Sum(item => item.scrapValue);
        }

        private int GetObjectValue()
        {
            return LethalMenu.items.FindAll(item => !item.isInShipRoom && !item.isHeld && !item.isPocketed && !GetDefaultShipItem(item)).Sum(item => item.scrapValue);
        }

        private int GetObjectCount()
        {
            return LethalMenu.items.FindAll(item => !item.isInShipRoom && !item.isHeld && !item.isPocketed && !GetDefaultShipItem(item)).Count();
        }

        private int GetQuota()
        {
            if (!(bool)StartOfRound.Instance) return 0;
            return TimeOfDay.Instance.profitQuota;
        }

        private int GetDeadline()
        {
            if (!(bool)StartOfRound.Instance) return 0;
            return TimeOfDay.Instance.daysUntilDeadline;
        }

        private float GetBuyingRate()
        {
            if (!(bool)StartOfRound.Instance) return 0f;
            return StartOfRound.Instance.companyBuyingRate * 100;
        }
        private bool GetDefaultShipItem(GrabbableObject item)
        {
            if (item == null) return false;
            string[] Items = ["ClipboardManual", "StickyNoteItem"];
            return Items.Contains(item.name);
        }
    }
}
