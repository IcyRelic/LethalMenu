using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class UnlimitedStamina : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "Update"), HarmonyPostfix]
        public static void PlayerUpdate(PlayerControllerB __instance)
        {
            if (!Hack.UnlimitedStamina.IsEnabled()) return;
            __instance.sprintMeter = 1f;
        }
    }
}
