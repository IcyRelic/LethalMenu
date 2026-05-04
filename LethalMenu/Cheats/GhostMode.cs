using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class GhostMode : Cheat
    {
        [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable)), HarmonyPrefix]
        public static bool PlayerIsTargetable(PlayerControllerB playerScript, ref bool __result)
        {
            if (Hack.GhostMode.IsEnabled() && LethalMenu.localPlayer?.playerClientId == playerScript.playerClientId)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(PumaAI), nameof(PumaAI.PlayerIsTargetable)), HarmonyPrefix]
        public static bool PumaAIPlayerIsTargetable(PlayerControllerB playerScript, ref bool __result)
        {
            if (Hack.GhostMode.IsEnabled() && LethalMenu.localPlayer?.playerClientId == playerScript.playerClientId)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
