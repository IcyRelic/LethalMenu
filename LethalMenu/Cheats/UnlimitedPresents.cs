using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class UnlimitedPresents
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.DestroyObjectInHand))]
        public static bool PlayerDiscardHeldObject(GrabbableObject __instance, PlayerControllerB playerHolding)
        {
            return true;
            //return __instance.GetType() != typeof(GiftBoxItem) && Hack.UnlimitedPresents.IsEnabled();

        }
    }
}
