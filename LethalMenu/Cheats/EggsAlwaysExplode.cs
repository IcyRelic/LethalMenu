using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch(typeof(StunGrenadeItem))]
    internal class EggsAlwaysExplode : Cheat
    {
        private static StunGrenadeItem EasterEgg;

        public override void Update()
        {
            if (Hack.EggsAlwaysExplode.IsEnabled() && LethalMenu.localPlayer?.currentlyHeldObjectServer?.name == "EasterEgg(Clone)" && !Hack.EggsNeverExplode.IsEnabled())
            {
                EasterEgg.SetExplodeOnThrowClientRpc(true);
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch("EquipItem")]
        public static void EquipItem(StunGrenadeItem __instance)
        {
            EasterEgg = __instance;
        }
    }
}
