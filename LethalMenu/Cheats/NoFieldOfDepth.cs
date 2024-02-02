using HarmonyLib;
using UnityEngine.Rendering.HighDefinition;

namespace LethalMenu.Cheats
{
    [HarmonyPatch(typeof(DepthOfField), "IsActive")]
    public static class NoFieldOfDepthPatch
    {
        public static bool Prefix(DepthOfField __instance, ref bool __result)
        {
            if (Hack.NoFieldOfDepth.IsEnabled())
            {
                __result = false;
            }
            else
            {
                __result = true;
            }

            return false;
        }
    }
}
