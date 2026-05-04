using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class NoQuickSand : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForSinkingInQuicksand"), HarmonyPrefix]
        public static bool CheckConditionsForSinkingInQuicksand(PlayerControllerB __instance)
        {
            return !Hack.NoQuicksand.IsEnabled();
        }
    }
}
