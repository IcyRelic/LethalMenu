using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class OpenDropShipLand : Cheat
    {
        [HarmonyPatch(typeof(ItemDropship), "ShipLandedAnimationEvent"), HarmonyPostfix]
        public static void ShipLandedAnimationEvent(ItemDropship __instance)
        {
            if (!Hack.OpenDropShipLand.IsEnabled() || __instance == null || __instance.shipDoorsOpened) return;
            __instance.OpenShipServerRpc();
        }
    }
}
