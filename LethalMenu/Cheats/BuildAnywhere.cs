using HarmonyLib;
using LethalMenu.Util;

namespace LethalMenu.Cheats;

[HarmonyPatch]
internal class BuildAnywhere : Cheat
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ShipBuildModeManager), "Update")]
    public static void ShipBuildModeUpdate(ShipBuildModeManager __instance)
    {
        if (!Hack.BuildAnywhere.IsEnabled()) return;
        var placingObject = (PlaceableShipObject)__instance.Reflect().GetValue("placingObject");

        if (placingObject == null) return;

        placingObject.AllowPlacementOnCounters = true;
        placingObject.AllowPlacementOnWalls = true;
        __instance.Reflect().SetValue("CanConfirmPosition", true);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
    public static bool PlayerMeetsConditions(ShipBuildModeManager __instance, ref bool __result)
    {
        if (Hack.BuildAnywhere.IsEnabled())
        {
            __result = true;
            return false;
        }

        return true;
    }
}