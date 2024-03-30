using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats;

[HarmonyPatch]
internal class Movement : Cheat
{
    public override void Update()
    {
        var player = GameNetworkManager.Instance.localPlayerController;
        if (!(bool)StartOfRound.Instance || !player) return;
        if (Mathf.Approximately(Settings.f_defaultMovementSpeed, -1f))
            Settings.f_defaultMovementSpeed = player.movementSpeed;


        player.movementSpeed = Hack.SuperSpeed.IsEnabled() ? Settings.f_movementSpeed : Settings.f_defaultMovementSpeed;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    public static void PlayerLateUpdate(PlayerControllerB __instance)
    {
        if (LethalMenu.LocalPlayer == null ||
            LethalMenu.LocalPlayer.playerClientId != __instance.playerClientId) return;

        __instance.movementSpeed =
            Hack.SuperSpeed.IsEnabled() ? Settings.f_movementSpeed : Settings.f_defaultMovementSpeed;
    }
}