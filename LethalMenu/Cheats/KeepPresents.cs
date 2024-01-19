using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class KeepPresents
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.DestroyObjectInHand))]
        public static bool PlayerDiscardHeldObject(GrabbableObject __instance, PlayerControllerB playerHolding)
        {
            return __instance.GetType() != typeof(GiftBoxItem) && Hack.KeepPresents.IsEnabled();
        }
    }
}
