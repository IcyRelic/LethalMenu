using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class SuperShovel : Cheat
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Shovel), nameof(Shovel.HitShovel))]
        public static void HitShovel(Shovel __instance)
        {
            __instance.shovelHitForce = Hack.SuperShovel.IsEnabled() ? 1000 : 1;
        }

    }
}
