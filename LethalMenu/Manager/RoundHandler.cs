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
            if ((bool)StartOfRound.Instance) StartOfRound.Instance.EndGameServerRpc(0);
        }

        public static void BuyUnlockable(Unlockable unlockable)
        {
            if (!(bool)StartOfRound.Instance) return;

            unlockable.Buy(GetTerminal().groupCredits);
        }

        public static void SpawnScrap()
        {
            if ((bool)RoundManager.Instance) RoundManager.Instance.SpawnScrapInLevel();
        }

        public static void ModScrap(int value, int type)
        {
            if (!(bool)StartOfRound.Instance || !(bool)RoundManager.Instance) return;

            if (type == 0)
                RoundManager.Instance.scrapAmountMultiplier = value;

            if (type == 1)
                RoundManager.Instance.scrapValueMultiplier = value;
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

        public static void TeleportAllItems()
        {
            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;

            LethalMenu.items.FindAll(i => !i.isHeld && !i.isPocketed && !i.isInShipRoom).ForEach(i =>
            {
                Vector3 point = new Ray(localPlayer.gameplayCamera.transform.position, localPlayer.gameplayCamera.transform.forward).GetPoint(1f);

                i.gameObject.transform.position = point;
                i.startFallingPosition = point;
                i.targetFloorPosition = point;
            });
        }

        public static void TeleportOneItem()
        {
            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;

            GrabbableObject itemToTeleport = LethalMenu.items
                .Where(i => !i.isHeld && !i.isPocketed && !i.isInShipRoom)
                .OrderBy(_ => UnityEngine.Random.value) 
                .FirstOrDefault();

            if (itemToTeleport != null)
            {
                Vector3 point = new Ray(localPlayer.gameplayCamera.transform.position, localPlayer.gameplayCamera.transform.forward).GetPoint(1f);

                itemToTeleport.gameObject.transform.position = point;
                itemToTeleport.startFallingPosition = point;
                itemToTeleport.targetFloorPosition = point;
            }
        }

        public static void ToggleShipHorn()
        {
            if (Hack.ToggleShipHorn.IsEnabled()) Object.FindObjectOfType<ShipAlarmCord>().PullCordServerRpc(-1);
            else Object.FindObjectOfType<ShipAlarmCord>().StopPullingCordServerRpc(-1);
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
            SelectableLevel level = StartOfRound.Instance.currentLevel;
            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;

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
            Vector3 pos = LethalMenu.localPlayer.transform.position + LethalMenu.localPlayer.transform.forward * 2f;

            SpawnableMapObject spawnable = GameUtil.GetSpawnableMapObjects().FirstOrDefault(o => o.prefabToSpawn.name == type.ToString());
            GameObject gameObject = Object.Instantiate<GameObject>(spawnable.prefabToSpawn, pos, Quaternion.identity, RoundManager.Instance.mapPropsContainer.transform);
            gameObject.transform.eulerAngles = !spawnable.spawnFacingAwayFromWall ? new Vector3(gameObject.transform.eulerAngles.x, (float)RoundManager.Instance.AnomalyRandom.Next(0, 360), gameObject.transform.eulerAngles.z) : new Vector3(0.0f, RoundManager.Instance.YRotationThatFacesTheFarthestFromPosition(pos + Vector3.up * 0.2f), 0.0f);
            gameObject.GetComponent<NetworkObject>().Spawn(true);
        }
        
        public static void SpawnMapObjects(MapObject type)
        {
            RandomMapObject[] randomObjects = Object.FindObjectsOfType<RandomMapObject>();

            SpawnableMapObject spawnable = GameUtil.GetSpawnableMapObjects().FirstOrDefault(o => o.prefabToSpawn.name == type.ToString());

            int num = Random.Range(5, 15);

            Debug.LogError("Spawning " + num + " " + spawnable.prefabToSpawn.name);


            for (int i = 0; i < num; i++)
            {
                var node = RoundManager.Instance.insideAINodes[Random.Range(0, RoundManager.Instance.insideAINodes.Length)];

                Vector3 pos = RoundManager.Instance.GetRandomNavMeshPositionInRadius(node.transform.position, 30);
                GameObject gameObject = Object.Instantiate<GameObject>(spawnable.prefabToSpawn, pos, Quaternion.identity, RoundManager.Instance.mapPropsContainer.transform);
                gameObject.transform.eulerAngles = !spawnable.spawnFacingAwayFromWall ? new Vector3(gameObject.transform.eulerAngles.x, (float)RoundManager.Instance.AnomalyRandom.Next(0, 360), gameObject.transform.eulerAngles.z) : new Vector3(0.0f, RoundManager.Instance.YRotationThatFacesTheFarthestFromPosition(pos + Vector3.up * 0.2f), 0.0f);
                gameObject.GetComponent<NetworkObject>().Spawn(true);
            }
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
        public static void BlowUpAllLandmines() => LethalMenu.landmines.ForEach(mine => mine.ExplodeMineServerRpc());
        public static Terminal GetTerminal() => Object.FindObjectOfType(typeof(Terminal)) as Terminal;
    }
}
