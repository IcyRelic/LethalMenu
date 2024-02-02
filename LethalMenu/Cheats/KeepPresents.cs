using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.DestroyObjectInHand))]
    internal class ExtraPresents : Cheat
    {
        [HarmonyPrefix]
        public static bool PlayerDiscardHeldObject(GrabbableObject __instance, PlayerControllerB playerHolding)
        {
            return __instance.GetType() != typeof(GiftBoxItem) && !Hack.ExtraPresents.IsEnabled();
        }
    }
}
