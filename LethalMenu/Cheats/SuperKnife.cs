using HarmonyLib;

namespace LethalMenu.Cheats
{
    internal class SuperKnife : Cheat
    {
        [HarmonyPatch(typeof(KnifeItem), ("HitKnife"))]
        public static class KnifeItemHitKnifePatch
        {
            [HarmonyPrefix]
            public static void Prefix(KnifeItem __instance)
            {
                __instance.knifeHitForce = Hack.SuperKnife.IsEnabled() ? 1000 : 1;
            }
        }
    }
}
