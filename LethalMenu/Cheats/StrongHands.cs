using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats;

[HarmonyPatch]
internal class StrongHands : Cheat
{
    public override void Update()
    {
        if (!LethalMenu.LocalPlayer) return;

        var heldObject = LethalMenu.LocalPlayer.currentlyHeldObjectServer;

        LethalMenu.LocalPlayer.twoHanded =
            heldObject && !Hack.StrongHands.IsEnabled() && heldObject.itemProperties.twoHanded;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    public static void PlayerLateUpdate(PlayerControllerB __instance)
    {
        if (!LethalMenu.LocalPlayer ||
            __instance.playerClientId != LethalMenu.LocalPlayer.playerClientId) return;

        var heldObject = __instance.currentlyHeldObjectServer;
        __instance.twoHanded = heldObject && !Hack.StrongHands.IsEnabled() && heldObject.itemProperties.twoHanded;
    }
}