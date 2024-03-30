using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats;

[HarmonyPatch]
internal class GhostMode : Cheat
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(EnemyAI), nameof(EnemyAI.PlayerIsTargetable))]
    public static bool PlayerIsTargetablePrefix(ref bool __result, EnemyAI __instance, PlayerControllerB playerScript,
        bool cannotBeInShip = false, bool overrideInsideFactoryCheck = false)
    {
        if (LethalMenu.LocalPlayer == null || LethalMenu.LocalPlayer.playerClientId != playerScript.playerClientId ||
            !Hack.GhostMode.IsEnabled()) return true;
        __result = false;
        return false;
    }
}