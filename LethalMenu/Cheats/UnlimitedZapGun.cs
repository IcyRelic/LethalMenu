using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class UnlimitedZapGun : Cheat
    {
        [HarmonyPatch(typeof(PatcherTool), "ShiftBendRandomizer"), HarmonyPostfix]
        public static void ShiftBendRandomizer(ref float ___bendMultiplier)
        {
            if (Hack.UnlimitedZapGun.IsEnabled()) ___bendMultiplier = 0f;
        }

        [HarmonyPatch(typeof(GrabbableObject), "RequireCooldown"), HarmonyPostfix]
        public static void RequireCooldown(GrabbableObject __instance)
        {
            if (Hack.UnlimitedZapGun.IsEnabled() && __instance is PatcherTool) __instance.currentUseCooldown = 0f;
        }
    }
}
