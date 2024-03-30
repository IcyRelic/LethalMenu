using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats;

[HarmonyPatch]
internal class UnlimitedStamina : Cheat
{
    public override void Update()
    {
        if (!Hack.UnlimitedStamina.IsEnabled()) return;

        var player = GameNetworkManager.Instance.localPlayerController;
        if (!player) return;
        player.sprintMeter = 1f;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
    public static void PlayerLateUpdate(PlayerControllerB __instance)
    {
        if (LethalMenu.LocalPlayer == null || LethalMenu.LocalPlayer.playerClientId != __instance.playerClientId ||
            !Hack.UnlimitedStamina.IsEnabled()) return;


        __instance.sprintMeter = 1f;
        if (__instance.sprintMeterUI != null) __instance.sprintMeterUI.fillAmount = 1f;
    }
}