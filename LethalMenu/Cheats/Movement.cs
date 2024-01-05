using GameNetcodeStuff;
using HarmonyLib;
using System;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class Movement : Cheat
    {

        public override void Update()
        {
            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
            if (!(bool)StartOfRound.Instance || player == null) return;
            if (Settings.f_defaultMovementSpeed == -1f) Settings.f_defaultMovementSpeed = player.movementSpeed;
  

            player.movementSpeed = Hack.SuperSpeed.IsEnabled() ? Settings.f_movementSpeed : Settings.f_defaultMovementSpeed;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        public static void PlayerLateUpdate(PlayerControllerB __instance)
        {
            if (LethalMenu.localPlayer == null || LethalMenu.localPlayer.playerClientId != __instance.playerClientId) return;
            
            __instance.movementSpeed = Hack.SuperSpeed.IsEnabled() ? Settings.f_movementSpeed : Settings.f_defaultMovementSpeed;

        }

    }
}
