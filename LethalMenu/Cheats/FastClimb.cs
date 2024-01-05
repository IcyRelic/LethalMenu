using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class FastClimb : Cheat
    {
        public override void Update()
        {

            if (LethalMenu.localPlayer == null) return;
            if (Settings.f_defaultClimbSpeed == -1f) Settings.f_defaultClimbSpeed = LethalMenu.localPlayer.climbSpeed;
            LethalMenu.localPlayer.climbSpeed = Hack.FastClimb.IsEnabled() ? Settings.f_climbSpeed : Settings.f_defaultClimbSpeed;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        public static void PlayerLateUpdate(PlayerControllerB __instance)
        {
            if (LethalMenu.localPlayer == null || LethalMenu.localPlayer.playerClientId != __instance.playerClientId) return;
            __instance.climbSpeed = Hack.FastClimb.IsEnabled() ? Settings.f_climbSpeed : Settings.f_defaultClimbSpeed;
        }
    }
}
