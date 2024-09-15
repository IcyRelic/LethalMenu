using HarmonyLib;

namespace LethalMenu.Cheats
{
    internal class EggsNeverExplode : Cheat
    {
        [HarmonyPatch(typeof(StunGrenadeItem), nameof(StunGrenadeItem.SetExplodeOnThrowServerRpc))]
        public static class SetExplodeOnThrowServerRpcPatch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                if (Hack.EggsNeverExplode.IsEnabled())
                {
                    return false;
                }
                return true;
            }
        }
    }
}
