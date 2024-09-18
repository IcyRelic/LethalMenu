using UnityEngine;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class EnemyDeathNotification : Cheat
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(EnemyAI), "KillEnemyClientRpc")]
        public static void KillEnemyClientRpc(EnemyAI __instance, bool destroy)
        {
            if (!Hack.EnemyDeathNotifications.IsEnabled() || __instance == null || __instance.enemyType == null) return;
            Hack.EnemyDeathNotify.Execute(__instance.enemyType);
        }
    }
}
