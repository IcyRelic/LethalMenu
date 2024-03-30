using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats;

[HarmonyPatch]
internal class Weight : Cheat
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "Update")]
    public static void WeightUpdate(PlayerControllerB __instance)
    {
        if (LethalMenu.localPlayer == null ||
            __instance.playerClientId != LethalMenu.localPlayer.playerClientId) return;
        __instance.carryWeight = Hack.Weight.IsEnabled() ? 1f : GetHeldWeight(__instance);
    }

    private static float GetHeldWeight(PlayerControllerB player)
    {
        var weight = 1f;

        if (player.ItemSlots == null) return weight;

        foreach (var item in player.ItemSlots)
        {
            if (item == null || item.itemProperties == null) continue;
            weight += Mathf.Clamp(item.itemProperties.weight - 1f, 0.0f, 10f);
        }

        return weight;
    }
}