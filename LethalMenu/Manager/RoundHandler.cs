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
using System.Collections.Generic;
using System.Threading.Tasks;


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
            Terminal terminal = GetTerminal();

            if (terminal == null) return;
            
            int newAmt = amount;

            if(type == ActionType.Add) newAmt = terminal.groupCredits + amount;
            if(type == ActionType.Remove) newAmt = terminal.groupCredits - amount;

            terminal.SyncGroupCreditsServerRpc(newAmt, terminal.numberOfItemsInDropship);
        }


        public static void SetQuota(int amount)
        {
            Object.FindObjectOfType<TimeOfDay>().profitQuota = amount;
            TimeOfDay.Instance.SyncNewProfitQuotaClientRpc(TimeOfDay.Instance.profitQuota, 0, TimeOfDay.Instance.timesFulfilledQuota);
        }

        public static void Message(string msg, int type, int id)
        {
            if (HUDManager.Instance == null) return;
            if (type == 4)
            {
                if (HUDManager.Instance.lastChatMessage == msg) msg += "\u200B";
                HUDManager.Instance.AddTextToChatOnServer(msg, id);
            }
            if (type == 3 && !Unlockable.SignalTranslator.GetItem().hasBeenUnlockedByPlayer)
            {
                BuyUnlockable(Unlockable.SignalTranslator, false, false);
                HUDManager.Instance.UseSignalTranslatorServerRpc(msg);
                Unlockable.SignalTranslator.Move(new Vector3(-1000, -1000, -1000), new Vector3(0, 0, 0), true);
            }
            else if (type == 3) HUDManager.Instance.UseSignalTranslatorServerRpc(msg);
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

        public static void DeleteTerminal()
        {
            if (!(bool)StartOfRound.Instance) return;
            Unlockable.Terminal.Move(new Vector3(-1000, -1000, -1000), new Vector3(0, 0, 0), false);
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

        public static async void Panic(bool e) => await RPanic();

        public static async Task RPanic()
        {
            float f_chamDistance = Settings.f_chamDistance;
            bool DebugMode = Settings.DebugMode;
            Dictionary<Hack, bool> StoredFlags = new Dictionary<Hack, bool>();
            if (Settings.b_Panic && !StoredFlags.Any())
            {
                foreach (Hack hack in Enum.GetValues(typeof(Hack)))
                {
                    if (HackExtensions.ToggleFlags.ContainsKey(hack) && !StoredFlags.ContainsKey(hack))
                    {
                        StoredFlags[hack] = HackExtensions.ToggleFlags[hack];
                        hack.SetToggle(false);
                        Settings.f_chamDistance = float.MaxValue;
                        Settings.DebugMode = false;
                    }
                }
            }
            while (true)
            {
                if (!Settings.b_Panic && StoredFlags.Any())
                {
                    foreach (var k in StoredFlags) HackExtensions.ToggleFlags[k.Key] = k.Value;
                    Settings.f_chamDistance = f_chamDistance;
                    Settings.DebugMode = DebugMode;
                    StoredFlags.Clear();
                }
                if (Settings.b_Panic && HackExtensions.ToggleFlags.Where(k => k.Key != Hack.ToggleCursor).Count(k => k.Value) >= 1) Settings.b_Panic = false;
                await Task.Delay(2500);
            }
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
            if (Settings.b_DropItems)
            {
                LethalMenu.localPlayer.DropAllHeldItemsAndSync();
            }
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

            var nodes = outside ? RoundManager.Instance.outsideAINodes : RoundManager.Instance.insideAINodes;

            for (int i = 0; i < num; i++)
            {
                var node = nodes[Random.Range(0, nodes.Length)];
                RoundManager.Instance.SpawnEnemyGameObject(node.transform.position, 0.0f, -1, type);
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

        public static void ToggleTerminalSound()
        {
            if (Hack.ToggleTerminalSound.IsEnabled()) Object.FindObjectOfType<Terminal>().PlayTerminalAudioServerRpc(1);
        }

        public static void BreakAllWebs()
        {
            LethalMenu.enemies.FindAll(e => e.GetType() == typeof(SandSpiderAI)).Cast<SandSpiderAI>().ToList().ForEach(s => s.BreakAllWebs());
        }
        public static void UnlockAllDoors()
        {
            LethalMenu.doorLocks.FindAll(door => door.isLocked).ForEach(door => door.UnlockDoorServerRpc());
            LethalMenu.bigDoors.ForEach(door => door.SetDoorOpenServerRpc(true));
        }

        public static void CloseAllBigDoors()
        {
            LethalMenu.bigDoors.ForEach(door => door.SetDoorOpenServerRpc(false));
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
