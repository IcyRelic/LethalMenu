using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class BridgeNeverFalls : Cheat
    {
        [HarmonyPatch(typeof(BridgeTrigger), ("BridgeFallClientRpc")), HarmonyPrefix]
        public static bool BridgeFallClientRpc()
        {
            if (Hack.BridgeNeverFalls.IsEnabled()) return false;
            return true;
        }


        [HarmonyPatch(typeof(BridgeTriggerType2), ("AddToBridgeInstabilityServerRpc")), HarmonyPrefix]
        public static bool AddToBridgeInstabilityServerRpc()
        {
            if (Hack.BridgeNeverFalls.IsEnabled()) return false;
            return true;
        }
    }
}
