using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch(typeof(PlayerControllerB), "PlayerHitGroundEffects")]
    internal class NoFallDamage : Cheat
    {
        public static bool Prefix(PlayerControllerB __instance)
        {
            if (Hack.NoFallDamage.IsEnabled())
            {
                __instance.takingFallDamage = false;
            }

            return true;
        }
    }
}
