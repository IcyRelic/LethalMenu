using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class StrongHands : Cheat
    {

        public override void Update()
        {
            if (LethalMenu.localPlayer == null || LethalMenu.localPlayer.currentlyHeldObjectServer == null) return;

            LethalMenu.localPlayer.twoHanded = Hack.StrongHands.IsEnabled() ? false : LethalMenu.localPlayer.currentlyHeldObjectServer.itemProperties.twoHanded;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        public static void PlayerLateUpdate(PlayerControllerB __instance)
        {
            if (__instance.currentlyHeldObjectServer == null) return;

            __instance.twoHanded = Hack.StrongHands.IsEnabled() ? false : __instance.currentlyHeldObjectServer.itemProperties.twoHanded;
        }
    }
}
