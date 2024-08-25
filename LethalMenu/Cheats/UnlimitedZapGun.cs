using HarmonyLib;

namespace LethalMenu.Cheats
{
    internal class UnlimitedZapGun : Cheat
    {
        [HarmonyPatch(typeof(PatcherTool), "ShiftBendRandomizer")]
        public class ShiftBendRandomizerPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref float ___bendMultiplier)
            {
                if (Hack.UnlimitedZapGun.IsEnabled()) ___bendMultiplier = 0f;
            }
        }

        [HarmonyPatch(typeof(GrabbableObject), "RequireCooldown")]
        public class RequireCooldownPatch
        {
            [HarmonyPostfix]
            public static void Postfix(GrabbableObject __instance)
            {
                if (Hack.UnlimitedZapGun.IsEnabled() && __instance is PatcherTool) __instance.currentUseCooldown = 0f;
            }
        }
    }
}
