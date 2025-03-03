using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Util;
using System.Collections.Generic;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    public class LootBeforeGameStarts
    {
        private static Dictionary<GrabbableObject, bool> ModifiedItems = new Dictionary<GrabbableObject, bool>();

        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject"), HarmonyPrefix]
        public static void BeginGrabObject(PlayerControllerB __instance)
        {
            if (!Hack.LootBeforeGameStarts.IsEnabled()) return;
            GrabbableObject grabbableObject = __instance.Reflect().GetValue<GrabbableObject>("currentlyGrabbingObject");
            if (grabbableObject == null || grabbableObject.itemProperties == null || grabbableObject.itemProperties.canBeGrabbedBeforeGameStart || GameNetworkManager.Instance.gameHasStarted) return;
            ModifiedItems.Add(grabbableObject, grabbableObject.itemProperties.canBeGrabbedBeforeGameStart);
            grabbableObject.itemProperties.canBeGrabbedBeforeGameStart = true;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "DiscardHeldObject"), HarmonyPrefix]
        public static void DiscardHeldObject(PlayerControllerB __instance)
        {
            if (ModifiedItems.ContainsKey(__instance.currentlyHeldObjectServer))
            {
                if (ModifiedItems.TryGetValue(__instance.currentlyHeldObjectServer, out var value)) __instance.currentlyHeldObjectServer.itemProperties.canBeGrabbedBeforeGameStart = value;
                ModifiedItems.Remove(__instance.currentlyHeldObjectServer);  
            }
        }
    }
}