using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class ScrapNeverDespawns : Cheat
    {
        [HarmonyPatch(typeof(RoundManager), "DespawnPropsAtEndOfRound")]
        public static class DespawnPropsAtEndOfRoundPatch
        {
            public static bool Prefix(bool despawnAllItems = false)
            {
                if (Hack.ScrapNeverDespawns.IsEnabled())
                {
                  return false;
                }
                return true;
            }
        }
    }
}
