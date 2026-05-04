using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class VehiclePatches
    {
        [HarmonyPatch(typeof(VehicleController), "DealPermanentDamage"), HarmonyPrefix]
        public static bool DealPermanentDamage()
        {
            return !Hack.VehicleGodMode.IsEnabled();
        }

        [HarmonyPatch(typeof(VehicleController), "ReactToDamage"), HarmonyPrefix]
        public static bool ReactToDamage()
        {
            return !Hack.VehicleGodMode.IsEnabled();
        }
    }
}
