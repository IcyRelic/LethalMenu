using GameNetcodeStuff;
using HarmonyLib;
namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class SuperJump : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "PlayerJump"), HarmonyPostfix]
        public static void PlayerJump(PlayerControllerB __instance)
        {
            if (LethalMenu.localPlayer == null || __instance == null || LethalMenu.localPlayer != __instance) return;
            __instance.jumpForce = Hack.SuperJump.IsEnabled() ? Settings.f_jumpForce : Settings.f_defaultJumpForce;
        }
    }
}