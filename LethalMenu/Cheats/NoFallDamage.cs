using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class NoFallDamage : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "PlayerHitGroundEffects"), HarmonyPrefix]
        public static bool PlayerHitGroundEffects(PlayerControllerB __instance)
        {
            if (Hack.NoFallDamage.IsEnabled()) __instance.takingFallDamage = false;
            return true;
        }
    }
}
