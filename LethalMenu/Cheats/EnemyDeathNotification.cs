using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class EnemyDeathNotification : Cheat
    {
        [HarmonyPatch(typeof(EnemyAI), "KillEnemyClientRpc"), HarmonyPostfix]
        public static void KillEnemyClientRpc(EnemyAI __instance)
        {
            if (!Hack.EnemyDeathNotifications.IsEnabled() || __instance.enemyType == null) return;
            Hack.EnemyDeathNotify.Execute(__instance.enemyType);
        }
    }
}