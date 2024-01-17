using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch(typeof(PlayerControllerB), "PlayerHitGroundEffects")]
    internal class NoFallDamage : Cheat
    {
        public static bool Prefix(PlayerControllerB instance)
        {
            if (Hack.NoFallDamage.IsEnabled())
            {
                instance.takingFallDamage = false;
            }

            return true;
        }
    }
}
