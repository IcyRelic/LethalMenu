using System;
using System.Collections.Generic;
using System.Linq;
using LethalMenu.Handler;
using LethalMenu.Types;
using LethalMenu.Util;
using UnityEngine;
using Object = UnityEngine.Object;


namespace LethalMenu.Cheats;

internal class ESP : Cheat
{
    public ESP()
    {
        ChamHandler.SetupChamMaterial();
    }

    public override void Update()
    {
        DisplayChams();
    }

    public override void OnGui()
    {
        if (!(bool)StartOfRound.Instance) return;

        var player = GameNetworkManager.Instance.localPlayerController;

        if (!player) return;

        try
        {
            if (Hack.ObjectEsp.IsEnabled()) DisplayScrap();
            if (Hack.EnemyEsp.IsEnabled()) DisplayEnemyAI();
            if (Hack.PlayerEsp.IsEnabled()) DisplayPlayers();
            if (Hack.DoorEsp.IsEnabled()) DisplayEntranceExitDoors();
            if (Hack.LandmineEsp.IsEnabled()) DisplayLandmines();
            if (Hack.TurretEsp.IsEnabled()) DisplayTurrets();
            if (Hack.ShipEsp.IsEnabled()) DisplayShip();
            if (Hack.BigDoorEsp.IsEnabled()) DisplayBigDoors();
            if (Hack.SteamHazardEsp.IsEnabled()) DisplaySteamHazards();
            if (Hack.DoorLockEsp.IsEnabled()) DisplayDoorLocks();
            if (Hack.BreakerEsp.IsEnabled()) DisplayBreaker();
        }
        catch (Exception e)
        {
            LethalMenu.DebugMessage = e.Message + "\n" + e.StackTrace;
        }
    }

    private static void DisplayChams<T>(IEnumerable<T> objects, Func<T, RgbaColor> colorSelector) where T : Object
    {
        objects.ToList().ForEach(o =>
        {
            Transform chamsObjectTransform;

            switch (o)
            {
                case Component component:
                    chamsObjectTransform = component.transform;
                    break;
                case GameObject chamsObject:
                    chamsObjectTransform = chamsObject.transform;
                    break;
                default:
                    return;
            }

            if (!o) return;

            var distance = GetDistanceToPlayer(chamsObjectTransform.position);
            o.GetChamHandler().ProcessCham(distance);
        });
    }

    private static void DisplayChams()
    {
        DisplayChams(LethalMenu.Items, _ => Settings.c_chams);
        DisplayChams(LethalMenu.Landmines, _ => Settings.c_chams);
        DisplayChams(LethalMenu.Turrets.ConvertAll(t => t.gameObject.transform.parent.gameObject),
            _ => Settings.c_chams);
        DisplayChams(LethalMenu.Players.Where(p => p.playerClientId != LethalMenu.LocalPlayer.playerClientId),
            _ => Settings.c_chams);
        DisplayChams(LethalMenu.Enemies, _ => Settings.c_chams);
        DisplayChams(LethalMenu.SteamValves, _ => Settings.c_chams);
        DisplayChams(LethalMenu.BigDoors, _ => Settings.c_chams);
        DisplayChams(LethalMenu.DoorLocks, _ => Settings.c_chams);
        DisplayChams(new[] { LethalMenu.ShipDoor }, _ => Settings.c_chams);
        DisplayChams(new[] { LethalMenu.Breaker }, _ => Settings.c_chams);
    }


    private static void DisplayObjects<T>(IEnumerable<T> objects, Func<T, string> labelSelector,
        Func<T, RgbaColor> colorSelector) where T : Component
    {
        foreach (var obj in objects)
            if (obj && obj.gameObject.activeSelf)
            {
                var distance = GetDistanceToPlayer(obj.transform.position);

                if (distance > Settings.f_espDistance ||
                    !WorldToScreen(obj.transform.position, out var screen)) continue;

                VisualUtil.DrawDistanceString(screen, labelSelector(obj), colorSelector(obj), distance);
            }
    }

    private static void DisplayTurrets()
    {
        DisplayObjects(
            LethalMenu.Turrets.Where(t => t && t.IsSpawned),
            turret => "Turret [ " + turret.GetComponent<TerminalAccessibleObject>().objectCode + " ]",
            turret => Settings.c_turretESP
        );
    }

    private static void DisplayShip()
    {
        DisplayObjects(
            new[] { LethalMenu.ShipDoor },
            ship => "Ship",
            ship => Settings.c_shipESP
        );
    }

    private static void DisplayBreaker()
    {
        DisplayObjects(
            new[] { LethalMenu.Breaker },
            breaker => "Breaker Box",
            breaker => Settings.c_breakerESP
        );
    }

    private static void DisplayEntranceExitDoors()
    {
        DisplayObjects(
            LethalMenu.Doors,
            door => door.isEntranceToBuilding ? "Entrance" : "Exit",
            door => Settings.c_entranceExitESP
        );
    }

    private static void DisplayLandmines()
    {
        DisplayObjects(
            LethalMenu.Landmines.Where(m => m && m.IsSpawned && !m.hasExploded),
            mine => "Landmine [ " + mine.GetComponent<TerminalAccessibleObject>().objectCode + " ]",
            mine => Settings.c_landmineESP
        );
    }

    private static void DisplayPlayers()
    {
        DisplayObjects(
            LethalMenu.Players
                .Where(p => p && !p.isPlayerDead && !p.IsLocalPlayer && !p.disconnectedMidGame &&
                            p.playerClientId != LethalMenu.LocalPlayer.playerClientId),
            player =>
                $"{(Settings.b_VCDisplay && player.voicePlayerState.IsSpeaking ? "[VC] " : "")}{player.playerUsername}",
            player => Settings.c_playerESP
        );
    }

    private static void DisplayEnemyAI()
    {
        DisplayObjects(
            LethalMenu.Enemies.Where(e => e && !e.isEnemyDead && e.GetEnemyAIType().IsEspEnabled()),
            enemy => enemy.enemyType.enemyName,
            enemy => Settings.c_enemyESP
        );
    }

    private void DisplayDeadBody()
    {
        DisplayObjects(
            LethalMenu.Items.Where(i =>
                i != null && !i.isHeld && !i.IsSpawned && i.itemProperties && i is RagdollGrabbableObject),
            item =>
            {
                var body = item as RagdollGrabbableObject;
                var player = StartOfRound.Instance.allPlayerScripts[body.ragdoll.playerObjectId];
                return player.playerUsername + "\n" +
                       Settings.c_causeOfDeath.AsString(body.ragdoll.causeOfDeath.ToString());
            },
            _ => Settings.c_playerESP
        );
    }

    private static void DisplayScrap()
    {
        DisplayObjects(
            LethalMenu.Items.Where(i =>
                i != null && !i.isHeld && !i.isPocketed && i.IsSpawned && i.itemProperties &&
                i is not RagdollGrabbableObject),
            item => item.itemProperties.itemName + $" ({item.scrapValue}) ",
            item =>
            {
                if (!Settings.b_useScrapTiers) return Settings.c_objectESP;
                var index = Array.FindLastIndex(Settings.i_scrapValueThresholds, x => x <= item.scrapValue);
                return index > -1 ? Settings.c_scrapValueColors[index] : Settings.c_objectESP;
            }
        );
    }

    public void DisplaySteamHazards()
    {
        DisplayObjects(
            LethalMenu.SteamValves.Where(v => v && v.triggerScript.interactable),
            valve => "Steam Valve",
            valve => Settings.c_steamHazardESP
        );
    }

    public void DisplayBigDoors()
    {
        DisplayObjects(
            LethalMenu.BigDoors,
            door => "Big Door [ " + door.objectCode + " ]",
            door => Settings.c_bigDoorESP
        );
    }

    public void DisplayDoorLocks()
    {
        DisplayObjects(
            LethalMenu.DoorLocks.Where(d => d && d.isLocked),
            door => "Locked Door",
            door => Settings.c_doorLockESP
        );
    }
}