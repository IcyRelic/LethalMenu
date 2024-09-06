using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Cheats;
using LethalMenu.Menu.Tab;
using LethalMenu.Util;
using Steamworks;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.Rendering;
using Debug = UnityEngine.Debug;


namespace LethalMenu
{
    [HarmonyPatch]
    internal class Patches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Disconnect))]
        public static void Disconnect(GameNetworkManager __instance)
        {   
            SpectatePlayer.Reset();
            Freecam.Reset();
            LethalMenu.Instance.LMUsers.Clear();
            Shoplifter.Clear();
            ServerTab.ClearPlayerOptions();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "SendNewPlayerValuesClientRpc")]
        public static void SendNewPlayerValuesClientRpc(PlayerControllerB __instance)
        {
            MenuUtil.RunLMUser();
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), "OnClientConnect")]
        public static void OnClientConnect(StartOfRound __instance)
        {
            MenuUtil.RunLMUser();
            
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartClient")]
        public static void StartClient(SteamId id)
        {
            Settings.s_lobbyid = id;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.AttackPlayersServerRpc))]
        public static void CompanyAttackPrefix(ref bool ___attacking, ref bool ___inGrabbingObjectsAnimation, ref bool __state)
        {
            __state = ___inGrabbingObjectsAnimation;
            ___attacking = false;
            ___inGrabbingObjectsAnimation = false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.AttackPlayersServerRpc))]
        public static void CompanyAttackPostfix(ref bool ___inGrabbingObjectsAnimation, ref bool __state)
        {
            ___inGrabbingObjectsAnimation = __state;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.PlaceItemOnCounter))]
        public static IEnumerable<CodeInstruction> CompanyInfiniteSell(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand.ToString().Equals("12") ?
                    new CodeInstruction(OpCodes.Ldc_I4, int.MaxValue) : instruction;
            }
        } 
    }
}
