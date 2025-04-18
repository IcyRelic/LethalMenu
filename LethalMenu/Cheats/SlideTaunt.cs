using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class SlideTaunt : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "CheckConditionsForEmote"), HarmonyPrefix]
        public static bool CheckConditionsForEmote(PlayerControllerB __instance, ref bool __result)
        {
            if (Hack.SlideTaunt.IsEnabled())
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
