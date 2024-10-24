using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class SpamDepositDeskDoor : Cheat
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DepositItemsDesk), "Update")]
        public static void Update(DepositItemsDesk __instance)
        {
            if (Hack.ToggleDepositDeskDoorSound.IsEnabled()) __instance.OpenShutDoorClientRpc();
        }
    }
}
