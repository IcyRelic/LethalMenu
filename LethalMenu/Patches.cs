using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Cheats;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace LethalMenu
{
    [HarmonyPatch]
    internal class Patches
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Disconnect))]
        public static void Disconnect(GameNetworkManager __instance)
        {
            LethalMenu.debugMessage2 = "Disconnect Detected";

            SpectatePlayer.Reset();
            LethalMenu.lmUsers.Clear();
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


        [HarmonyPrefix]
        [HarmonyPatch(typeof(HUDManager), "AddTextMessageServerRpc")]
        public static void ReceiveMessage(string chatMessage)
        {
            Debug.Log("Message Received =>" + chatMessage);
            chatMessage = chatMessage.Replace("<size=0>", "").Replace("</size>", "");

            bool isEncrypted = MenuUtil.IsEncrypted(chatMessage);

            if (isEncrypted) MenuUtil.ProcessEncryptedMessage(chatMessage);
        }

    }
}