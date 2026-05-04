using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class UnlimitedOxygen
    {
        [HarmonyPatch(typeof(StartOfRound), "SetFaceUnderwaterFilters"), HarmonyPrefix]
        public static void SetFaceUnderwaterFilters(StartOfRound __instance)
        {
            if (Hack.UnlimitedOxygen.IsEnabled()) __instance.drowningTimer = 1f;
        }
    }
}
