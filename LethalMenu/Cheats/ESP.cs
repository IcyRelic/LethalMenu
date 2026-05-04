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


namespace LethalMenu.Cheats
{
    internal class ESP : Cheat
    {
        public ESP() => ChamHandler.SetupChamMaterial();

        public override void OnGui()
        {
            if (StartOfRound.Instance == null) return;
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
                Settings.DebugMessage = ("ESP Exception: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace);
            }
        }

        public override void Update()
        {
            DisplayChams(LethalMenu.items?.Where(i => i != null && !i.isHeld));
            DisplayChams(LethalMenu.landmines?.Where(m => m != null && m.IsSpawned && !m.hasExploded));
            DisplayChams(LethalMenu.turrets?.Where(t => t != null && t.IsSpawned && t.gameObject != null && t.gameObject.transform?.parent?.gameObject != null).Select(t => t.gameObject.transform.parent.gameObject));
            DisplayChams(LethalMenu.spikeRoofTraps?.Where(s => s != null && s.IsSpawned && s.gameObject && s.gameObject.transform?.parent?.gameObject != null).Select(s => s.gameObject.transform.parent.gameObject));
            DisplayChams(LethalMenu.players?.Where(p => p != null && !p.isPlayerDead && p.IsRealPlayer() && p != LethalMenu.localPlayer));
            DisplayChams(LethalMenu.enemies?.Where(e => e != null && !e.isEnemyDead));
            DisplayChams(LethalMenu.steamValves?.Where(s => s != null));
            DisplayChams(LethalMenu.bigDoors?.Where(b => b != null));
            DisplayChams(LethalMenu.doors?.Where(d => d != null));
            DisplayChams(LethalMenu.doorLocks?.Where(d => d != null && d.isLocked));
            DisplayChams(LethalMenu.enemyVents?.Where(e => e != null));
            DisplayChams(LethalMenu.vainShrouds?.Where(v => v != null)?.Where(v => v != null));
            DisplayChams(LethalMenu.vehicles?.Where(v => v != null));
            ItemDropship? itemDropship = LethalMenu.itemDropship;
            if (itemDropship != null && itemDropship.deliveringOrder) DisplayChams([itemDropship]);
            HangarShipDoor? shipDoor = LethalMenu.shipDoor;
            if (shipDoor != null) DisplayChams([shipDoor]);
            BreakerBox? breakerBox = LethalMenu.breaker;
            if (breakerBox != null) DisplayChams([breakerBox]);
            MineshaftElevatorController? mineshaftElevator = LethalMenu.mineshaftElevator;
            if (mineshaftElevator != null) DisplayChams([mineshaftElevator]);
        }

        private void DisplayChams<T>(IEnumerable<T>? objects) where T : Object
        {
            if (objects == null) return;
            foreach (var obj in objects.Where(o => o != null))
            {
                Transform? transform = obj switch
                {
                    Transform _transform => _transform,
                    Component component => component.transform,
                    GameObject gameObject => gameObject.transform,
                    _ => null
                };
                if (transform == null) continue;
                float distance = GetDistanceToPlayer(transform.position);
                if (distance == 0f) continue;
                obj.GetChamHandler()?.ProcessCham(distance);
            }
        }

        private void DisplayObjects<T>(IEnumerable<T>? objects, Func<T, string> labelSelector, Func<T, RGBAColor> colorSelector) where T : Component
        {
            if (objects == null) return;
            foreach (T obj in objects.Where(o => o != null && o.gameObject.activeSelf))
            {
                float distance = GetDistanceToPlayer(obj.transform.position);
                if (distance == 0f || distance > Settings.f_espDistance || !WorldToScreen(obj.transform.position, out var screen)) continue;
                if (Hack.NameESP.IsEnabled()) VisualUtil.DrawDistanceString(screen, labelSelector(obj), colorSelector(obj), distance);
                if (Hack.BoxESP.IsEnabled()) VisualUtil.DrawBoxOutline(obj.gameObject, colorSelector(obj), Settings.f_ESPThickness);
            }
        }

        private string Format(string label, params object[] args) => string.Format(Localization.Localize(label), args);

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
            HangarShipDoor? shipDoor = LethalMenu.shipDoor;
            if (shipDoor == null) return;
            DisplayObjects(
                [shipDoor],
                ship => Format("Cheats.ESP.Ship"),
                ship => Settings.c_shipESP
            );
        }

        private void DisplayElevator()
        {
            MineshaftElevatorController? mineshaftElevator = LethalMenu.mineshaftElevator;
            if (mineshaftElevator == null) return;
            DisplayObjects(
                [mineshaftElevator],
                elevator => Format("Cheats.ESP.MineshaftElevator"),
                elevator => Settings.c_mineshaftElevatorESP
            );
        }

        private void DisplayBreaker()
        {
            BreakerBox? breakerBox = LethalMenu.breaker;
            if (breakerBox == null) return;
            DisplayObjects(
                [breakerBox],
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
                LethalMenu.players?.Where(p => p != null && !p.isPlayerDead && p.IsRealPlayer() && p != LethalMenu.localPlayer),
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
                LethalMenu.items?.OfType<RagdollGrabbableObject>().Where(i => i != null && !i.heldByPlayerOnServer && !i.isHeld && !i.isPocketed && i.IsSpawned),
                ragdoll => $"{StartOfRound.Instance.allPlayerScripts[ragdoll.ragdoll.playerObjectId].playerUsername} - {Settings.c_causeOfDeath.AsString(ragdoll.ragdoll.causeOfDeath.ToString())}",
                ragdoll => Settings.c_deadPlayer
            );
        }

        private void DisplaySteamHazards()
        {
            DisplayObjects(
                LethalMenu.steamValves?.Where(v => v != null && !v.Reflect().GetValue<bool>("valveHasBeenRepaired")),
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
                LethalMenu.vainShrouds?.Where(v => v != null && v.transform != null).Select(v => v.transform),
                vain => Format("Cheats.ESP.VainShroud"),
                vain => Settings.c_vainShroudESP
            );
        }

        private void DisplayItemDropShip()
        {
            ItemDropship? itemDropship = LethalMenu.itemDropship;
            if (itemDropship == null || !itemDropship.deliveringOrder) return;
            DisplayObjects(
                new[] { itemDropship }.Where(d => d != null && d.deliveringOrder).ToList(),  
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
