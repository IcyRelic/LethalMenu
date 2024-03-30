using System.Linq;
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
        if (LethalMenu.LocalPlayer == null ||
            __instance.playerClientId != LethalMenu.LocalPlayer.playerClientId) return;
        __instance.carryWeight = Hack.Weight.IsEnabled() ? 1f : GetHeldWeight(__instance);
    }

    private static float GetHeldWeight(PlayerControllerB player)
    {
        var weight = 1f;

        if (player.ItemSlots == null) return weight;

        weight += player.ItemSlots.Where(item => item != null && item.itemProperties != null)
            .Sum(item => Mathf.Clamp(item.itemProperties.weight - 1f, 0.0f, 10f));

        return weight;
    }
}