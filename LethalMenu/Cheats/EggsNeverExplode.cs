using HarmonyLib;

namespace LethalMenu.Cheats
{
    internal class EggsNeverExplode : Cheat
    {
        [HarmonyPatch(typeof(StunGrenadeItem), nameof(StunGrenadeItem.SetExplodeOnThrowClientRpc))]
        public static class SetExplodeOnThrowClientRpcPatch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                if (Hack.EggsNeverExplode.IsEnabled() && LethalMenu.localPlayer?.currentlyHeldObjectServer?.name == "EasterEgg(Clone)" && !Hack.EggsAlwaysExplode.IsEnabled())
                {
                    return false;
                }
                return true;
            }
        }
    }
}
