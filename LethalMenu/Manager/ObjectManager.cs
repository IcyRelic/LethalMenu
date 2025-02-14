using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System;
using UnityEngine;
using Steamworks.Ugc;
using Object = UnityEngine.Object;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using static UnityEngine.EventSystems.EventTrigger;
using System.Collections;

namespace LethalMenu.Manager
{
    [HarmonyPatch]
    public class ObjectManager
    {
        [HarmonyPatch(typeof(QuickMenuManager), nameof(QuickMenuManager.AddUserToPlayerList)), HarmonyPrefix]
        public static void AddUserToPlayerList(QuickMenuManager __instance)
        {
            if (LethalMenu.quickMenuManager == null) AddToQueue(() => LethalMenu.quickMenuManager = __instance);
            if (LethalMenu.localPlayer == null) AddToQueue(() => LethalMenu.localPlayer = GameNetworkManager.Instance?.localPlayerController);
            if (LethalMenu.localPlayer != null && !LethalMenu.players.Contains(LethalMenu.localPlayer)) AddToQueue(() => LethalMenu.players.Add(LethalMenu.localPlayer));
        }
        [HarmonyPatch(typeof(PlayerControllerB), "Awake"), HarmonyPrefix]
        public static void PlayerControllerBAwake(PlayerControllerB __instance) => AddToQueue(() => LethalMenu.players.Add(__instance));
        [HarmonyPatch(typeof(TerminalAccessibleObject), "Start"), HarmonyPrefix]
        public static void TerminalAccessibleObjectStart(TerminalAccessibleObject __instance)
        {
            AddToQueue(() => LethalMenu.allTerminalObjects.Add(__instance));
            if (__instance.isBigDoor) AddToQueue(() => LethalMenu.bigDoors.Add(__instance));
        }
        [HarmonyPatch(typeof(EnemyAI), "SubtractFromPowerLevel"), HarmonyPostfix]
        public static void SubtractFromPowerLevel(EnemyAI __instance) => AddToQueue(() => LethalMenu.enemies.Remove(__instance));
        [HarmonyPatch(typeof(NetworkObject), nameof(NetworkObject.Despawn)), HarmonyPrefix]
        public static void Despawn(NetworkObject __instance)
        {
            if (__instance.TryGetComponent<EnemyAI>(out EnemyAI enemy)) AddToQueue(() => LethalMenu.enemies.Remove(enemy));
        }
        [HarmonyPatch(typeof(BreakerBox), "Start"), HarmonyPrefix]
        public static void BreakerBoxStart(BreakerBox __instance) => AddToQueue(() => LethalMenu.breaker = __instance);
        [HarmonyPatch(typeof(GrabbableObject), "Start"), HarmonyPrefix]
        public static void GrabbableObjectStart(GrabbableObject __instance) => AddToQueue(() => LethalMenu.items.Add(__instance));
        [HarmonyPatch(typeof(Turret), "Start"), HarmonyPrefix]
        public static void TurretStart(Turret __instance) => AddToQueue(() => LethalMenu.turrets.Add(__instance));
        [HarmonyPatch(typeof(Landmine), "Start"), HarmonyPrefix]
        public static void LandmineStart(Landmine __instance) => AddToQueue(() => LethalMenu.landmines.Add(__instance));
        [HarmonyPatch(typeof(DoorLock), "Awake"), HarmonyPrefix]
        public static void DoorLockAwake(DoorLock __instance) => AddToQueue(() => LethalMenu.doorLocks.Add(__instance));
        [HarmonyPatch(typeof(EnemyAI), "Start"), HarmonyPrefix]
        public static void EnemyAIStart(EnemyAI __instance) => AddToQueue(() => LethalMenu.enemies.Add(__instance));
        [HarmonyPatch(typeof(SteamValveHazard), "Start"), HarmonyPrefix]
        public static void SteamValveHazardStart(SteamValveHazard __instance) => AddToQueue(() => LethalMenu.steamValves.Add(__instance));
        [HarmonyPatch(typeof(ShipTeleporter), "Awake"), HarmonyPrefix]
        public static void ShipTeleporterAwake(ShipTeleporter __instance) => AddToQueue(() => LethalMenu.teleporters.Add(__instance));
        [HarmonyPatch(typeof(InteractTrigger), "Start"), HarmonyPrefix]
        public static void InteractTriggerStart(InteractTrigger __instance) => AddToQueue(() => LethalMenu.interactTriggers.Add(__instance));
        [HarmonyPatch(typeof(SpikeRoofTrap), "Start"), HarmonyPrefix]
        public static void SpikeRoofTrapStart(SpikeRoofTrap __instance) => AddToQueue(() => LethalMenu.spikeRoofTraps.Add(__instance));
        [HarmonyPatch(typeof(AnimatedObjectTrigger), "Start"), HarmonyPrefix]
        public static void AnimatedObjectTriggerStart(AnimatedObjectTrigger __instance) => AddToQueue(() => LethalMenu.animatedTriggers.Add(__instance));
        [HarmonyPatch(typeof(EnemyVent), "Start"), HarmonyPrefix]
        public static void EnemyVentStart(EnemyVent __instance) => AddToQueue(() => LethalMenu.enemyVents.Add(__instance));
        [HarmonyPatch(typeof(MoldSpreadManager), nameof(MoldSpreadManager.GenerateMold)), HarmonyPostfix]
        public static void GenerateMold(MoldSpreadManager __instance) => AddToQueue(() => LethalMenu.vainShrouds = __instance.generatedMold);
        [HarmonyPatch(typeof(HangarShipDoor), "Start"), HarmonyPrefix]
        public static void HangarShipDoorStart(HangarShipDoor __instance) => AddToQueue(() => LethalMenu.shipDoor = __instance);
        [HarmonyPatch(typeof(MineshaftElevatorController), "OnEnable"), HarmonyPrefix]
        public static void MineshaftElevatorControllerOnEnable(MineshaftElevatorController __instance) => AddToQueue(() => LethalMenu.mineshaftElevator = __instance);
        [HarmonyPatch(typeof(ItemDropship), "Start"), HarmonyPrefix]
        public static void ItemDropshipStart(ItemDropship __instance) => AddToQueue(() => LethalMenu.itemDropship = __instance);
        [HarmonyPatch(typeof(VehicleController), "Awake"), HarmonyPrefix]
        public static void VehicleControllerAwake(VehicleController __instance) => AddToQueue(() => LethalMenu.vehicles.Add(__instance));
        [HarmonyPatch(typeof(EntranceTeleport), "Awake"), HarmonyPrefix]
        public static void EntranceTeleportAwake(EntranceTeleport __instance) => AddToQueue(() => LethalMenu.doors.Add(__instance));
        [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Disconnect)), HarmonyPostfix]
        public static void Disconnect(GameNetworkManager __instance) => ClearObjects();

        public static void CollectObjects()
        {
            CollectObjects(LethalMenu.items);
            CollectObjects(LethalMenu.landmines);
            CollectObjects(LethalMenu.turrets);
            CollectObjects(LethalMenu.doors);
            CollectObjects(LethalMenu.players);
            CollectObjects(LethalMenu.enemies);
            CollectObjects(LethalMenu.steamValves);
            CollectObjects(LethalMenu.allTerminalObjects);
            CollectObjects(LethalMenu.teleporters);
            CollectObjects(LethalMenu.interactTriggers);
            CollectObjects(LethalMenu.bigDoors, obj => obj.isBigDoor);
            CollectObjects(LethalMenu.doorLocks);
            CollectObjects(LethalMenu.spikeRoofTraps);
            CollectObjects(LethalMenu.animatedTriggers);
            CollectObjects(LethalMenu.vehicles);
            CollectObjects(LethalMenu.enemyVents);
            LethalMenu.localPlayer = GameNetworkManager.Instance?.localPlayerController;
            LethalMenu.itemDropship = Object.FindAnyObjectByType<ItemDropship>();
            LethalMenu.breaker = Object.FindAnyObjectByType<BreakerBox>();
            LethalMenu.shipDoor = Object.FindAnyObjectByType<HangarShipDoor>();
            LethalMenu.mineshaftElevator = Object.FindAnyObjectByType<MineshaftElevatorController>();
            LethalMenu.vainShrouds = Object.FindAnyObjectByType<MoldSpreadManager>()?.generatedMold;
            LethalMenu.quickMenuManager = Object.FindAnyObjectByType<QuickMenuManager>();
        }

        private static void CollectObjects<T>(List<T> list, Func<T, bool> filter = null) where T : MonoBehaviour
        {
            list.AddRange(filter == null ? Object.FindObjectsOfType<T>() : Object.FindObjectsOfType<T>().Where(filter));
        }

        private static void ClearObjects()
        {
            LethalMenu.items.Clear();
            LethalMenu.landmines.Clear();
            LethalMenu.turrets.Clear();
            LethalMenu.doors.Clear();
            LethalMenu.players.Clear();
            LethalMenu.enemies.Clear();
            LethalMenu.steamValves.Clear();
            LethalMenu.allTerminalObjects.Clear();
            LethalMenu.teleporters.Clear();
            LethalMenu.interactTriggers.Clear();
            LethalMenu.bigDoors.Clear();
            LethalMenu.doorLocks.Clear();
            LethalMenu.spikeRoofTraps.Clear();
            LethalMenu.animatedTriggers.Clear();
            LethalMenu.enemyVents.Clear();
            LethalMenu.vainShrouds.Clear();
            LethalMenu.vehicles.Clear();
            LethalMenu.shipDoor = null;
            LethalMenu.breaker = null;
            LethalMenu.mineshaftElevator = null;
            LethalMenu.itemDropship = null;
            LethalMenu.localPlayer = null;
            LethalMenu.quickMenuManager = null;
        }

        private static void AddToQueue(Action action) => LethalMenu.Instance.StartCoroutine(DelayedAction(action));

        private static IEnumerator DelayedAction(Action action)
        {
            yield return new WaitForSeconds(0.1f);
            action?.Invoke();
        }
    }
}
