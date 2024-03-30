using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats;

[HarmonyPatch]
internal class DeathNotification : Cheat
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc")]
    public static void KillPlayerClientRpcPatch(PlayerControllerB __instance, int playerId, int causeOfDeath)
    {
        if (!Hack.DeathNotifications.IsEnabled()) return;
        var died = __instance.playersManager.allPlayerObjects[playerId].GetComponent<PlayerControllerB>();

        Hack.DeathNotify.Execute(died, (CauseOfDeath)causeOfDeath);
    }
}