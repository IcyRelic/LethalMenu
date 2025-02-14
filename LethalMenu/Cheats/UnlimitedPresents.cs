using HarmonyLib;
using UnityEngine;
using LethalMenu.Util;
using UnityEngine.InputSystem;
using System.Reflection;
using System;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class UnlimitedPresents : Cheat
    {
        private static bool stuckpresent = false;

        public override void Update()
        {
            GiftBoxItem __instance = new();
            if (Hack.UnlimitedPresents.IsEnabled() && LethalMenu.localPlayer.currentlyHeldObjectServer is GiftBoxItem && stuckpresent && Keyboard.current.gKey.isPressed)
            {
                Hack.DeleteHeldItem.Execute();
                Hack.DropAllItems.Execute();
                stuckpresent = false;
            }
        }

        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.DestroyObjectInHand)), HarmonyPrefix]
        public static bool DestroyObjectInHand()
        {
            if (Hack.UnlimitedPresents.IsEnabled() && LethalMenu.localPlayer.currentlyHeldObjectServer is GiftBoxItem)
            {
                stuckpresent = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(GiftBoxItem), nameof(GiftBoxItem.Start)), HarmonyPostfix]
        public static void Start(GiftBoxItem __instance)
        {
            if (Hack.UnlimitedPresents.IsEnabled() && LethalMenu.localPlayer.currentlyHeldObjectServer is GiftBoxItem && stuckpresent) __instance.Reflect().SetValue("hasUsedGift", false);
        }
    }
}
