using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class FullRenderResolution : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "Start"), HarmonyPostfix]
        public static void Start(PlayerControllerB __instance)
        {
            __instance.gameplayCamera.targetTexture.width = Hack.FullRenderResolution.IsEnabled() ? Screen.width : 860;
            __instance.gameplayCamera.targetTexture.height = Hack.FullRenderResolution.IsEnabled() ? Screen.height : 520;
        }
    }
}
