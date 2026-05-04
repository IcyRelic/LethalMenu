using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class OpenShipDoorSpace : Cheat
    {
        [HarmonyPatch(typeof(HangarShipDoor), "SetDoorButtonsEnabled"), HarmonyPrefix]
        public static void SetDoorButtonsEnabled(HangarShipDoor __instance, ref bool doorButtonsEnabled)
        {
            if (StartOfRound.Instance.inShipPhase) doorButtonsEnabled = Hack.OpenShipDoorSpace.IsEnabled();
        }

        [HarmonyPatch(typeof(StartOfRound), "TeleportPlayerInShipIfOutOfRoomBounds"), HarmonyPrefix]
        public static bool TeleportPlayerInShipIfOutOfRoomBounds(StartOfRound __instance)
        {
            return !Hack.OpenShipDoorSpace.IsEnabled();
        }
    }
}

