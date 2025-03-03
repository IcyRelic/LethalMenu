using System;
using System.Linq;
using System.Reflection.Emit;
using LethalMenu.Handler;
using LethalMenu.Language;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class InfoDisplay : Cheat
    {
        private static String info = "";
        private void Format(string label, params object[] args) => info += $"{string.Format(Localization.Localize(label), args)}\n";
        /** 
         * InfoDisplay Rework Notes:
         * - Make the display more modern and clean
         * - Example: Rather than ship value and quota being displayed as two separate lines, display them as one line
         **/


        public override void OnGui()
        {
            info = "";

            if (Hack.DisplayBodies.IsEnabled())
                Format("Cheats.Info.Bodies", GetBodyCount());

            if (Hack.DisplayEnemies.IsEnabled())
                Format("Cheats.Info.Enemies", GetEnemyCount());

            if (Hack.DisplayMapObjects.IsEnabled())
                Format("Cheats.Info.MapObjects", GetObjectCount(), GetObjectValue());

            if (Hack.DisplayShipObjects.IsEnabled())
                Format("Cheats.Info.ShipObjects", GetShipCount(), GetShipValue());

            if (Hack.DisplayQuota.IsEnabled())
                Format("Cheats.Info.Quota", GetShipValue(), GetQuota());

            if (Hack.DisplayDeadline.IsEnabled())
                Format("Cheats.Info.Deadline", GetDeadline(), $"{GetBuyingRate()}%");

            if (Hack.DisplayCredits.IsEnabled())
                Format("Cheats.Info.Credits", GetCredits());

            Vector2 position = new Vector2(Screen.width - 200, 10);
            VisualUtil.DrawString(position, info, Settings.c_primary, centered: false, forceOnScreen: true, fontSize: Settings.i_screenFontSize);
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
            return LethalMenu.items.FindAll(item => item.isInShipRoom && !item.isHeld && !item.isPocketed && !item.IsDefaultShipItem()).Count();
        }

        private int GetShipValue()
        {
            return LethalMenu.items.FindAll(item => item.isInShipRoom && !item.isHeld && !item.isPocketed && !item.IsDefaultShipItem()).Sum(item => item.scrapValue);
        }

        private int GetObjectValue()
        {
            return LethalMenu.items.FindAll(item => !item.isInShipRoom && !item.isHeld && !item.isPocketed && !item.IsDefaultShipItem()).Sum(item => item.scrapValue);
        }

        private int GetObjectCount()
        {
            return LethalMenu.items.FindAll(item => !item.isInShipRoom && !item.isHeld && !item.isPocketed && !item.IsDefaultShipItem()).Count();
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

        private int GetCredits()
        {
            if (!(bool)LethalMenu.terminal) return 0;
            return LethalMenu.terminal.groupCredits;
        }

        private float GetBuyingRate()
        {
            if (!(bool)StartOfRound.Instance) return 0f;
            return (float)Math.Round(StartOfRound.Instance.companyBuyingRate * 100, 2);
        }
    }
}
