using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class NoShipDoorClose : Cheat
    {

        [HarmonyPrefix]
        [HarmonyPatch(typeof(StartOfRound), "SetShipDoorsOverheatClientRpc")]
        public static bool SetShipDoorsOverheatClientRpc(StartOfRound __instance)
        {
            if (Hack.NoShipDoorClose.IsEnabled() && LethalMenu.localPlayer.IsHost)
            {
                return false;
            }
            else if (Hack.NoShipDoorClose.IsEnabled() && !LethalMenu.localPlayer.IsHost)
            {
                HUDManager.Instance.DisplayTip("Lethal Menu", "This is host only :C");
                return true;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HangarShipDoor), "PlayDoorAnimation")]
        public static bool PlayDoorAnimation(bool closed)
        {
            if (LethalMenu.shipDoor != null && LethalMenu.shipDoor.doorPower == 0f && Hack.NoShipDoorClose.IsEnabled() && LethalMenu.localPlayer.IsHost)
            {
                return false;
            }
            return true;
        }
    }
}
