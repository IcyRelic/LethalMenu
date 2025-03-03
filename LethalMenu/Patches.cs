using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Cheats;
using LethalMenu.Manager;
using LethalMenu.Menu.Tab;
using LethalMenu.Util;
using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

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
            MenuUtil.StartLMUser();
            LethalMenu.items.Where(i => i != null && !i.isInShipRoom).ToList().ForEach(i => i.isInShipRoom = true);
        }

        [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Disconnect)), HarmonyPostfix]
        public static void Disconnect(GameNetworkManager __instance)
        {
            ObjectManager.ClearObjects();
            SpectatePlayer.Reset();
            Freecam.Reset();
            LethalMenu.Instance.LMUsers.Clear();
            Shoplifter.Clear();
            ServerTab.UpdatePlayerOptions(true);
        }

        [HarmonyPatch(typeof(StartOfRound), "OnClientConnect"), HarmonyPostfix]
        public static void OnClientConnect(StartOfRound __instance, ulong clientId)
        {
            MenuUtil.StartLMUser();
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

        [HarmonyPatch(typeof(GameNetworkManager), "SteamMatchmaking_OnLobbyCreated"), HarmonyPrefix]
        public static void SteamMatchmaking_OnLobbyCreated(GameNetworkManager __instance, Result result, Lobby lobby)
        {
            if (Settings.b_DisplayLMUsers) __instance.lobbyHostSettings.lobbyName += "\x00AD";
        }

        [HarmonyPatch(typeof(QuickMenuManager), "CloseQuickMenu"), HarmonyPostfix]
        public static void CloseQuickMenu(QuickMenuManager __instance)
        {
            if (Hack.OpenMenu.IsEnabled() && !Cursor.visible) MenuUtil.ShowCursor();
            if (LethalMenu.quickMenuManager == null) ObjectManager.AddToObjectQueue(() => LethalMenu.quickMenuManager = __instance);
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
                yield return instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand.ToString().Equals("12") ?
                    new CodeInstruction(OpCodes.Ldc_I4, int.MaxValue) : instruction;
            }
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

        [HarmonyPatch(typeof(PlayerControllerB), "AllowPlayerDeath"), HarmonyPrefix]
        public static bool AllowPlayerDeath(PlayerControllerB __instance, ref bool __result)
        {
            if (Settings.DebugMode)
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(QuickMenuManager), "Debug_SpawnEnemy"), HarmonyPrefix]
        public static bool Debug_SpawnEnemy(QuickMenuManager __instance)
        {
            if (Settings.DebugMode)
            {
                EnemyType enemyType = null;
                Vector3 spawnPosition = Vector3.zero;
                int enemyTypeId = __instance.Reflect().GetValue<int>("enemyTypeId");
                int enemyToSpawnId = __instance.Reflect().GetValue<int>("enemyToSpawnId");
                int numberEnemyToSpawn = __instance.Reflect().GetValue<int>("numberEnemyToSpawn");
                switch (enemyTypeId)
                {
                    case 0:
                        enemyType = __instance.testAllEnemiesLevel.Enemies[enemyToSpawnId].enemyType;
                        spawnPosition = ((!(StartOfRound.Instance.testRoom != null)) ? RoundManager.Instance.insideAINodes[Random.Range(0, RoundManager.Instance.insideAINodes.Length)].transform.position : __instance.debugEnemySpawnPositions[enemyTypeId].position);
                        break;
                    case 1:
                        enemyType = __instance.testAllEnemiesLevel.OutsideEnemies[enemyToSpawnId].enemyType;
                        spawnPosition = ((!(StartOfRound.Instance.testRoom != null)) ? RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length)].transform.position : __instance.debugEnemySpawnPositions[enemyTypeId].position);
                        break;
                    case 2:
                        enemyType = __instance.testAllEnemiesLevel.DaytimeEnemies[enemyToSpawnId].enemyType;
                        spawnPosition = ((!(StartOfRound.Instance.testRoom != null)) ? RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length)].transform.position : __instance.debugEnemySpawnPositions[enemyTypeId].position);
                        break;
                }
                if (!(enemyType == null))
                {
                    for (int i = 0; i < numberEnemyToSpawn && i <= 50; i++)
                    {
                        if (enemyType.enemyName == "Bush Wolf") RoundHandler.SpawnBushWolf(enemyType);
                        RoundManager.Instance.SpawnEnemyGameObject(spawnPosition, 0f, -1, enemyType);
                    }
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(QuickMenuManager), "Start"), HarmonyPrefix]
        public static bool Start(QuickMenuManager __instance)
        {
            if (Settings.DebugMode)
            {
                __instance.Reflect().SetValue("currentMicrophoneDevice", PlayerPrefs.GetInt("LethalCompany_currentMic", 0));
                __instance.Reflect().Invoke("Debug_SetEnemyDropdownOptions");
                Debug_SetAllItemsDropdownOptions();
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(QuickMenuManager), "Debug_SpawnItem"), HarmonyPrefix]
        public static bool Debug_SpawnItem(QuickMenuManager __instance)
        {
            if (Settings.DebugMode)
            {
                int itemToSpawnId = __instance.Reflect().GetValue<int>("itemToSpawnId");
                Vector3 position = LethalMenu.localPlayer.playerEye.transform.position;
                if (StartOfRound.Instance.allItemsList.itemsList[itemToSpawnId].spawnPrefab == null || StartOfRound.Instance.propsContainer == null || position == null) return true;
                GameObject obj = Object.Instantiate(StartOfRound.Instance.allItemsList.itemsList[itemToSpawnId].spawnPrefab, position, Quaternion.identity, StartOfRound.Instance.propsContainer);
                obj.GetComponent<GrabbableObject>().fallTime = 0f;
                obj.GetComponent<NetworkObject>().Spawn();
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(QuickMenuManager), "Debug_KillLocalPlayer"), HarmonyPrefix]
        public static bool Debug_KillLocalPlayer()
        {
            if (Settings.DebugMode)
            {
                GameNetworkManager.Instance.localPlayerController.KillPlayer(Vector3.zero);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(QuickMenuManager), "Debug_SpawnTruck"), HarmonyPrefix]
        public static bool Debug_SpawnTruck(QuickMenuManager __instance)
        {
            if (Settings.DebugMode)
            {
                Object.Instantiate(__instance.truckPrefab, StartOfRound.Instance.groundOutsideShipSpawnPosition.position, Quaternion.identity, RoundManager.Instance.VehiclesContainer).gameObject.GetComponent<NetworkObject>().Spawn();
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(QuickMenuManager), "Debug_ToggleTestRoom"), HarmonyPrefix]
        public static bool Debug_ToggleTestRoom()
        {
            if (Settings.DebugMode)
            {
                StartOfRound.Instance.Debug_EnableTestRoomServerRpc(StartOfRound.Instance.testRoom == null);
                return false;
            }
            return true;
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

        [HarmonyPatch(typeof(QuickMenuManager), "Debug_ToggleAllowDeath"), HarmonyPrefix]
        public static bool Debug_ToggleAllowDeath()
        {
            if (Settings.DebugMode)
            {
                StartOfRound.Instance.Debug_ToggleAllowDeathServerRpc();
                return false;
            }
            return true;
        }

        public static void Debug_SetAllItemsDropdownOptions()
        {
            if (LethalMenu.quickMenuManager == null) return;
            LethalMenu.quickMenuManager.allItemsDropdown.ClearOptions();
            List<string> list = new List<string>();
            for (int i = 0; i < StartOfRound.Instance.allItemsList.itemsList.Count; i++) list.Add(StartOfRound.Instance.allItemsList.itemsList[i].itemName);
            LethalMenu.quickMenuManager.allItemsDropdown.AddOptions(list);
        }
    }
}
