using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class AntiGhostGirl : Cheat
    {
        [HarmonyPatch(typeof(DressGirlAI), "ChoosePlayerToHaunt"), HarmonyPostfix]
        public static void ChoosePlayerToHaunt(DressGirlAI __instance)
        {
            if (!Hack.AntiGhostGirl.IsEnabled() || __instance == null || !__instance.hauntingLocalPlayer) return;
            __instance.hauntingPlayer = null;
        }

        [HarmonyPatch(typeof(DressGirlAI), "BeginChasing"), HarmonyPostfix]
        public static void BeginChasing(DressGirlAI __instance)
        {
            if (!Hack.AntiGhostGirl.IsEnabled() || __instance == null || !__instance.hauntingLocalPlayer) return;
            __instance.hauntingPlayer = null;
        }
    }
}