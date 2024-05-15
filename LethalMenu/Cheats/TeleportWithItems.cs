using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    internal class TeleportWithItems : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "DropAllHeldItems")]
        public static class PlayerControllerBDropAllHeldItemsPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(PlayerControllerB __instance)
            {
                if (Hack.TeleportWithItems.IsEnabled() && !Settings.b_DropItems)
                {
                    return false;
                }
                return true;
            }
        }
    }
}