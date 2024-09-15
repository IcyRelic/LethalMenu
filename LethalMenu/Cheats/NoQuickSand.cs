using System;
using System.Collections.Generic;
using System.Text;
using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    internal class NoQuickSand : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForSinkingInQuicksand")]
        public static class CheckConditionsForSinkingInQuicksandPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(PlayerControllerB __instance)
            {
                if (Hack.NoQuicksand.IsEnabled())
                {
                    return false;
                }
                return true;
            }
        }
    }
}
