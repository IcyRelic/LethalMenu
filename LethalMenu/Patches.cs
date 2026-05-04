using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Cheats;
using LethalMenu.Manager;
using LethalMenu.Menu.Tab;
using LethalMenu.Util;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace LethalMenu
{
    [HarmonyPatch]
    internal class Patches
    {
        public static bool SellQuota = false;

        [HarmonyPatch(typeof(PlayerControllerB), "SendNewPlayerValuesClientRpc"), HarmonyPostfix]
        public static void SendNewPlayerValuesClientRpc(PlayerControllerB __instance)
        {
            if (__instance.IsLocalPlayer) LethalMenu.Instance.StartCoroutine(LocalPlayerJoin());
        }

        private static IEnumerator LocalPlayerJoin()
        {
            yield return new WaitForSeconds(2f);
            LethalMenu.items.Where(i => i != null && !i.isInShipRoom).ToList().ForEach(i => i.isInShipRoom = true);
        }

        [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Disconnect)), HarmonyPostfix]
        public static void Disconnect(GameNetworkManager __instance)
        {
            ObjectManager.ClearObjects();
            SpectatePlayer.Reset();
            Freecam.Reset();
            Shoplifter.Clear();
            ServerTab.UpdatePlayerOptions(true);
        }

        [HarmonyPatch(typeof(StartOfRound), "OnPlayerDC"), HarmonyPostfix]
        public static void OnPlayerDC(StartOfRound __instance, int playerObjectNumber, ulong clientId)
        {
            __instance.allPlayerScripts[clientId].playerSteamId = 0;
        }

        [HarmonyPatch(typeof(GameNetworkManager), "StartClient"), HarmonyPostfix]
        public static void StartClient(SteamId id)
        {
            Settings.s_lobbyid = id;
        }

        [HarmonyPatch(typeof(QuickMenuManager), "CloseQuickMenu"), HarmonyPostfix]
        public static void CloseQuickMenu(QuickMenuManager __instance)
        {
            if (Hack.OpenMenu.IsEnabled() && !Cursor.visible) MenuUtil.ShowCursor();
            if (LethalMenu.quickMenuManager == null) ObjectManager.AddToObjectQueue(() => LethalMenu.quickMenuManager = __instance);
        }

        [HarmonyPatch(typeof(PlayerControllerB), "ScrollMouse_performed"), HarmonyPrefix]
        public static bool ScrollMouse_performed()
        {
            return !Hack.OpenMenu.IsEnabled();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "UseUtilitySlot_performed"), HarmonyPrefix]
        public static bool UseUtilitySlot_performed()
        {
            return !Hack.OpenMenu.IsEnabled();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "ActivateItem_performed"), HarmonyPrefix]
        public static bool ActivateItem_performed()
        {
            return !Hack.OpenMenu.IsEnabled();
        }

        [HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.AttackPlayersServerRpc)), HarmonyPrefix]
        public static void CompanyAttackPrefix(ref bool ___attacking, ref bool ___inGrabbingObjectsAnimation, ref bool __state)
        {
            __state = ___inGrabbingObjectsAnimation;
            ___attacking = false;
            ___inGrabbingObjectsAnimation = false;
        }

        [HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.AttackPlayersServerRpc)), HarmonyPostfix]
        public static void CompanyAttackPostfix(ref bool ___inGrabbingObjectsAnimation, ref bool __state)
        {
            ___inGrabbingObjectsAnimation = __state;
        }

        [HarmonyPatch(typeof(HUDManager), nameof(HUDManager.DisplayCreditsEarning)), HarmonyPostfix]
        public static void DisplayCreditsEarning(HUDManager __instance, int creditsEarned, GrabbableObject[] objectsSold, int newGroupCredits)
        {
            if (SellQuota)
            {
                int total = TimeOfDay.Instance.profitQuota;
                int fulfilled = TimeOfDay.Instance.quotaFulfilled;
                int quotaLeft = total - fulfilled;
                if (quotaLeft > 0) HUDManager.Instance.DisplayTip("Lethal Menu", $"Not enough items to meet quota! {fulfilled}/{total}");
                if (quotaLeft < 0) HUDManager.Instance.DisplayTip("Lethal Menu", $"Quota met! {fulfilled}/{total}");
                SellQuota = false;
            }
        }

        [HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.PlaceItemOnCounter)), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> PlaceItemOnCounter(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand.ToString().Equals("12") ? new CodeInstruction(OpCodes.Ldc_I4, int.MaxValue) : instruction;
            }
        }

        [HarmonyPatch(typeof(StartOfRound), "IsClientFriendsWithHost"), HarmonyPrefix]
        public static bool IsClientFriendsWithHost(ref bool __result)
        {
            if (Settings.DebugMode)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(QuickMenuManager), "CanEnableDebugMenu"), HarmonyPrefix]
        public static bool CanEnableDebugMenu(ref bool __result)
        {
            if (Settings.DebugMode)
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}