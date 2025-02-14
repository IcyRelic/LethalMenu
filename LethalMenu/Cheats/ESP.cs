using GameNetcodeStuff;
using LethalMenu.Handler;
using LethalMenu.Language;
using LethalMenu.Types;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;


namespace LethalMenu.Cheats
{
    internal class ESP : Cheat
    {
        public ESP() => ChamHandler.SetupChamMaterial();
        public override void OnGui()
        {
            if (!(bool)StartOfRound.Instance) return;

            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;

            if (player == null) return;

            try
            {
                if (Hack.ObjectESP.IsEnabled()) this.DisplayScrap();
                if (Hack.EnemyESP.IsEnabled()) this.DisplayEnemyAI();
                if (Hack.PlayerESP.IsEnabled()) this.DisplayPlayers();
                if (Hack.BodyESP.IsEnabled()) this.DisplayBodies();
                if (Hack.DoorESP.IsEnabled()) this.DisplayEntranceExitDoors();
                if (Hack.LandmineESP.IsEnabled()) this.DisplayLandmines();
                if (Hack.TurretESP.IsEnabled()) this.DisplayTurrets();
                if (Hack.ShipESP.IsEnabled()) this.DisplayShip();
                if (Hack.BigDoorESP.IsEnabled()) this.DisplayBigDoors();
                if (Hack.SteamHazardESP.IsEnabled()) this.DisplaySteamHazards();
                if (Hack.DoorLockESP.IsEnabled()) this.DisplayDoorLocks();
                if (Hack.BreakerESP.IsEnabled()) this.DisplayBreaker();
                if (Hack.SpikeRoofTrapESP.IsEnabled()) this.DisplaySpikeRoofTraps();
                if (Hack.MineshaftElevatorESP.IsEnabled()) this.DisplayElevator();
                if (Hack.EnemyVentESP.IsEnabled()) this.DisplayEnemyVents();
                if (Hack.VainShroudESP.IsEnabled()) this.DisplayVainShrouds();
                if (Hack.CruiserESP.IsEnabled()) this.DisplayCruiser();
                if (Hack.ItemDropShipESP.IsEnabled()) this.DisplayItemDropShip();
            }
            catch (Exception e)
            {
                Settings.debugMessage = $"SRC: {e.Message}\nStackTrace: {e.StackTrace}";
            }
        }

        public override void Update() => this.DisplayChams();

        private void DisplayChams<T>(IEnumerable<T> objects, Func<T, RGBAColor> colorSelector) where T : Object
        {
            objects?.ToList().ForEach(o =>
            {
                if (o == null) return;
                Transform transform = o is Component component ? component.transform : o is GameObject gameObject ? gameObject.transform : null;
                if (transform == null) return;
                float distance = GetDistanceToPlayer(transform.position);
                o.GetChamHandler()?.ProcessCham(distance);
            });
        }

        public void DisplayChams()
        {
            DisplayChams(LethalMenu.items, _ => Settings.c_chams);
            DisplayChams(LethalMenu.landmines, _ => Settings.c_chams);
            DisplayChams(LethalMenu.turrets?.Where(t => t != null && t.gameObject != null && t.gameObject.transform?.parent?.gameObject != null).Select(t => t.gameObject.transform.parent.gameObject), _ => Settings.c_chams);
            DisplayChams(LethalMenu.spikeRoofTraps?.Where(s => s != null && s.gameObject && s.gameObject.transform?.parent?.gameObject != null).Select(s => s?.gameObject.transform?.parent?.gameObject), _ => Settings.c_chams);
            DisplayChams(LethalMenu.players?.Where(p => p != null && p != LethalMenu.localPlayer), _ => Settings.c_chams);
            DisplayChams(LethalMenu.enemies, _ => Settings.c_chams);
            DisplayChams(LethalMenu.steamValves, _ => Settings.c_chams);
            DisplayChams(LethalMenu.bigDoors, _ => Settings.c_chams);
            DisplayChams(LethalMenu.doorLocks, _ => Settings.c_chams);
            DisplayChams(LethalMenu.enemyVents, _ => Settings.c_chams);
            DisplayChams(LethalMenu.vainShrouds?.Where(v => v != null), _ => Settings.c_chams);
            DisplayChams(LethalMenu.vehicles, _ => Settings.c_chams);
            DisplayChams(new[] { LethalMenu.itemDropship }, _ => Settings.c_chams);
            DisplayChams(new[] { LethalMenu.shipDoor }, _ => Settings.c_chams);
            DisplayChams(new[] { LethalMenu.breaker }, _ => Settings.c_chams);
            DisplayChams(new[] { LethalMenu.mineshaftElevator }, _ => Settings.c_chams);
        }

        private void DisplayObjects<T>(IEnumerable<T> objects, Func<T, string> labelSelector, Func<T, RGBAColor> colorSelector) where T : Component
        {
            foreach (T obj in objects)
            {
                if (obj != null && obj.gameObject.activeSelf)
                {
                    float distance = GetDistanceToPlayer(obj.transform.position);

                    if (distance > Settings.f_espDistance || !WorldToScreen(obj.transform.position, out Vector3 screen)) continue;

                    VisualUtil.DrawDistanceString(screen, labelSelector(obj), colorSelector(obj), distance);
                }
            }
        }

        private string Format(string label, params object[] args) => String.Format(Localization.Localize(label), args);

        private void DisplayTurrets()
        {
            DisplayObjects(
                LethalMenu.turrets?.Where(t => t != null && t.IsSpawned), 
                turret => Format("Cheats.ESP.Turret", turret.GetComponent<TerminalAccessibleObject>().objectCode),
                turret => Settings.c_turretESP 
            );
        }

        private void DisplayShip()
        {
            DisplayObjects(
                new[] { LethalMenu.shipDoor },
                ship => Format("Cheats.ESP.Ship"),
                ship => Settings.c_shipESP
            );
        }

        private void DisplayElevator()
        {
            DisplayObjects(
                new[] { LethalMenu.mineshaftElevator },
                elevator => Format("Cheats.ESP.MineshaftElevator"),
                elevator => Settings.c_mineshaftElevatorESP
            );
        }

        private void DisplayBreaker()
        {
            DisplayObjects(
                new[] { LethalMenu.breaker },
                breaker => Format("Cheats.ESP.BreakerBox"),
                breaker => Settings.c_breakerESP
            );
        }

        private void DisplayEntranceExitDoors()
        {
            DisplayObjects(
                LethalMenu.doors,
                door => door.isEntranceToBuilding ? Format("Cheats.ESP.Entrance") : Format("Cheats.ESP.Exit"),
                door => Settings.c_entranceExitESP
            );
        }

        private void DisplayLandmines()
        {
            DisplayObjects(
                LethalMenu.landmines?.Where(m => m != null && m.IsSpawned && !m.hasExploded),
                mine => Format($"Cheats.ESP.Landmine", mine.GetComponent<TerminalAccessibleObject>().objectCode),
                mine => Settings.c_landmineESP
            );
        }

        private void DisplayPlayers()
        {
            DisplayObjects(
                LethalMenu.players?.Where(p => p != null && !p.isPlayerDead && !p.disconnectedMidGame && !p.playerUsername.StartsWith("Player #") && p != LethalMenu.localPlayer),
                player => $"{(Settings.b_VCDisplay && player.voicePlayerState != null && player.voicePlayerState.IsSpeaking ? "[VC] " : "")}{(Settings.b_PlayerHPDisplay ? $"[HP: {player.health}] " : "")}{(player.playerUsername ?? "Unknown")}",
                player => Settings.c_playerESP
            );
        }

        private void DisplayEnemyAI()
        {
            DisplayObjects(
                LethalMenu.enemies?.Where(e => e != null && !e.isEnemyDead && e.GetEnemyAIType().IsESPEnabled()),
                enemy => enemy.enemyType.enemyName,
                enemy => Settings.c_enemyESP
            );
        }

        private void DisplayScrap()
        {
            DisplayObjects(
                LethalMenu.items?.Where(i => i != null && !i.heldByPlayerOnServer && !i.isHeld && !i.isPocketed && i.IsSpawned && i.itemProperties != null && !i.deactivated && !(i is RagdollGrabbableObject)),
                item =>
                {
                    if (item is GiftBoxItem box && box.Reflect().GetValue<Item>("objectInPresentItem") is Item _item && box.Reflect().GetValue<int>("objectInPresentValue") is int _value) return $"{item.itemProperties.itemName} ( {item.scrapValue} ) - {_item.itemName} ( {_value} )";
                    return $"{item.itemProperties.itemName} ( {item.scrapValue} )"; 
                },
                item =>
                {
                    if (!Settings.b_useScrapTiers) return Settings.c_objectESP;
                    int index = Array.FindLastIndex(Settings.i_scrapValueThresholds, x => x <= item.scrapValue);
                    return index > -1 ? Settings.c_scrapValueColors[index] : Settings.c_objectESP;
                }
            );
        }

        private void DisplayBodies()
        {
            DisplayObjects(
                LethalMenu.items?.Where(i => i != null && !i.heldByPlayerOnServer && !i.isHeld && !i.isPocketed && i.IsSpawned && i is RagdollGrabbableObject),
                ragdoll => ragdoll is RagdollGrabbableObject body ? $"{StartOfRound.Instance.allPlayerScripts[body.ragdoll.playerObjectId].playerUsername} - {Settings.c_causeOfDeath.AsString(body.ragdoll.causeOfDeath.ToString())}" : null,
                ragdoll => Settings.c_deadPlayer
            );
        }

        private void DisplaySteamHazards()
        {
            DisplayObjects(
                LethalMenu.steamValves?.Where(v => v != null && v.triggerScript.interactable),
                valve => Format("Cheats.ESP.SteamValve"),
                valve => Settings.c_steamHazardESP
            );
        }

        private void DisplayBigDoors()
        {
            DisplayObjects(
                LethalMenu.bigDoors?.Where(d => d != null && d.isBigDoor),
                door => Format($"Cheats.ESP.BigDoors", door.objectCode),
                door => Settings.c_bigDoorESP
            );
        }

        private void DisplayDoorLocks()
        {
            DisplayObjects(
                LethalMenu.doorLocks?.Where(d => d != null && d.isLocked),
                door => Format("Cheats.ESP.LockedDoor"),
                door => Settings.c_doorLockESP
            );
        }

        private void DisplaySpikeRoofTraps()
        {
            DisplayObjects(
                LethalMenu.spikeRoofTraps?.Where(t => t != null && t.trapActive),
                trap => Format("Cheats.ESP.SpikeRoofTrap"),
                trap => Settings.c_spikeRoofTrapESP
            );
        }

        private void DisplayEnemyVents()
        {
            DisplayObjects(
                LethalMenu.enemyVents?.Where(e => e != null && !e.ventIsOpen),
                vent => Format("Cheats.ESP.EnemyVent"),
                vent => Settings.c_enemyVentESP
            );
        }
        private void DisplayVainShrouds()
        {
            DisplayObjects(
                LethalMenu.vainShrouds?.Where(v => v != null && v.transform != null).Select(v => v.transform) ?? Enumerable.Empty<Transform>(),
                vain => Format("Cheats.ESP.VainShroud"),
                vain => Settings.c_vainShroudESP
            );
        }

        private void DisplayItemDropShip()
        {
            DisplayObjects(
                new[] { LethalMenu.itemDropship }.Where(d => d != null && d.deliveringOrder).ToList(),  
                dropship => Format("Cheats.ESP.ItemDropShip"),
                dropship => Settings.c_itemDropShipESP
            );
        }

        private void DisplayCruiser()
        {
            DisplayObjects(
                LethalMenu.vehicles?.Where(v => v != null),
                vehicle => Format("Cheats.ESP.Cruiser"),
                vehicle => Settings.c_CruiserESP
            );
        }
    }
}
