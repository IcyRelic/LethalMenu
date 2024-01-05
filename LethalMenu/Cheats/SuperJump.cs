using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class SuperJump : Cheat
    {
        public override void Update()
        {
            
            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
            if (player == null) return;
            if (Settings.f_defaultJumpForce == -1f) Settings.f_defaultJumpForce = player.jumpForce;
            player.jumpForce = Hack.SuperJump.IsEnabled() ? Settings.f_jumpForce : Settings.f_defaultJumpForce;

            
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        public static void PlayerLateUpdate(PlayerControllerB __instance)
        {
            if (LethalMenu.localPlayer == null || LethalMenu.localPlayer.playerClientId != __instance.playerClientId) return;
            if (!Hack.SuperJump.IsEnabled()) return;
            __instance.jumpForce = Hack.SuperJump.IsEnabled() ? Settings.f_jumpForce : Settings.f_defaultJumpForce;
        }
    }
}
