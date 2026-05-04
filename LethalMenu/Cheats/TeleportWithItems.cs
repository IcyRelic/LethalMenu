using GameNetcodeStuff;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class TeleportWithItems : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "DropAllHeldItems"), HarmonyPrefix]
        public static bool DropAllHeldItems(PlayerControllerB __instance)
        {
            return !(Hack.TeleportWithItems.IsEnabled() && !Settings.b_DropItems);
        }
    }
}