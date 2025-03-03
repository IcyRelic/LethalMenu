using GameNetcodeStuff;
using LethalMenu.Handler;
using LethalMenu.Types;
using LethalMenu.Util;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

namespace LethalMenu.Manager
{
    public enum ActionType {
        Add = 0,
        Remove = 1,
        Set = 2
    }

    public class RoundHandler
    {
        public static void ModCredits(int amount, ActionType type)
        {
            if (LethalMenu.terminal == null) return;
            int newAmt = amount;
            if (type == ActionType.Add) newAmt = LethalMenu.terminal.groupCredits + amount;
            if (type == ActionType.Remove) newAmt = LethalMenu.terminal.groupCredits - amount;
            LethalMenu.terminal.SyncGroupCreditsServerRpc(newAmt, LethalMenu.terminal.numberOfItemsInDropship);
        }

        public static void SetQuota(int amount)
        {
            if (TimeOfDay.Instance == null) return;
            TimeOfDay.Instance.profitQuota = amount;
            TimeOfDay.Instance.SyncNewProfitQuotaClientRpc(TimeOfDay.Instance.profitQuota, 0, TimeOfDay.Instance.timesFulfilledQuota);
        }

        public static void SetDeadline(int amount)
        {
            if (TimeOfDay.Instance == null || HUDManager.Instance == null || !LethalMenu.localPlayer.IsHost()) return;
            if (!StartOfRound.Instance.inShipPhase)
            {
                HUDManager.Instance.DisplayTip("Lethal Menu", "You must be in orbit!");
                return;
            }
            TimeOfDay.Instance.timeUntilDeadline = TimeOfDay.Instance.totalTime * amount;
            TimeOfDay.Instance.UpdateProfitQuotaCurrentTime();
            TimeOfDay.Instance.SyncTimeClientRpc(TimeOfDay.Instance.globalTime, (int)TimeOfDay.Instance.timeUntilDeadline);
        }

        public static void Message(string msg, int type, int id)
        {
            if (HUDManager.Instance == null || string.IsNullOrEmpty(msg)) return;
            if (type == 4)
            {
                if (HUDManager.Instance.lastChatMessage == msg) msg += "\u200B";
                HUDManager.Instance.AddTextToChatOnServer(msg, id);
            }
            if (type == 3 && !Unlockable.SignalTranslator.GetItem().hasBeenUnlockedByPlayer)
            {
                BuyUnlockable(Unlockable.SignalTranslator, false);
                HUDManager.Instance.UseSignalTranslatorServerRpc(msg);
            }
            else if (type == 3 && Unlockable.SignalTranslator.GetItem().hasBeenUnlockedByPlayer) HUDManager.Instance.UseSignalTranslatorServerRpc(msg);
            if (type > 2) return;
            string[] prefixes = { "[System]", "[Server]", "[Broadcast]" };
            string fmsg = $"{prefixes[type]} {msg}";
            if (HUDManager.Instance.lastChatMessage == fmsg) fmsg += "\u200B";
            HUDManager.Instance.AddTextToChatOnServer(fmsg);
        }

        public static void BuyUnlockable(Unlockable unlockable, bool enabled = true)
        {
            if (StartOfRound.Instance == null || HUDManager.Instance == null || LethalMenu.terminal == null) return;
            unlockable.Buy(LethalMenu.terminal.groupCredits);
            if (enabled) HUDManager.Instance.DisplayTip("Lethal Menu", $"Unlocked {unlockable.GetItem().unlockableName}!");
        }

        public static bool ToggleShipLights()
        {
            if (!(bool)StartOfRound.Instance) return false;
            ShipLights lights = Object.FindObjectOfType<ShipLights>();
            lights.ToggleShipLights();
            return lights.areLightsOn;
        }

        public static bool ToggleFactoryLights()
        {
            if (!(bool)StartOfRound.Instance || !(bool)RoundManager.Instance || LethalMenu.breaker == null) return false;
            if (RoundManager.Instance.powerOffPermanently) RoundManager.Instance.powerOffPermanently = false;
            RoundManager.Instance.SwitchPower(!LethalMenu.breaker.isPowerOn);
            return LethalMenu.breaker.isPowerOn;
        }

        public static bool AreFactoryLightsOn()
        {
            if (!(bool)StartOfRound.Instance || LethalMenu.breaker == null) return false;
            return LethalMenu.breaker.isPowerOn;
        }

        public static bool AreShipLightsOn()
        {
            if (!(bool)StartOfRound.Instance) return false;
            ShipLights lights = Object.FindObjectOfType<ShipLights>();
            return lights.areLightsOn;
        }

        public static void StartGame() => StartOfRound.Instance?.StartGameServerRpc();

        public static void EndGame() => StartOfRound.Instance?.EndGameServerRpc(-1);

        public static void ResetShip()
        {
            if (!(bool)StartOfRound.Instance) return;
            if (!StartOfRound.Instance.inShipPhase)
            {
                HUDManager.Instance.DisplayTip("Lethal Menu", $"You must be in orbit!");
                return;
            }
            StartOfRound.Instance.ResetShip();
        }

        public static void SpawnScrap()
        {
            if (RoundManager.Instance == null || RoundManager.Instance.currentLevel == null || !RoundManager.Instance.currentLevel.spawnEnemiesAndScrap) return;
            RoundManager.Instance.SpawnScrapInLevel();
        }

        public static void ModScrap(int value, int type)
        {
            if (!(bool)StartOfRound.Instance || !(bool)RoundManager.Instance) return;
            if (type == 0) RoundManager.Instance.scrapAmountMultiplier = value;
            if (type == 1) RoundManager.Instance.scrapValueMultiplier = value;
        }

        public static void ForceTentacleAttack()
        {
            if (LethalMenu.depositItemsDesk == null) return;
            LethalMenu.depositItemsDesk.AttackPlayersServerRpc();
        }

        public static void ForceBridgeFall()
        {
            BridgeTrigger trigger = Object.FindObjectOfType(typeof(BridgeTrigger)) as BridgeTrigger;
            if (trigger == null) return;
            trigger.BridgeFallServerRpc();
        }

        public static void ForceSmallBridgeFall()
        {
            BridgeTriggerType2 trigger = Object.FindObjectOfType(typeof(BridgeTriggerType2)) as BridgeTriggerType2;
            if (trigger == null) return;
            for (int i = 0; i < 4; i++) trigger.AddToBridgeInstabilityServerRpc();
        }

        public static void SpawnHoardingBugInfestation()
        {
            if (RoundManager.Instance == null || !LethalMenu.localPlayer.IsHost()) return;
            for (int i = 0; i < RoundManager.Instance.currentLevel.Enemies.Count; i++)
            {
                if (RoundManager.Instance.currentLevel.Enemies.Any(e => e.enemyType.enemyName == "Hoarding bug")) SpawnEnemy(GameUtil.GetEnemyTypes().ToList().FirstOrDefault(e => e.enemyName == "Hoarding bug"), 1, false);
                else HUDManager.Instance.DisplayTip("Lethal Menu", "There must be one Hoarding bug spawned");
                if (RoundManager.Instance.currentLevel.Enemies[i].enemyType.enemyName == "Hoarding bug")
                {
                    RoundManager.Instance.Reflect().SetValue("enemyRushIndex", i);
                    RoundManager.Instance.currentMaxInsidePower = 30f;
                }
            }
        }

        public static void KillAll()
        {
            if (HUDManager.Instance == null) return;
            if (LethalMenu.enemies.Count == 0)
            {
                HUDManager.Instance.DisplayTip("Lethal Menu", "No enemies to kill!");
                return;
            }
            HUDManager.Instance.DisplayTip("Lethal Menu", $"Killed {LethalMenu.enemies.Count} enemies!");
            LethalMenu.enemies.Where(e => e != null && !e.isEnemyDead).ToList().ForEach(e => e.Handle().Kill(false, true));
        }

        public static void StunAll()
        {
            if (HUDManager.Instance == null) return;
            if (LethalMenu.enemies.Count == 0)
            {
                HUDManager.Instance.DisplayTip("Lethal Menu", "No enemies to stun!");
                return;
            }
            HUDManager.Instance.DisplayTip("Lethal Menu", $"Stunned {LethalMenu.enemies.Count} enemies!");
            LethalMenu.enemies.Where(e => e != null && !e.isEnemyDead).ToList().ForEach(e => e.Handle().Stun(true));
        }

        public static void TeleportAllItems()
        {
            LethalMenu.items.FindAll(i => i != null && !i.isHeld && !i.isPocketed && !i.isInShipRoom && !i.heldByPlayerOnServer).ForEach(i =>
            {
                Vector3 point = new Ray(LethalMenu.localPlayer.gameplayCamera.transform.position, LethalMenu.localPlayer.gameplayCamera.transform.forward).GetPoint(1f);
                i.gameObject.transform.position = point;
                i.startFallingPosition = point;
                i.targetFloorPosition = point;
            });
        }

        public static void TeleportOneItem()
        {
            GrabbableObject i = LethalMenu.items.Where(i => i != null && !i.isHeld && !i.isPocketed && !i.isInShipRoom && !i.heldByPlayerOnServer).OrderBy(i => Random.value).FirstOrDefault();
            Vector3 point = new Ray(LethalMenu.localPlayer.gameplayCamera.transform.position, LethalMenu.localPlayer.gameplayCamera.transform.forward).GetPoint(1f);
            i.gameObject.transform.position = point;
            i.startFallingPosition = point;
            i.targetFloorPosition = point;
        }

        public static void JoinLobby(SteamId id) => GameNetworkManager.Instance.StartClient(id);

        public static void Disconnect() => GameNetworkManager.Instance.Disconnect();

        public static void DropAllItems()
        {
            Settings.b_DropItems = true;
            if (Settings.b_DropItems) LethalMenu.localPlayer.DropAllHeldItemsAndSync();
            Settings.b_DropItems = false;
        }

        public static void DeleteHeldItem() => LethalMenu.localPlayer.DespawnHeldObject();

        public static void ToggleShipHorn()
        {
            ShipAlarmCord horn = Object.FindObjectOfType<ShipAlarmCord>();
            if (horn == null) return;
            if (Hack.ToggleShipHorn.IsEnabled()) horn.PullCordServerRpc(-1);
            else horn.StopPullingCordServerRpc(-1);
        }

        public static void ToggleCarHorn()
        {
            if (LethalMenu.vehicles == null || LethalMenu.vehicles.Count == 0) return;
            LethalMenu.vehicles.Where(v => v != null).ToList().ForEach(v =>
            {
                if (Hack.ToggleCarHorn.IsEnabled()) v.SetHonkServerRpc(true, -1);
                else v.SetHonkServerRpc(false, -1);
            });
        }

        public static void SpawnMimicFromMasks()
        {
            PlayerControllerB alivePlayer = StartOfRound.Instance.allPlayerScripts.ToList().Find(p => !p.isPlayerDead);
            LethalMenu.items.FindAll(i => i.GetType() == typeof(HauntedMaskItem)).Cast<HauntedMaskItem>().ToList().ForEach(m =>
            {
                m.ChangeOwnershipOfProp(LethalMenu.localPlayer.actualClientId);
                m.Reflect().SetValue("previousPlayerHeldBy", alivePlayer);
                m.CreateMimicServerRpc(m.isInFactory, m.transform.position);
            });
        }

        public static IEnumerator SpamShootAllShotguns()
        {
            while (Hack.SpamShootAllShotguns.IsEnabled())
            {
                ShootAllShotguns();
                yield return new WaitForSeconds(0.1f);
            }
        }

        public static void ShootAllShotguns()
        {
            LethalMenu.items.FindAll(i => i != null && i.NetworkObject != null && i.NetworkObject.IsSpawned && i.GetType() == typeof(ShotgunItem)).Cast<ShotgunItem>().ToList().ForEach(m =>
            {
                PlayerControllerB player = m.Reflect().GetValue<PlayerControllerB>("previousPlayerHeldBy");
                Vector3 pos = player != null ? player.gameplayCamera.transform.position - player.gameplayCamera.transform.up * 0.45f : m.transform.up * 0.45f;
                Vector3 forward = player != null ? player.gameplayCamera.transform.forward : m.transform.forward;
                m.ShootGunServerRpc(pos, forward);
            });
        }

        public static void ExplodeCruiser()
        {
            if (LethalMenu.vehicles == null || LethalMenu.vehicles.Count == 0) return;
            LethalMenu.vehicles.Where(v => v != null).ToList().ForEach(v => v.DestroyCarServerRpc(-1));
        }

        public static void SpawnEnemy(EnemyType type, int num, bool outside)
        {
            if (StartOfRound.Instance.inShipPhase) return;
            SelectableLevel level = StartOfRound.Instance.currentLevel;
            level.maxEnemyPowerCount = Int32.MaxValue;
            GameObject[] gameobject = outside ? RoundManager.Instance.outsideAINodes : RoundManager.Instance.insideAINodes;
            List<Transform> nodes = new List<Transform>();
            Array.ForEach(gameobject, obj => nodes.Add(obj.transform));
            for (int i = 0; i < num; i++)
            {
                Transform node = nodes[Random.Range(0, nodes.Count)];
                if (type.enemyName == "Bush Wolf") LethalMenu.Instance.StartCoroutine(SpawnBushWolf(type));
                else RoundManager.Instance.SpawnEnemyGameObject(node.position, 0.0f, -1, type);
            }
        }

        public static void SpawnMapObject(MapObject type)
        {
            if (LethalMenu.localPlayer == null || RoundManager.Instance.AnomalyRandom == null || StartOfRound.Instance.inShipPhase) return;
            Vector3 pos = LethalMenu.localPlayer.transform.position + LethalMenu.localPlayer.transform.forward * 2f;
            SpawnableMapObject spawnable = GameUtil.GetSpawnableMapObjects().FirstOrDefault(o => o.prefabToSpawn.name == type.ToString());
            GameObject gameObject = Object.Instantiate<GameObject>(spawnable.prefabToSpawn, pos, Quaternion.identity, RoundManager.Instance.mapPropsContainer.transform);
            gameObject.transform.eulerAngles = !spawnable.spawnFacingAwayFromWall ? new Vector3(gameObject.transform.eulerAngles.x, (float)RoundManager.Instance.AnomalyRandom.Next(0, 360), gameObject.transform.eulerAngles.z) : new Vector3(0.0f, RoundManager.Instance.YRotationThatFacesTheFarthestFromPosition(pos + Vector3.up * 0.2f), 0.0f);
            gameObject.GetComponent<NetworkObject>().Spawn(true);
        }

        public static void SpawnMapObjects(MapObject type)
        {
            if (LethalMenu.localPlayer == null || RoundManager.Instance.AnomalyRandom == null || StartOfRound.Instance.inShipPhase) return;
            RandomMapObject[] randomObjects = Object.FindObjectsOfType<RandomMapObject>();
            SpawnableMapObject spawnable = GameUtil.GetSpawnableMapObjects().FirstOrDefault(o => o.prefabToSpawn.name == type.ToString());
            int num = Random.Range(5, 15);
            for (int i = 0; i < num; i++)
            {
                var node = RoundManager.Instance.insideAINodes[Random.Range(0, RoundManager.Instance.insideAINodes.Length)];
                Vector3 pos = RoundManager.Instance.GetRandomNavMeshPositionInRadius(node.transform.position, 30);
                GameObject gameObject = Object.Instantiate<GameObject>(spawnable.prefabToSpawn, pos, Quaternion.identity, RoundManager.Instance.mapPropsContainer.transform);
                gameObject.transform.eulerAngles = !spawnable.spawnFacingAwayFromWall ? new Vector3(gameObject.transform.eulerAngles.x, (float)RoundManager.Instance.AnomalyRandom.Next(0, 360), gameObject.transform.eulerAngles.z) : new Vector3(0.0f, RoundManager.Instance.YRotationThatFacesTheFarthestFromPosition(pos + Vector3.up * 0.2f), 0.0f);
                gameObject.GetComponent<NetworkObject>().Spawn(true);
            }
        }

        public static IEnumerator SpawnBushWolf(EnemyType type)
        {
            MoldSpreadManager moldSpreadManager = Object.FindObjectOfType<MoldSpreadManager>();
            if (moldSpreadManager == null) yield break;
            moldSpreadManager.GenerateMold(RoundManager.Instance.outsideAINodes[Random.Range(0, RoundManager.Instance.outsideAINodes.Length)].transform.position, 1);
            yield return new WaitForSeconds(0.5f);
            moldSpreadManager.RemoveAllMold();
        }

        public static IEnumerator ToggleTerminalSound()
        {
            while (Hack.ToggleTerminalSound.IsEnabled())
            {
                if (LethalMenu.terminal != null) LethalMenu.terminal.PlayTerminalAudioServerRpc(1);
                yield return new WaitForSeconds(0.1f);
            }
        }

        public static void BreakAllWebs()
        {
            LethalMenu.enemies.FindAll(e => e.GetType() == typeof(SandSpiderAI)).Cast<SandSpiderAI>().ToList().ForEach(s => s.BreakAllWebs());
        }

        public static void UnlockAllDoors()
        {
            if (LethalMenu.doorLocks == null || HUDManager.Instance == null) return;
            List<DoorLock> ldoors = LethalMenu.doorLocks.Where(d => d != null && d.isLocked).ToList();
            if (ldoors.Count == 0)
            {
                HUDManager.Instance.DisplayTip("Lethal Menu", $"No doors to unlock!");
                return;
            }
            ldoors.ToList().ForEach(d => d.UnlockDoorSyncWithServer());
            string type = ldoors.Count == 1 ? "door" : "doors";
            HUDManager.Instance.DisplayTip("Lethal Menu", $"Unlocked {ldoors.Count} {type}!");
        }

        public static void OpenAllBigDoors()
        {
            if (LethalMenu.bigDoors == null || HUDManager.Instance == null || RoundManager.Instance == null) return;
            LethalMenu.bigDoors.Where(d => d != null).ToList().ForEach(d =>
            {
                if (!d.Reflect().GetValue<bool>("isPoweredOn")) d.OnPowerSwitch(true);
                d.SetDoorOpenServerRpc(true);
            });
            HUDManager.Instance.DisplayTip("Lethal Menu", "All Big Doors Opened");
        }

        public static void CloseAllBigDoors()
        {
            if (LethalMenu.bigDoors == null || HUDManager.Instance == null) return;
            LethalMenu.bigDoors.Where(d => d != null).ToList().ForEach(d =>
            {
                if (!d.Reflect().GetValue<bool>("isPoweredOn")) d.OnPowerSwitch(true);
                d.SetDoorOpenServerRpc(false);
            });
            HUDManager.Instance.DisplayTip("Lethal Menu", "All Big Doors Closed");
        }

        public static void ToggleMineshaftElevator()
        {
            if (LethalMenu.mineshaftElevator == null) return;
            LethalMenu.mineshaftElevator.ElevatorFinishClientRpc(true);
            LethalMenu.mineshaftElevator.PressElevatorButtonServerRpc();
        }

        public static void ToggleVehicleMagnet()
        {
            if (StartOfRound.Instance == null) return;
            StartOfRound.Instance.SetMagnetOnServerRpc(!StartOfRound.Instance.magnetOn);
        }

        public static void ForceMeteorShower()
        {
            if (!LethalMenu.localPlayer.IsHost() || TimeOfDay.Instance == null) return;
            TimeOfDay.Instance.MeteorWeather.BeginDay(TimeOfDay.Instance.normalizedTimeOfDay);
        }

        public static void ClearMeteorShower()
        {
            if (TimeOfDay.Instance.MeteorWeather.meteors.Count == 0 || !LethalMenu.localPlayer.IsHost() || TimeOfDay.Instance == null) return;
            HUDManager.Instance.DisplayTip("Lethal Menu", $"Cleared {TimeOfDay.Instance.MeteorWeather.meteors.Count} Meteors");
            TimeOfDay.Instance.MeteorWeather.ResetMeteorWeather();
        }

        public static void ChangeMoon(int levelID)
        {
            if(!(bool) StartOfRound.Instance) return;
            StartOfRound.Instance.ChangeLevelServerRpc(levelID, LethalMenu.terminal.groupCredits);
        }

        public static void UnlockObjects()
        {
            if (!Hack.UnlockObjects.IsEnabled() || CameraManager.ActiveCamera == null) return;
            if (Physics.Raycast(CameraManager.ActiveCamera.transform.position, CameraManager.ActiveCamera.transform.forward, out RaycastHit hit, 5f, LayerMask.GetMask("InteractableObject") | LayerMask.GetMask("Default") | LayerMask.GetMask("MapHazards")))
            {
                DoorLock doorLock = hit.transform.GetComponent<DoorLock>();
                TerminalAccessibleObject terminalAccessibleObject = hit.transform.GetComponent<TerminalAccessibleObject>();
                if (doorLock != null && doorLock.isLocked && !doorLock.isPickingLock)
                {
                    doorLock.UnlockDoorSyncWithServer();
                    HUDManager.Instance.DisplayTip("Lethal Menu", "Door Unlocked");
                }
                else if (terminalAccessibleObject != null)
                {
                    if (!terminalAccessibleObject.Reflect().GetValue<bool>("isPoweredOn")) terminalAccessibleObject.OnPowerSwitch(true);
                    if (terminalAccessibleObject.Reflect().GetValue<bool>("inCooldown")) terminalAccessibleObject.Reflect().SetValue("inCooldown", false);
                    string type = terminalAccessibleObject.isBigDoor ? "Big Door" : (terminalAccessibleObject.name == "TurretScript" ? "Turret" : (terminalAccessibleObject.name == "Landmine" ? "Landmine" : "Terminal Object"));
                    terminalAccessibleObject.CallFunctionFromTerminal();
                    HUDManager.Instance.DisplayTip("Lethal Menu", $"{type} ( {terminalAccessibleObject.objectCode} ) has been called from the terminal");
                }
            }
        }

        public static void ClickKill()
        {
            if (!Hack.ClickKill.IsEnabled() || Hack.ClickTeleport.IsEnabled() || CameraManager.ActiveCamera == null) return;
            if (Physics.Raycast(CameraManager.ActiveCamera.transform.position, CameraManager.ActiveCamera.transform.forward, out RaycastHit hit, 1000f, LayerMask.GetMask("Enemies")))
            {
                if (hit.transform == null) return;
                EnemyAI enemy = hit.transform.GetComponentInParent<EnemyAI>() ?? hit.transform.GetComponentInChildren<EnemyAI>();
                if (enemy == null) return;
                if (!enemy.isEnemyDead) enemy?.Handle()?.Kill();
            }
        }

        public static void ClickTeleport()
        {
            if (!Hack.ClickTeleport.IsEnabled() || Hack.ClickKill.IsEnabled() || CameraManager.ActiveCamera == null) return;
            if (Physics.Raycast(CameraManager.ActiveCamera.transform.position, CameraManager.ActiveCamera.transform.forward, out RaycastHit hit, 1000f, LayerMask.GetMask("Room")))
            {
                if (hit.point != null && LethalMenu.localPlayer != null) LethalMenu.localPlayer?.Handle()?.Teleport(hit.point);
            }
        }

        public static void FixAllValves() => LethalMenu.steamValves.Where(s => s != null).ToList().ForEach(v => v.FixValveServerRpc());
        public static void ToggleAllLandmines() => LethalMenu.landmines.Where(l => l != null).ToList().ForEach(m => m.ToggleMine(!Hack.ToggleAllLandmines.IsEnabled()));
        public static void ToggleAllTurrets() => LethalMenu.turrets.Where(t => t != null).ToList().ForEach(t => t.turretActive = !Hack.ToggleAllTurrets.IsEnabled());
        public static void BerserkAllTurrets() => LethalMenu.turrets.Where(t => t != null).ToList().ForEach(t => t.turretMode = Hack.BerserkAllTurrets.IsEnabled() ? TurretMode.Berserk : TurretMode.Detection);
        public static void BlowUpAllLandmines() => LethalMenu.landmines.Where(m => m != null).ToList().ForEach(m => m.ExplodeMineServerRpc());
    }
}
