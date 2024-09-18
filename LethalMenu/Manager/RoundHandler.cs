using GameNetcodeStuff;
using LethalMenu.Handler;
using LethalMenu.Types;
using LethalMenu.Util;
using System;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Steamworks;
using System.Threading.Tasks;
using System.Collections.Generic;


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
            if (GetTerminal() == null) return;           
            int newAmt = amount;
            if(type == ActionType.Add) newAmt = GetTerminal().groupCredits + amount;
            if(type == ActionType.Remove) newAmt = GetTerminal().groupCredits - amount;
            GetTerminal().SyncGroupCreditsServerRpc(newAmt, GetTerminal().numberOfItemsInDropship);
        }

        public static void SetQuota(int amount)
        {
            Object.FindObjectOfType<TimeOfDay>().profitQuota = amount;
            TimeOfDay.Instance.SyncNewProfitQuotaClientRpc(TimeOfDay.Instance.profitQuota, 0, TimeOfDay.Instance.timesFulfilledQuota);
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
                BuyUnlockable(Unlockable.SignalTranslator, false, false);
                HUDManager.Instance.UseSignalTranslatorServerRpc(msg);
            }
            else if (type == 3 && Unlockable.SignalTranslator.GetItem().hasBeenUnlockedByPlayer) HUDManager.Instance.UseSignalTranslatorServerRpc(msg);
            if (type > 2) return;
            string[] prefixes = { "[System]", "[Server]", "[Broadcast]" };
            string fmsg = $"{prefixes[type]} {msg}";
            if (HUDManager.Instance.lastChatMessage == fmsg) fmsg += "\u200B";
            HUDManager.Instance.AddTextToChatOnServer(fmsg);
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

        public static void FlickerLights()
        {
            if (!(bool)StartOfRound.Instance || !(bool)RoundManager.Instance) return;

            RoundManager.Instance.FlickerLights();
        }

        public static void StartGame()
        {
            if ((bool)StartOfRound.Instance) StartOfRound.Instance.StartGameServerRpc();
        }

        public static void EndGame()
        {
            if ((bool)StartOfRound.Instance) StartOfRound.Instance.EndGameServerRpc(-1);
        }
        
        public static void BuyUnlockable(Unlockable unlockable, bool all, bool enabled)
        {
            if (!(bool)StartOfRound.Instance) return;
            unlockable.Buy(GetTerminal().groupCredits);
            if (all)
            {
                HUDManager.Instance.DisplayTip("Lethal Menu", $"Unlocked All Cosmetics!");
                return;
            }
            if (enabled)
            {
                HUDManager.Instance.DisplayTip("Lethal Menu", $"Unlocked Cosmetic: {unlockable}!");
                return;
            }
        }

        public static void BuyUnlockableSuit(Unlockable unlockablesuit, bool wearbuy, bool buy, bool sound)
        {
            if (!(bool)StartOfRound.Instance) return;
            if (buy && !unlockablesuit.GetItem().hasBeenUnlockedByPlayer)
            {
                unlockablesuit.Buy(GetTerminal().groupCredits);
                unlockablesuit.SetLocked(true);
                HUDManager.Instance.DisplayTip("Lethal Menu", $"Unlocked Suit: {unlockablesuit}!");
            }
            if (wearbuy) UnlockableSuit.SwitchSuitForPlayer(LethalMenu.localPlayer, (int)unlockablesuit, sound);
        }

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
            if (!RoundManager.Instance.currentLevel.spawnEnemiesAndScrap) return;
            if ((bool)RoundManager.Instance) RoundManager.Instance.SpawnScrapInLevel();
        }

        public static void ModScrap(int value, int type)
        {
            if (!(bool)StartOfRound.Instance || !(bool)RoundManager.Instance) return;
            if (type == 0)RoundManager.Instance.scrapAmountMultiplier = value;
            if (type == 1) RoundManager.Instance.scrapValueMultiplier = value;
        }
        
        public static void ForceTentacleAttack()
        {
            DepositItemsDesk desk = ((DepositItemsDesk)Object.FindObjectOfType(typeof(DepositItemsDesk)));

            if (desk == null) return;

            desk.AttackPlayersServerRpc();
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
            for (int i = 0; i < 4; i++)
            {
                trigger.AddToBridgeInstabilityServerRpc();
            }
        }

        public static void TeleportAllItems()
        {
            LethalMenu.items.FindAll(i => !i.isHeld && !i.isPocketed && !i.isInShipRoom).ForEach(i =>
            {
                Vector3 point = new Ray(LethalMenu.localPlayer.gameplayCamera.transform.position, LethalMenu.localPlayer.gameplayCamera.transform.forward).GetPoint(1f);
                i.gameObject.transform.position = point;
                i.startFallingPosition = point;
                i.targetFloorPosition = point;
            });
        }

        public static void TeleportOneItem()
        {
            GrabbableObject itemToTeleport = LethalMenu.items.Where(i => !i.isHeld && !i.isPocketed && !i.isInShipRoom).OrderBy(i => Random.value) .FirstOrDefault();
            if (itemToTeleport != null)
            {
                Vector3 point = new Ray(LethalMenu.localPlayer.gameplayCamera.transform.position, LethalMenu.localPlayer.gameplayCamera.transform.forward).GetPoint(1f);
                itemToTeleport.gameObject.transform.position = point;
                itemToTeleport.startFallingPosition = point;
                itemToTeleport.targetFloorPosition = point;
            }
        }

        public static void JoinLobby(SteamId id)
        {
            GameNetworkManager.Instance.StartClient(id);
        }

        public static void Disconnect()
        {
            GameNetworkManager.Instance.Disconnect();
        }

        public static void DropAllItems()
        {
            Settings.b_DropItems = true;
            if (Settings.b_DropItems) LethalMenu.localPlayer.DropAllHeldItemsAndSync();
            Settings.b_DropItems = false;
        }

        public static void DeleteHeldItem()
        {
            LethalMenu.localPlayer.DespawnHeldObject();
        }

        public static void ToggleShipHorn()
        {
            if (Hack.ToggleShipHorn.IsEnabled()) Object.FindObjectOfType<ShipAlarmCord>().PullCordServerRpc(-1);
            else Object.FindObjectOfType<ShipAlarmCord>().StopPullingCordServerRpc(-1);
        }

        public static void ToggleCarHorn()
        {
            if (Hack.ToggleCarHorn.IsEnabled()) Object.FindObjectOfType<VehicleController>().SetHonkServerRpc(true, -1);
            else Object.FindObjectOfType<VehicleController>().SetHonkServerRpc(false, -1);
        }

        public static void SpawnMimicFromMasks()
        {
            PlayerControllerB alivePlayer = StartOfRound.Instance.allPlayerScripts.ToList().Find(p => !p.isPlayerDead);
            LethalMenu.items.FindAll(i => i.GetType() == typeof(HauntedMaskItem)).Cast<HauntedMaskItem>().ToList().ForEach(m =>
            {
                m.ChangeOwnershipOfProp(GameNetworkManager.Instance.localPlayerController.actualClientId);
                m.Reflect().SetValue("previousPlayerHeldBy", alivePlayer);
                bool factory = m.transform.position.y < LethalMenu.shipDoor.transform.position.y - 10f;
                m.CreateMimicServerRpc(factory, m.transform.position);
            });
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
                RoundManager.Instance.SpawnEnemyGameObject(node.position, 0.0f, -1, type);
            }
        }

        public static void SpawnMapObject(MapObject type)
        {
            if (LethalMenu.localPlayer == null || RoundManager.Instance.AnomalyRandom == null) return;
            Vector3 pos = LethalMenu.localPlayer.transform.position + LethalMenu.localPlayer.transform.forward * 2f;
            SpawnableMapObject spawnable = GameUtil.GetSpawnableMapObjects().FirstOrDefault(o => o.prefabToSpawn.name == type.ToString());
            GameObject gameObject = Object.Instantiate<GameObject>(spawnable.prefabToSpawn, pos, Quaternion.identity, RoundManager.Instance.mapPropsContainer.transform);
            gameObject.transform.eulerAngles = !spawnable.spawnFacingAwayFromWall ? new Vector3(gameObject.transform.eulerAngles.x, (float)RoundManager.Instance.AnomalyRandom.Next(0, 360), gameObject.transform.eulerAngles.z) : new Vector3(0.0f, RoundManager.Instance.YRotationThatFacesTheFarthestFromPosition(pos + Vector3.up * 0.2f), 0.0f);
            gameObject.GetComponent<NetworkObject>().Spawn(true);
        }

        public static void SpawnMapObjects(MapObject type)
        {
            if (LethalMenu.localPlayer == null || RoundManager.Instance.AnomalyRandom == null) return;
            RandomMapObject[] randomObjects = Object.FindObjectsOfType<RandomMapObject>();
            SpawnableMapObject spawnable = GameUtil.GetSpawnableMapObjects().FirstOrDefault(o => o.prefabToSpawn.name == type.ToString());
            int num = Random.Range(5, 15);
            Debug.Log("Spawning " + num + " " + spawnable.prefabToSpawn.name);
            for (int i = 0; i < num; i++)
            {
                var node = RoundManager.Instance.insideAINodes[Random.Range(0, RoundManager.Instance.insideAINodes.Length)];
                Vector3 pos = RoundManager.Instance.GetRandomNavMeshPositionInRadius(node.transform.position, 30);
                GameObject gameObject = Object.Instantiate<GameObject>(spawnable.prefabToSpawn, pos, Quaternion.identity, RoundManager.Instance.mapPropsContainer.transform);
                gameObject.transform.eulerAngles = !spawnable.spawnFacingAwayFromWall ? new Vector3(gameObject.transform.eulerAngles.x, (float)RoundManager.Instance.AnomalyRandom.Next(0, 360), gameObject.transform.eulerAngles.z) : new Vector3(0.0f, RoundManager.Instance.YRotationThatFacesTheFarthestFromPosition(pos + Vector3.up * 0.2f), 0.0f);
                gameObject.GetComponent<NetworkObject>().Spawn(true);
            }
        }

        public static async void ToggleTerminalSound() => await SpamTerminalSound();

        public static async Task SpamTerminalSound()
        {
            while (Hack.ToggleTerminalSound.IsEnabled())
            {
                if (GetTerminal() == null)
                {
                    await Task.Delay(10000);
                    continue;
                }
                GetTerminal().PlayTerminalAudioServerRpc(1);
                await Task.Delay(100);
            }
        }

        public static void BreakAllWebs()
        {
            LethalMenu.enemies.FindAll(e => e.GetType() == typeof(SandSpiderAI)).Cast<SandSpiderAI>().ToList().ForEach(s => s.BreakAllWebs());
        }

        public static void UnlockAllDoors()
        {
            if (LethalMenu.doorLocks == null) return;
            LethalMenu.doorLocks.FindAll(door => door.isLocked).ForEach(door => door.UnlockDoorServerRpc());
            HUDManager.Instance.DisplayTip("Lethal Menu", "All Doors Unlocked");
        }

        public static void OpenAllBigDoors()
        { 
            if (LethalMenu.bigDoors == null) return;
            LethalMenu.bigDoors.ForEach(door => door.SetDoorOpenServerRpc(true));
            HUDManager.Instance.DisplayTip("Lethal Menu", "All Big Doors Opened");
        }

        public static void CloseAllBigDoors()
        {
            if (LethalMenu.bigDoors == null) return;
            LethalMenu.bigDoors.ForEach(door => door.SetDoorOpenServerRpc(false));
            HUDManager.Instance.DisplayTip("Lethal Menu", "All Big Doors Closed");
        }

        public static void ForceMeteorShower()
        {
            if (!LethalMenu.localPlayer.IsHost || TimeOfDay.Instance == null) return;
            TimeOfDay.Instance.MeteorWeather.BeginDay(TimeOfDay.Instance.normalizedTimeOfDay);
        }

        public static void ClearMeteorShower()
        {
            if (TimeOfDay.Instance.MeteorWeather.meteors.Count == 0 || !LethalMenu.localPlayer.IsHost || TimeOfDay.Instance == null) return;
            HUDManager.Instance.DisplayTip("Lethal Menu", $"Cleared {TimeOfDay.Instance.MeteorWeather.meteors.Count} Meteors");
            TimeOfDay.Instance.MeteorWeather.ResetMeteorWeather();
        }

        public static void ChangeMoon(int levelID)
        {
            if(!(bool) StartOfRound.Instance) return;
            StartOfRound.Instance.ChangeLevelServerRpc(levelID, GetTerminal().groupCredits);
        }

        public static void FixAllValves() => LethalMenu.steamValves.ForEach(v => v.FixValveServerRpc());
        public static void ToggleAllLandmines() => LethalMenu.landmines.ForEach(mine => mine.ToggleMine(!Hack.ToggleAllLandmines.IsEnabled()));
        public static void ToggleAllTurrets() => LethalMenu.turrets.ForEach(turret => turret.turretActive = !Hack.ToggleAllTurrets.IsEnabled());
        public static void BerserkAllTurrets() => LethalMenu.turrets.ForEach(turret => turret.turretMode = Hack.BerserkAllTurrets.IsEnabled() ? TurretMode.Berserk : TurretMode.Detection);
        public static void BlowUpAllLandmines() => LethalMenu.landmines.ForEach(mine => mine.ExplodeMineServerRpc());
        public static Terminal GetTerminal() => Object.FindObjectOfType(typeof(Terminal)) as Terminal;
    }
}
