using System;
using System.Linq;
using LethalMenu.Language;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class InfoDisplay : Cheat
    {
        private static String info = "";
        private void Info(string label, string value) => info += $"{Localization.Localize(label)} {value} \n";

        public override void OnGui()
        {
            info = "";
        
            if (Hack.DisplayBodyCount.IsEnabled()) Info("Cheats.Info.DisplayBodyCount", GetBodyCount().ToString());
            if (Hack.DisplayEnemyCount.IsEnabled()) Info("Cheats.Info.DisplayEnemyCount", GetEnemyCount().ToString());
            if (Hack.DisplayObjectCount.IsEnabled()) Info("Cheats.Info.DisplayObjectCount", GetObjectCount().ToString());
            if (Hack.DisplayObjectValue.IsEnabled()) Info("Cheats.Info.DisplayObjectValue", GetObjectValue().ToString());
            if (Hack.DisplayShipObjectCount.IsEnabled()) Info("Cheats.Info.DisplayShipObjectCount", GetShipCount().ToString());
            if (Hack.DisplayShipObjectValue.IsEnabled()) Info("Cheats.Info.DisplayShipObjectValue", GetShipValue().ToString());
            if (Hack.DisplayQuota.IsEnabled()) Info("Cheats.Info.DisplayQuota", GetQuota().ToString());
            if (Hack.DisplayDeadline.IsEnabled()) Info("Cheats.Info.DisplayDeadline", GetDeadline().ToString());
            if (Hack.DisplayBuyingRate.IsEnabled()) Info("Cheats.Info.DisplayBuyingRate", GetBuyingRate().ToString() + "%");

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
            return LethalMenu.items.FindAll(item => item.isInShipRoom && !item.isHeld && !item.isPocketed && !DefaultShipItem(item)).Count();
        }

        private int GetShipValue()
        {
            return LethalMenu.items.FindAll(item => item.isInShipRoom && !item.isHeld && !item.isPocketed && !DefaultShipItem(item)).Sum(item => item.scrapValue);
        }

        private int GetObjectValue()
        {
            return LethalMenu.items.FindAll(item => !item.isInShipRoom && !item.isHeld && !item.isPocketed && !DefaultShipItem(item)).Sum(item => item.scrapValue);
        }

        private int GetObjectCount()
        {
            return LethalMenu.items.FindAll(item => !item.isInShipRoom && !item.isHeld && !item.isPocketed && !DefaultShipItem(item)).Count();
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
        private bool DefaultShipItem(GrabbableObject item)
        {
            if (item == null) return false;
            string[] Items = ["ClipboardManual", "StickyNoteItem"];
            return Items.Contains(item.name);
        }
    }
}
