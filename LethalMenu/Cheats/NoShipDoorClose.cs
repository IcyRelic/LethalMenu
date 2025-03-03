using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using LethalMenu.Handler;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class NoShipDoorClose : Cheat
    {
        public static Dictionary<string, bool> Triggered = new Dictionary<string, bool>();

        [HarmonyPostfix]
        [HarmonyPatch(typeof(InteractTrigger), nameof(InteractTrigger.Interact))]
        public static void Interact(InteractTrigger __instance, Transform playerTransform)
        {
            if (__instance == null || playerTransform == null) return;
            if (__instance.transform == null) return;
            Triggered[__instance.transform.name] = true;
            LethalMenu.Instance.StartCoroutine(WaitForTriggerFinish(__instance, __instance.animationWaitTime));
        }

        private static IEnumerator WaitForTriggerFinish(InteractTrigger __instance, float animationWaitTime)
        {
            yield return new WaitForSeconds(animationWaitTime);
            Triggered[__instance.transform.name] = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "SetShipDoorsOverheatClientRpc")]
        public static bool SetShipDoorsOverheatClientRpc(StartOfRound __instance)
        {
            if (Hack.NoShipDoorClose.IsEnabled() && LethalMenu.localPlayer.IsHost()) return false;
            else if (Hack.NoShipDoorClose.IsEnabled() && !LethalMenu.localPlayer.IsHost())
            {
                HUDManager.Instance.DisplayTip("Lethal Menu", "This is host only :C");
                return true;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HangarShipDoor), "PlayDoorAnimation")]
        public static bool PlayDoorAnimation(bool closed)
        {
            InteractTrigger trigger = LethalMenu.interactTriggers.FirstOrDefault(i => i != null && i.transform.parent != null && i.transform.parent.name == "StartButton");
            if (trigger == null || LethalMenu.shipDoor == null) return false;
            if (LethalMenu.localPlayer && Triggered.TryGetValue(trigger.transform.name, out bool isTriggered) && isTriggered) return true;
            if (Hack.NoShipDoorClose.IsEnabled() && LethalMenu.localPlayer.IsHost()) return false;
            return true;
        }
    }
}
