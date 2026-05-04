using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Handler;
using System.Collections.Generic;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class DemiGodCheat : Cheat
    {
        public static readonly List<PlayerControllerB> DemiGodPlayers = new List<PlayerControllerB>();

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer))]
        [HarmonyPatch(typeof(PlayerControllerB), "DamageOnOtherClients")]
        [HarmonyPrefix]
        public static bool DamagePrefix(PlayerControllerB __instance)
        {
            if (DemiGodPlayers.Contains(__instance))
            {
                __instance.Handle().Heal();
                return false;
            }
            return true;
        }

        public static void ToggleDemiGod(PlayerControllerB player)
        {
            if (!DemiGodPlayers.Contains(player)) DemiGodPlayers.Add(player);
            else DemiGodPlayers.Remove(player);
        }
    }
}
