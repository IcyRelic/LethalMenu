using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class GhostMode : Cheat
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable))]
        public static bool PlayerIsTargetablePrefix(ref bool __result, EnemyAI __instance, PlayerControllerB playerScript, bool cannotBeInShip = false, bool overrideInsideFactoryCheck = false)
        {
            if (LethalMenu.localPlayer != null && LethalMenu.localPlayer.playerClientId == playerScript.playerClientId && Hack.GhostMode.IsEnabled())
            {
                __result = false;
                return false;
            }
            return true;
        }


    }
}
