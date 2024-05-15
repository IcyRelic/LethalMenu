using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Util;
using System.Collections.Generic;

namespace LethalMenu.Cheats
{
    internal class BridgeNeverFalls : Cheat
    {
        [HarmonyPatch(typeof(BridgeTrigger), ("BridgeFallClientRpc"))]
        public static class BridgeFallClientRpcPatch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                if (Hack.BridgeNeverFalls.IsEnabled())
                {
                    return false;
                }
                return true;
            }
        }

        public static class ClearPlayersOnBridge
        {
            public static void Prefix(BridgeTrigger trigger)
            {
                if (Hack.BridgeNeverFalls.IsEnabled() && trigger != null)
                {
                    trigger.bridgeDurability = float.MaxValue;
                    trigger.weightCapacityAmount = float.MaxValue;
                    trigger.playerCapacityAmount = float.MaxValue;
                }
                else
                {
                    trigger.bridgeDurability = 1f;
                    trigger.weightCapacityAmount = 0.04f;
                    trigger.playerCapacityAmount = 0.02f;
                }
            }
        }

        [HarmonyPatch(typeof(BridgeTriggerType2), ("AddToBridgeInstabilityServerRpc"))]
        public static class AddToBridgeInstabilityServerRpcPatch
        {
            [HarmonyPrefix]
            public static bool Prefix()
            {
                if (Hack.BridgeNeverFalls.IsEnabled())
                {
                    return false;
                }
                return true;
            }
        }
    }
}
