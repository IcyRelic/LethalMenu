using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats;

[HarmonyPatch]
internal class ThroughWalls : Cheat
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    public static void PlayerLateUpdate(PlayerControllerB __instance)
    {
        if (Hack.LootThroughWalls.IsEnabled() || Hack.InteractThroughWalls.IsEnabled())
        {
            __instance.grabDistance = 10000f;
            var mask = (LayerMask)LayerMask.GetMask("Props");
            if (Hack.LootThroughWalls.IsEnabled()) mask = LayerMask.GetMask("Props");
            if (Hack.InteractThroughWalls.IsEnabled()) mask = LayerMask.GetMask("InteractableObject");
            if (Hack.InteractThroughWalls.IsEnabled() && Hack.LootThroughWalls.IsEnabled())
                mask = LayerMask.GetMask("Props", "InteractableObject");
            typeof(PlayerControllerB)
                .GetField("interactableObjectsMask",
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(__instance, mask.value);
        }
        else
        {
            typeof(PlayerControllerB)
                .GetField("interactableObjectsMask",
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                ?.SetValue(__instance, 832);
            if (!Hack.Reach.IsEnabled())
                __instance.grabDistance = 5f;
        }
    }
}