using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class AntiGhostGirl : Cheat
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DressGirlAI), "Update")]
        public static bool PrefixUpdate()
        {
            DressGirlAI dressgirl = Object.FindAnyObjectByType<DressGirlAI>();
            if (Hack.AntiGhostGirl.IsEnabled() && dressgirl.hauntingPlayer == LethalMenu.localPlayer)
            {
                return false;
            }
            return true;
        }
    }
}