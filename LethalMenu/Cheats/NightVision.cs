using GameNetcodeStuff;
using HarmonyLib;
using System.Numerics;
using UnityEngine;


namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class NightVision : Cheat
    {
        public override void Update()
        {
            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
            if (player == null) return;
            if(Settings.f_nvIntensity == -1f) Settings.f_nvIntensity = player.nightVision.intensity;
            if(Settings.f_nvRange == -1f) Settings.f_nvRange = player.nightVision.range;

            player.nightVision.enabled = Hack.NightVision.IsEnabled() || player.isInsideFactory;
            player.nightVision.intensity = Hack.NightVision.IsEnabled() ? Settings.f_nvIntensity : Settings.f_defaultNightVisionIntensity;
            player.nightVision.range = Hack.NightVision.IsEnabled() ? Settings.f_nvRange : Settings.f_defaultNightVisionRange;
            

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        public static void PlayerLateUpdate(PlayerControllerB __instance)
        {

            //__instance.nightVision.enabled = Hack.NightVision.IsEnabled() || __instance.isInsideFactory;
            //__instance.nightVision.intensity = Hack.NightVision.IsEnabled() ? Settings.f_nvIntensity : Settings.f_defaultNightVisionIntensity;
            //__instance.nightVision.range = Hack.NightVision.IsEnabled() ? Settings.f_nvRange : Settings.f_defaultNightVisionRange;
        }
    }
}
