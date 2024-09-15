using HarmonyLib;

namespace LethalMenu.Cheats
{
    internal class VehiclePatches
    {
        [HarmonyPatch(typeof(VehicleController), "DealPermanentDamage")]
        public static class DealPermanentDamagePatch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                if (Hack.VehicleGodMode.IsEnabled())
                {
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(VehicleController), "ReactToDamage")]
        public static class ReactToDamagePatch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                if (Hack.VehicleGodMode.IsEnabled())
                {
                    return false;
                }
                return true;
            }
        }
    }
}
