using GameNetcodeStuff;
using LethalMenu.Handler;
using LethalMenu.Types;
using LethalMenu.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.Diagnostics;
using static UnityEngine.GraphicsBuffer;
using Object = UnityEngine.Object;


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
                if (Hack.DoorESP.IsEnabled()) this.DisplayEntranceExitDoors();
                if (Hack.LandmineESP.IsEnabled()) this.DisplayLandmines();
                if (Hack.TurretESP.IsEnabled()) this.DisplayTurrets();
                if (Hack.ShipESP.IsEnabled()) this.DisplayShip();
                if (Hack.BigDoorESP.IsEnabled()) this.DisplayBigDoors();
                if (Hack.SteamHazardESP.IsEnabled()) this.DisplaySteamHazards();
                if (Hack.DoorLockESP.IsEnabled()) this.DisplayDoorLocks();
                if (Hack.BreakerESP.IsEnabled()) this.DisplayBreaker();
            }
            catch (Exception e)
            {
                LethalMenu.debugMessage = e.Message + "\n" + e.StackTrace;
            }

            

        }

        public override void Update() => this.DisplayChams();

        private void DisplayChams<T>(IEnumerable<T> objects, Func<T, RGBAColor> colorSelector) where T : Object
        {
            objects.ToList().ForEach(o =>
            {
                Transform transform;

                if (o is Component component) transform = component.transform;
                else if (o is GameObject gameObject) transform = gameObject.transform;
                else return;

                if (o == null) return;
                float distance = GetDistanceToPlayer(transform.position);
                o.GetChamHandler().ProcessCham(distance);
            });
        }

        private void DisplayChams()
        {
            DisplayChams(LethalMenu.items, _ => Settings.c_chams);
            DisplayChams(LethalMenu.landmines, _ => Settings.c_chams);
            DisplayChams(LethalMenu.turrets.ConvertAll(t => t.gameObject.transform.parent.gameObject), _ => Settings.c_chams);
            DisplayChams(LethalMenu.players.Where(p => p.playerClientId != LethalMenu.localPlayer.playerClientId), _ => Settings.c_chams);
            DisplayChams(LethalMenu.enemies, _ => Settings.c_chams);
            DisplayChams(LethalMenu.steamValves, _ => Settings.c_chams);
            DisplayChams(LethalMenu.bigDoors, _ => Settings.c_chams);
            DisplayChams(LethalMenu.doorLocks, _ => Settings.c_chams);
            DisplayChams(new[] { LethalMenu.shipDoor }, _ => Settings.c_chams);
            DisplayChams(new[] { LethalMenu.breaker }, _ => Settings.c_chams);
        }

        

        private void DisplayObjects<T>(IEnumerable<T> objects, Func<T, string> labelSelector, Func<T, RGBAColor> colorSelector) where T : Component
        {
            foreach (T obj in objects)
            {
                if (obj != null && obj.gameObject.activeSelf)
                {
                    float distance = GetDistanceToPlayer(obj.transform.position);

                    if(distance > Settings.f_espDistance || !WorldToScreen(obj.transform.position, out Vector3 screen)) continue;

                    VisualUtil.DrawDistanceString(screen, labelSelector(obj), colorSelector(obj), distance);
                }
            }
        }

        private void DisplayTurrets()
        {
            DisplayObjects(
                LethalMenu.turrets.Where(t => t != null && t.IsSpawned), 
                turret => "Turret [ " + turret.GetComponent<TerminalAccessibleObject>().objectCode + " ]",
                turret => Settings.c_turretESP
            );
        }

        private void DisplayShip()
        {
            DisplayObjects(
                new[] { LethalMenu.shipDoor },
                ship => "Ship",
                ship => Settings.c_shipESP
            );
        }

        private void DisplayBreaker()
        {
            DisplayObjects(
                new[] { LethalMenu.breaker },
                breaker => "Breaker Box",
                breaker => Settings.c_breakerESP
            );
        }

        private void DisplayEntranceExitDoors()
        {
            DisplayObjects(
                LethalMenu.doors,
                door => door.isEntranceToBuilding ? "Entrance" : "Exit",
                door => Settings.c_entranceExitESP
            );
        }

        private void DisplayLandmines()
        {
            DisplayObjects(
                LethalMenu.landmines.Where(m => m != null && m.IsSpawned && !m.hasExploded),
                mine => "Landmine [ " + mine.GetComponent<TerminalAccessibleObject>().objectCode + " ]",
                mine => Settings.c_landmineESP
            );
        }

        private void DisplayPlayers()
        {
            DisplayObjects(
                LethalMenu.players
                    .Where(p => p != null && !p.isPlayerDead && !p.IsLocalPlayer && !p.disconnectedMidGame && p.playerClientId != LethalMenu.localPlayer.playerClientId),
                player => $"{(Settings.b_VCDisplay && player.voicePlayerState.IsSpeaking ? "[VC] " : "")}{player.playerUsername}",
                player => Settings.c_playerESP
            );
        }

        private void DisplayEnemyAI()
        {
            DisplayObjects(
                LethalMenu.enemies.Where(e => e != null && !e.isEnemyDead),
                enemy => enemy.enemyType.enemyName,
                enemy => Settings.c_enemyESP
            );
        }

        private void DisplayDeadBody()
        {
            DisplayObjects(
                LethalMenu.items.Where(i => i != null && !i.isHeld && !i.IsSpawned && i.itemProperties != null && i is RagdollGrabbableObject),
                item =>
                {
                    RagdollGrabbableObject body = item as RagdollGrabbableObject;
                    PlayerControllerB player = StartOfRound.Instance.allPlayerScripts[body.ragdoll.playerObjectId];
                    return player.playerUsername + "\n" + Settings.c_causeOfDeath.AsString(body.ragdoll.causeOfDeath.ToString());
                },
                _ => Settings.c_playerESP
            );
        }
        private void DisplayScrap()
        {
            DisplayObjects(
                LethalMenu.items.Where(i => i != null && !i.isHeld && !i.isPocketed && i.IsSpawned && i.itemProperties != null && i is not RagdollGrabbableObject),
                item => item.itemProperties.itemName + $" ({item.scrapValue}) ",
                item =>
                {
                    if(!Settings.b_useScrapTiers) return Settings.c_objectESP;
                    int index = Array.FindLastIndex<int>(Settings.i_scrapValueThresholds, x => x <= item.scrapValue);
                    return index > -1 ? Settings.c_scrapValueColors[index] : Settings.c_objectESP;
                }
            );
        }

        public void DisplaySteamHazards()
        {
            DisplayObjects(
                LethalMenu.steamValves.Where(v => v != null && v.triggerScript.interactable),
                valve => "Steam Valve",
                valve => Settings.c_steamHazardESP
            );
        }

        public void DisplayBigDoors()
        {
            DisplayObjects(
                LethalMenu.bigDoors,
                door => "Big Door [ " + door.objectCode + " ]",
                door => Settings.c_bigDoorESP
            );
        }

        public void DisplayDoorLocks()
        {
            DisplayObjects(
                LethalMenu.doorLocks.Where(d => d != null && d.isLocked),
                door => "Locked Door",
                door => Settings.c_doorLockESP
            );
        }

    }
}
