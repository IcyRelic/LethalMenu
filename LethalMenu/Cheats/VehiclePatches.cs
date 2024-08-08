using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch(typeof(VehicleController), "Update")]
    internal class VehiclePatches
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (LethalMenu.vehicle == null) return;
            if (Hack.VehicleGodMode.IsEnabled())
            {
                LethalMenu.vehicle.AddEngineOilServerRpc(-1, 12);
                LethalMenu.vehicle.carFragility = 0f;
                LethalMenu.vehicle.minimalBumpForce = 0f;
                LethalMenu.vehicle.mediumBumpForce = 0f;
                LethalMenu.vehicle.maximumBumpForce = 0f;
            }
            else
            {
                LethalMenu.vehicle.carFragility = 1f;
                LethalMenu.vehicle.minimalBumpForce = 10f;
                LethalMenu.vehicle.mediumBumpForce = 40f;
                LethalMenu.vehicle.maximumBumpForce = 80f;
            }
        }
    }
}