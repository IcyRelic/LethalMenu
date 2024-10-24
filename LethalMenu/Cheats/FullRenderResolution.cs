using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class FullRenderResolution : Cheat
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "Start")]
        public static void Start(PlayerControllerB __instance)
        {
            __instance.gameplayCamera.targetTexture.width = Hack.FullRenderResolution.IsEnabled() ? Screen.width : 860;
            __instance.gameplayCamera.targetTexture.height = Hack.FullRenderResolution.IsEnabled() ? Screen.height : 520;
        }
    }
}
