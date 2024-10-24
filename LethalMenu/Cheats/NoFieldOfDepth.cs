using HarmonyLib;
using UnityEngine.Rendering.HighDefinition;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class NoFieldOfDepth : Cheat
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DepthOfField), "IsActive")]
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
