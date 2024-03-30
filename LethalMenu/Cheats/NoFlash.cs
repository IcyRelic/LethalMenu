using HarmonyLib;

namespace LethalMenu.Cheats;

[HarmonyPatch(typeof(StunGrenadeItem), "Update")]
internal class NoFlash : Cheat
{
    [HarmonyPrefix]
    public static void Prefix()
    {
        if (!Hack.NoFlash.IsEnabled()) return;

        HUDManager.Instance.flashbangScreenFilter.weight = 0.0f;
        SoundManager.Instance.earsRingingTimer = 0.0f;
    }
}