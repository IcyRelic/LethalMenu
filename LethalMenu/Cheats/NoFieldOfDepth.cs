using HarmonyLib;
using UnityEngine.Rendering.HighDefinition;

namespace LethalMenu.Cheats;

[HarmonyPatch(typeof(DepthOfField), "IsActive")]
public static class NoFieldOfDepth
{
    public static bool Prefix(DepthOfField __instance, ref bool __result)
    {
        __result = !Hack.NoFieldOfDepth.IsEnabled();

        return false;
    }
}