using HarmonyLib;
using GameNetcodeStuff;
using System.Reflection;
using LethalMenu.Util;
using UnityEngine;
using System.Linq;

namespace LethalMenu.Cheats
{
    [HarmonyPatch(typeof(GiftBoxItem), nameof(GiftBoxItem.Start))]
    [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.DestroyObjectInHand))]
    internal class UnlimitedPresents : Cheat
    {
        private static bool stuckpresent = false;

        [HarmonyPrefix]
        public static bool PlayerDiscardHeldObject(GrabbableObject __instance)
        {
            if (Hack.UnlimitedPresents.IsEnabled() && __instance.GetType() == typeof(GiftBoxItem))
            {
                stuckpresent = true;
                return false;
            }
            return true;
        }


        [HarmonyPrefix]
        public static void Prefix(GiftBoxItem __instance)
        {
            if (Hack.UnlimitedPresents.IsEnabled() && __instance.GetType() == typeof(GiftBoxItem))
            {
                FieldInfo field = typeof(GiftBoxItem).GetField("hasUsedGift", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null && stuckpresent == true)
                {
                    field.SetValue(__instance, false);
                }
            }

        }
    }
}
