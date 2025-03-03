using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class SpamDepositDeskDoor : Cheat
    {
        [HarmonyPatch(typeof(DepositItemsDesk), "Update"), HarmonyPostfix]
        public static void Update(DepositItemsDesk __instance)
        {
            if (Hack.ToggleDepositDeskDoorSound.IsEnabled()) __instance.OpenShutDoorClientRpc();
        }
    }
}
