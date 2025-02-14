using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch(typeof(PlayerControllerB), "PlayerJump")]
    internal class SuperJump : Cheat
    {
        [HarmonyPostfix]
        public static void PlayerJump(PlayerControllerB __instance)
        {
            if (LethalMenu.localPlayer == null || __instance == null || LethalMenu.localPlayer != __instance) return;
            __instance.jumpForce = Hack.SuperJump.IsEnabled() ? Settings.f_jumpForce : Settings.f_defaultJumpForce;
        }
    }
}
