using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class StrongHands : Cheat
    {

        public override void Update()
        {
            if (LethalMenu.localPlayer == null) return;

            var heldObject = LethalMenu.localPlayer.currentlyHeldObjectServer;

            LethalMenu.localPlayer.twoHanded = heldObject == null ? false : Hack.StrongHands.IsEnabled() ? false : heldObject.itemProperties.twoHanded;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        public static void PlayerLateUpdate(PlayerControllerB __instance)
        {
            if(LethalMenu.localPlayer == null || __instance.playerClientId != LethalMenu.localPlayer.playerClientId) return;

            var heldObject = __instance.currentlyHeldObjectServer;
            __instance.twoHanded = heldObject == null ? false : Hack.StrongHands.IsEnabled() ? false : heldObject.itemProperties.twoHanded;
        }
    }
}
