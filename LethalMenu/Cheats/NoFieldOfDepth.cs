using HarmonyLib;
using UnityEngine.Rendering.HighDefinition;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class NoFieldOfDepth : Cheat
    {
        [HarmonyPatch(typeof(DepthOfField), "IsActive"), HarmonyPrefix]
        public static bool IsActive(DepthOfField __instance)
        {
            return !Hack.NoFieldOfDepth.IsEnabled();
        }
    }
}
