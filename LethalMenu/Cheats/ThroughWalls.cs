using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats
{
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
                LayerMask mask = (LayerMask)LayerMask.GetMask("Props");
                if (Hack.LootThroughWalls.IsEnabled()) mask = (LayerMask)LayerMask.GetMask("Props");
                if (Hack.InteractThroughWalls.IsEnabled()) mask = (LayerMask)LayerMask.GetMask("InteractableObject");
                if (Hack.InteractThroughWalls.IsEnabled() && Hack.LootThroughWalls.IsEnabled()) mask = (LayerMask)LayerMask.GetMask("Props", "InteractableObject");
                __instance.Reflect().SetValue("interactableObjectsMask", mask.value);
            }
            else
            {
                __instance.Reflect().SetValue("interactableObjectsMask", 832);
                if (!Hack.Reach.IsEnabled()) __instance.grabDistance = 5f;
            }
        }
    }
}

