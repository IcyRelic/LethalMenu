using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class NoFlash : Cheat
    {
        [HarmonyPatch(typeof(StunGrenadeItem), "Update"), HarmonyPrefix]
        public static void StunGrenadeItemUpdate()
        {
            if (Hack.NoFlash.IsEnabled())
            {
                HUDManager.Instance.flashbangScreenFilter.weight = 0.0f;
                SoundManager.Instance.earsRingingTimer = 0.0f;
            }
        }
    }
}
