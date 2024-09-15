using HarmonyLib;
using UnityEngine.Rendering.HighDefinition;

namespace LethalMenu.Cheats
{
    internal class NoFieldOfDepth : Cheat
    {
        [HarmonyPatch(typeof(DepthOfField), "IsActive")]
        public static class DepthOfFieldIsActivePatch
        {
            [HarmonyPrefix]
            public static bool Prefix(DepthOfField __instance)
            {
                if (Hack.NoFieldOfDepth.IsEnabled())
                {
                    return false;
                }
                return true;
            }
        }
    }
}
