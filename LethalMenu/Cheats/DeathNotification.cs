using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class DeathNotification : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "KillPlayerClientRpc"), HarmonyPrefix]
        public static void KillPlayerClientRpcPatch(PlayerControllerB __instance, int playerId, int causeOfDeath)
        {
            if(!Hack.DeathNotifications.IsEnabled()) return;
            PlayerControllerB player = __instance.playersManager.allPlayerObjects[playerId].GetComponent<PlayerControllerB>();
            if (player == null) return;
            Hack.DeathNotify.Execute(player, ((CauseOfDeath) causeOfDeath));
        }
    }
}
