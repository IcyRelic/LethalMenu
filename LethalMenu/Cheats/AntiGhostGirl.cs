using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch(typeof(DressGirlAI), "ChoosePlayerToHaunt")]
    internal class AntiGhostGirl : Cheat
    {
        [HarmonyPostfix]
        public static void ChoosePlayerToHaunt(DressGirlAI __instance)
        {
            if (!Hack.AntiGhostGirl.IsEnabled() || __instance == null || !__instance.hauntingLocalPlayer) return;
            __instance.hauntingPlayer = null;
        }
    }
}