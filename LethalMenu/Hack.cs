﻿using GameNetcodeStuff;
using LethalMenu.Cheats;
using LethalMenu.Handler;
using LethalMenu.Language;
using LethalMenu.Manager;
using LethalMenu.Types;
using LethalMenu.Util;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using Vector3 = UnityEngine.Vector3;

namespace LethalMenu
{
    public enum Hack
    {
        /** General stuff **/
        OpenMenu,
        UnloadMenu,
        ToggleCursor,

        /** Self Tab **/
        GodMode,
        SuperJump,
        FastClimb,
        NightVision,
        UnlimitedStamina,
        UnlimitedBattery,
        UnlimitedOxygen,
        UnlimitedZapGun,
        LootThroughWalls,
        InteractThroughWalls,
        Reach,
        AntiGhostGirl,
        UnlockObjects,
        UnlockObjectsAction,
        SuperSpeed,
        NoClip,
        Experience,
        Teleport,
        TeleportShip,
        TeleportSavedPosition,
        SaveTeleportPosition,
        BuildAnywhere,
        Weight,
        GhostMode,
        UnlimitedAmmo,
        FreeCam,
        NoCooldown,
        InstantInteract,
        SuperShovel,
        StrongHands,
        Invisibility,
        NoFallDamage,
        HearAllAlivePeople,
        HearAllDeadPeople,
        NoFlash,
        NoCameraShake,
        NoQuicksand,
        TeleportWithItems,
        SuperKnife,
        BridgeNeverFalls,
        DeleteHeldItem,
        DropAllItems,
        UnlimitedPresents,
        UnlimitedTZP,
        NoTZPEffects,
        VoteShipLeaveEarly,
        VehicleGodMode,
        EggsNeverExplode,
        EggsAlwaysExplode,
        UnlockAllDoors,
        OpenAllBigDoors,
        CloseAllBigDoors,
        NoShipDoorClose,
        LootBeforeGameStarts,
        ClickTeleport,
        ClickTeleportAction,
        ClickKill,
        ClickKillAction,
        LootAnyItemBeltBag,
        LootThroughWallsBeltBag,
        AntiKick,
        SellQuota,
        SellEverything,
        TeleportAllItems,
        TeleportOneItem,
        FixAllValves,
        FullRenderResolution,
        GrabNutcrackerShotgun,
        MinigunShotgun,

        /** Server Tab **/
        ToggleAllDisplays,
        DisplayBodies,
        DisplayEnemies,
        DisplayMapObjects,
        DisplayShipObjects,
        DisplayQuota,
        DisplayDeadline,
        DisplayCredits,
        ModifyCredits,
        ModifyQuota,
        EndGame,
        StartGame,
        SpawnMoreScrap,
        Shoplifter,
        ShowOffensiveLobbyNames,
        JoinLobby,
        Disconnect,
        Message,
        ResetShip,
        ItemSlots,
        ForceMeteorShower,
        ClearMeteorShower,
        OpenDropShipLand,
        ModifyScrap,
        ModifyDeadline,

        /** Troll Tab **/
        ToggleShipLights,
        ToggleFactoryLights,
        ToggleShipHorn,
        ToggleCarHorn,
        ToggleTerminalSound,
        ToggleDepositDeskDoorSound,
        ForceBridgeFall,
        ForceSmallBridgeFall,
        BlowUpAllLandmines,
        ToggleAllLandmines,
        ToggleAllTurrets,
        KillAllEnemies,
        KillNearbyEnemies,
        StunAllEnemies,
        ForceTentacleAttack,
        SpawnMaskedEnemy,
        BreakAllWebs,
        SpawnLandmine,
        SpawnTurret,
        SpawnSpikeRoofTrap,
        SpawnMapObjects,
        EjectEveryone,
        OpenShipDoorSpace,
        BerserkAllTurrets,
        PJSpammer,
        SpawnHoardingBugInfestation,
        ToggleMineshaftElevator,
        ToggleVehicleMagnet,
        SpamShootAllShotguns,
        ShootAllShotguns,
        ExplodeCruiser,

        /** Visuals Tab **/
        ToggleAllESP,
        NameESP,
        BoxESP,
        ObjectESP,
        EnemyESP,
        PlayerESP,
        BodyESP,
        DoorESP,
        LandmineESP,
        TurretESP,
        ShipESP,
        BreakerESP,
        SteamHazardESP,
        BigDoorESP,
        DoorLockESP,
        SpikeRoofTrapESP,
        VainShroudESP,
        CruiserESP,
        ItemDropShipESP,
        AlwaysShowClock,
        SimpleClock,
        Crosshair,
        Breadcrumbs,
        NoVisor,
        NoFieldOfDepth,
        FOV,
        HPDisplay,
        MineshaftElevatorESP,
        EnemyVentESP,

        /** Player Tab **/
        KillPlayer,
        DemiGod,
        HealPlayer,
        LightningStrikePlayer,
        DeathNotifications,
        EnemyDeathNotifications,
        DeathNotify,
        EnemyDeathNotify,
        SpectatePlayer,
        MiniCam,
        TeleportAllEnemies,
        TeleportEnemy,
        SpiderWebPlayer,
        LureAllEnemies,
        ExplodeClosestMine,
        EnemyControl,
    }

    public static class HackExtensions
    {
        private static readonly Hack[] KeyBindIgnore = new Hack[]
        {
            Hack.ModifyQuota,
            Hack.ModifyCredits,
            Hack.KillPlayer,
            Hack.HealPlayer,
            Hack.LightningStrikePlayer,
            Hack.Experience,
            Hack.Teleport,
            Hack.DeathNotify,
            Hack.SpectatePlayer,
            Hack.MiniCam,
            Hack.ModifyScrap,
            Hack.ModifyDeadline,
            Hack.Message,
            Hack.SpiderWebPlayer,
            Hack.TeleportAllEnemies,
            Hack.TeleportEnemy,
            Hack.EnemyControl,
            Hack.LureAllEnemies,
            Hack.ExplodeClosestMine,
            Hack.SpawnMapObjects,
        };

        private static List<Hack> Waiting = new List<Hack>();

        public static readonly Dictionary<Hack, bool> ToggleFlags = new Dictionary<Hack, bool>()
        {
            {Hack.OpenMenu, false},
            {Hack.ToggleCursor, false},
            {Hack.GodMode, false},
            {Hack.SuperJump, false},
            {Hack.FastClimb, false},
            {Hack.NightVision, false},
            {Hack.UnlimitedStamina, false},
            {Hack.UnlimitedBattery, false},
            {Hack.UnlimitedOxygen, false},
            {Hack.LootThroughWalls, false},
            {Hack.InteractThroughWalls, false},
            {Hack.Reach, false},
            {Hack.UnlockObjects, false},
            {Hack.AlwaysShowClock, false},
            {Hack.SimpleClock, false},
            {Hack.SuperSpeed, false},
            {Hack.NoClip, false},
            {Hack.NameESP, false},
            {Hack.BoxESP, false},
            {Hack.ObjectESP, false},
            {Hack.EnemyESP, false},
            {Hack.PlayerESP, false},
            {Hack.BodyESP, false},
            {Hack.DoorESP, false},
            {Hack.LandmineESP, false},
            {Hack.TurretESP, false},
            {Hack.ShipESP, false},
            {Hack.BreakerESP, false},
            {Hack.SteamHazardESP, false},
            {Hack.BigDoorESP, false},
            {Hack.DoorLockESP, false},
            {Hack.SpikeRoofTrapESP, false},
            {Hack.EnemyVentESP, false},
            {Hack.VainShroudESP, false},
            {Hack.CruiserESP, false},
            {Hack.ItemDropShipESP, false},
            {Hack.Crosshair, false},
            {Hack.DisplayBodies, false},
            {Hack.DisplayEnemies, false},
            {Hack.DisplayMapObjects, false},
            {Hack.DisplayShipObjects, false},
            {Hack.DisplayQuota, false},
            {Hack.DisplayDeadline, false},
            {Hack.DisplayCredits, false},
            {Hack.ToggleAllLandmines, false},
            {Hack.ToggleAllTurrets, false},
            {Hack.ToggleShipHorn, false},
            {Hack.ToggleCarHorn, false},
            {Hack.Breadcrumbs, false},
            {Hack.DeathNotifications, false},
            {Hack.EnemyDeathNotifications, false},
            {Hack.BuildAnywhere, false},
            {Hack.Weight, false},
            {Hack.GhostMode, false},
            {Hack.UnlimitedAmmo, false},
            {Hack.FreeCam, false},
            {Hack.SpectatePlayer, false},
            {Hack.MiniCam, false},
            {Hack.EnemyControl, false},
            {Hack.NoCooldown, false},
            {Hack.InstantInteract, false},
            {Hack.SuperShovel, false},
            {Hack.StrongHands, false},
            {Hack.Invisibility, false},
            {Hack.NoFallDamage, false},
            {Hack.Shoplifter, false},
            {Hack.HearAllAlivePeople, false},
            {Hack.HearAllDeadPeople, false},
            {Hack.NoVisor, false},
            {Hack.NoFieldOfDepth, false},
            {Hack.NoFlash, false},
            {Hack.TeleportWithItems, false},
            {Hack.OpenShipDoorSpace, false},
            {Hack.UnlimitedPresents, false},
            {Hack.ShowOffensiveLobbyNames, false},
            {Hack.NoCameraShake, false},
            {Hack.SuperKnife, false},
            {Hack.NoQuicksand, false},
            {Hack.BerserkAllTurrets, false},
            {Hack.BridgeNeverFalls, false},
            {Hack.VehicleGodMode, false},
            {Hack.EggsNeverExplode, false},
            {Hack.UnlimitedZapGun, false},
            {Hack.ToggleTerminalSound, false},
            {Hack.LootBeforeGameStarts, false},
            {Hack.ClickTeleport, false},
            {Hack.ClickKill, false},
            {Hack.PJSpammer, false},
            {Hack.ItemSlots, false},
            {Hack.FOV, false},
            {Hack.AntiGhostGirl, false},
            {Hack.EggsAlwaysExplode, false},
            {Hack.NoShipDoorClose, false},
            {Hack.OpenDropShipLand, false},
            {Hack.LootAnyItemBeltBag, false},
            {Hack.LootThroughWallsBeltBag, false},
            {Hack.UnlimitedTZP, false},
            {Hack.NoTZPEffects, false},
            {Hack.HPDisplay, false},
            {Hack.MineshaftElevatorESP, false},
            {Hack.AntiKick, false},
            {Hack.DemiGod, false},
            {Hack.FullRenderResolution, false},
            {Hack.ToggleDepositDeskDoorSound, false},
            {Hack.GrabNutcrackerShotgun, false},
            {Hack.MinigunShotgun, false},
            {Hack.SpamShootAllShotguns, false},
        };

        private static readonly Dictionary<Hack, Delegate> Executors = new Dictionary<Hack, Delegate>()
        {
            {Hack.ToggleCursor, (Action) HackExecutor.ToggleCursor},
            {Hack.OpenMenu, (Action) HackExecutor.OpenMenu},
            {Hack.UnloadMenu, (Action) HackExecutor.UnloadMenu},
            {Hack.Experience, (Action<int, ActionType>) HackExecutor.ModExperience},
            {Hack.Teleport, (Action<Vector3, bool, bool, bool>) HackExecutor.Teleport},
            {Hack.TeleportShip, (Action) HackExecutor.TeleportShip},
            {Hack.TeleportSavedPosition, (Action) HackExecutor.TeleportSavedPosition},
            {Hack.SaveTeleportPosition, (Action) HackExecutor.SaveTeleportPosition},
            {Hack.ModifyCredits, (Action<int, ActionType>) HackExecutor.ModCredits},
            {Hack.ModifyQuota, (Action<int>) HackExecutor.ModQuota},
            {Hack.ModifyDeadline, (Action<int>) HackExecutor.ModDeadline},
            {Hack.Message, (Action<string, int, int>) HackExecutor.Message},
            {Hack.SpawnMoreScrap, (Action) HackExecutor.SpawnMoreScrap},
            {Hack.EndGame, (Action) HackExecutor.EndGame},
            {Hack.StartGame, (Action) HackExecutor.StartGame},
            {Hack.ToggleShipLights, (Action) HackExecutor.ToggleShipLights},
            {Hack.ToggleCarHorn, (Action) HackExecutor.ToggleCarHorn},
            {Hack.ToggleFactoryLights, (Action) HackExecutor.ToggleFactoryLights},
            {Hack.ToggleShipHorn, (Action) HackExecutor.ToggleShipHorn},
            {Hack.ForceBridgeFall, (Action) HackExecutor.ForceBridgeFall},
            {Hack.ForceSmallBridgeFall, (Action) HackExecutor.ForceSmallBridgeFall},
            {Hack.BlowUpAllLandmines, (Action) HackExecutor.BlowUpAllLandmines},
            {Hack.KillAllEnemies, (Action) HackExecutor.KillAllEnemies},
            {Hack.KillNearbyEnemies, (Action<int>) HackExecutor.KillNearbyEnemies},
            {Hack.StunAllEnemies, (Action) HackExecutor.StunAllEnemies},
            {Hack.ForceTentacleAttack, (Action) HackExecutor.ForceTentacleAttack},
            {Hack.ToggleAllLandmines, (Action) HackExecutor.ToggleAllLandmines},
            {Hack.ToggleAllTurrets, (Action) HackExecutor.ToggleAllTurrets},
            {Hack.KillPlayer, (Action<PlayerControllerB>) HackExecutor.KillPlayer},
            {Hack.HealPlayer, (Action<PlayerControllerB>) HackExecutor.HealPlayer},
            {Hack.DemiGod, (Action<PlayerControllerB>) HackExecutor.DemiGod},
            {Hack.LightningStrikePlayer, (Action<PlayerControllerB>) HackExecutor.LightningStrikePlayer},
            {Hack.UnlockObjectsAction, (Action) HackExecutor.UnlockObjects},
            {Hack.ToggleAllESP, (Action) HackExecutor.ToggleAllESP},
            {Hack.DeathNotify, (Action<PlayerControllerB, CauseOfDeath>) HackExecutor.NotifyDeath},
            {Hack.EnemyDeathNotify, (Action<EnemyType>) HackExecutor.NotifyEnemyDeath},
            {Hack.SpectatePlayer, (Action<PlayerControllerB>) HackExecutor.SpectatePlayer},
            {Hack.MiniCam, (Action<PlayerControllerB>) HackExecutor.MiniCam},
            {Hack.FixAllValves, (Action) HackExecutor.FixAllValves},
            {Hack.ModifyScrap, (Action<int, int>) HackExecutor.ModScrap},
            {Hack.TeleportAllEnemies, (Action<PlayerControllerB, EnemyAI[]>) HackExecutor.TeleportAllEnemies},
            {Hack.TeleportEnemy, (Action<PlayerControllerB, EnemyAI>) HackExecutor.TeleportEnemy},
            {Hack.EnemyControl, (Action<EnemyAI>) HackExecutor.ControlEnemy},
            {Hack.SpawnMaskedEnemy, (Action) HackExecutor.SpawnMaskedEnemy},
            {Hack.SpiderWebPlayer, (Action<PlayerControllerB>) HackExecutor.SpiderWebPlayer},
            {Hack.BreakAllWebs, (Action) HackExecutor.BreakAllWebs},
            {Hack.LureAllEnemies, (Action<PlayerControllerB>) HackExecutor.LureAllEnemies},
            {Hack.SpawnMapObjects, (Action<MapObject>) HackExecutor.SpawnMapObjects},
            {Hack.SpawnLandmine, (Action) HackExecutor.SpawnLandmine},
            {Hack.SpawnTurret, (Action) HackExecutor.SpawnTurret},
            {Hack.SpawnSpikeRoofTrap, (Action) HackExecutor.SpawnSpikeRoofTrap},
            {Hack.ExplodeClosestMine, (Action<PlayerControllerB>) HackExecutor.ExplodeClosestMine},
            {Hack.SellQuota, (Action) HackExecutor.SellQuota},
            {Hack.SellEverything, (Action) HackExecutor.SellEverything},
            {Hack.TeleportAllItems, (Action) HackExecutor.TeleportAllItems},
            {Hack.TeleportOneItem, (Action) HackExecutor.TeleportOneItem},
            {Hack.EjectEveryone, (Action) HackExecutor.EjectEveryone},
            {Hack.JoinLobby, (Action<SteamId>) HackExecutor.JoinLobby},
            {Hack.Disconnect, (Action) HackExecutor.Disconnect},
            {Hack.VoteShipLeaveEarly, (Action) HackExecutor.VoteShipLeaveEarly},
            {Hack.UnlockAllDoors, (Action) HackExecutor.UnlockAllDoors},
            {Hack.DropAllItems, (Action) HackExecutor.DropAllItems},
            {Hack.BerserkAllTurrets, (Action) HackExecutor.BerserkAllTurrets},
            {Hack.ResetShip, (Action) HackExecutor.ResetShip},
            {Hack.ToggleAllDisplays, (Action) HackExecutor.ToggleAllDisplays},
            {Hack.ToggleTerminalSound, (Action) HackExecutor.ToggleTerminalSound},
            {Hack.DeleteHeldItem, (Action) HackExecutor.DeleteHeldItem},
            {Hack.ClickTeleportAction, (Action) HackExecutor.ClickTeleport},
            {Hack.ClickKillAction, (Action) HackExecutor.ClickKill},
            {Hack.ForceMeteorShower, (Action) HackExecutor.ForceMeteorShower},
            {Hack.ClearMeteorShower, (Action) HackExecutor.ClearMeteorShower},
            {Hack.OpenAllBigDoors, (Action) HackExecutor.OpenAllBigDoors},
            {Hack.CloseAllBigDoors, (Action) HackExecutor.CloseAllBigDoors},
            {Hack.SpawnHoardingBugInfestation, (Action) HackExecutor.SpawnHoardingBugInfestation},
            {Hack.ToggleMineshaftElevator, (Action) HackExecutor.ToggleMineshaftElevator},
            {Hack.ToggleVehicleMagnet, (Action) HackExecutor.ToggleVehicleMagnet},
            {Hack.SpamShootAllShotguns, (Action) HackExecutor.SpamShootAllShotguns},
            {Hack.ShootAllShotguns, (Action) HackExecutor.ShootAllShotguns},
            {Hack.ExplodeCruiser, (Action) HackExecutor.ExplodeCruiser},
        };

        public static readonly Dictionary<Hack, ButtonControl> KeyBinds = new Dictionary<Hack, ButtonControl>()
        {
            {Hack.OpenMenu, Keyboard.current.insertKey},
            {Hack.ToggleCursor, Keyboard.current.altKey},
            {Hack.UnloadMenu, Keyboard.current.pauseKey},
            {Hack.UnlockObjectsAction, Keyboard.current.f1Key},
            {Hack.ClickTeleportAction, Mouse.current.middleButton},
            {Hack.ClickKillAction, Mouse.current.middleButton}
        };

        public static void Execute(this Hack hack, params object[] param)
        {
            if (hack.CanBeToggled()) hack.Toggle();
            if (Executors.TryGetValue(hack, out var method))
            {
                if (method is Action action) action.Invoke();
                else method.DynamicInvoke(param);
            }
        }

        public static void Invoke(this Hack hack, params object[] param)
        {
            if (Executors.TryGetValue(hack, out var method))
            {
                if (method is Action action) action.Invoke();
                else method.DynamicInvoke(param);
            }
        }

        public static bool CanBeToggled(this Hack hack)
        {
            return ToggleFlags.ContainsKey(hack);
        }

        public static bool CanBeExecuted(this Hack hack)
        {
            return Executors.ContainsKey(hack);
        }

        public static bool IsEnabled(this Hack hack)
        {
            return ToggleFlags[hack];
        }

        public static void Toggle(this Hack hack)
        {
            ToggleFlags[hack] = !ToggleFlags[hack];
        }

        public static void SetToggle(this Hack hack, bool b)
        {
            ToggleFlags[hack] = b;
        }

        public static void RemoveKeyBind(this Hack hack)
        {
            if (KeyBinds.ContainsKey(hack)) KeyBinds.Remove(hack);
        }

        public static void SetKeyBind(this Hack hack, ButtonControl btn)
        {
            if (KeyBindIgnore.Contains(hack) || btn == null) return;
            if (KeyBinds.ContainsKey(hack)) KeyBinds[hack] = btn;
            else KeyBinds.Add(hack, btn);
        }

        public static bool Button(this Hack hack)
        {
            return GUILayout.Button(Localization.Localize(hack.ToBtnText()));
        }

        public static string ToBtnText(this Hack hack)
        {
            return hack.CanBeToggled() ? ToggleFlags[hack] ? "General.Disable" : "General.Enable" : "General.Execute";
        }

        public static ButtonControl GetKeyBind(this Hack hack)
        {
            return KeyBinds.ContainsKey(hack) ? KeyBinds[hack] : null;
        }

        public static bool HasKeyBind(this Hack hack)
        {
            return KeyBinds.ContainsKey(hack);
        }

        public static bool CanHaveKeyBind(this Hack hack)
        {
            return !KeyBindIgnore.Contains(hack);
        }

        public static bool IsWaiting(this Hack hack)
        {
            return Waiting.Contains(hack);
        }

        public static bool IsAnyHackWaiting(this Hack hack)
        {
            return Waiting.Count > 0;
        }

        public static void SetWaiting(this Hack hack, bool b)
        {
            if (b && !Waiting.Contains(hack)) Waiting.Add(hack);
            if (!b && Waiting.Contains(hack)) Waiting.Remove(hack);
        }

        public static bool KeyBindInUse(ButtonControl btn)
        {
            foreach (Hack hack in KeyBinds.Keys)
            {
                if (KeyBinds[hack] == btn) return true;
            }
            return false;
        }
    }

    public class HackExecutor
    {
        public static void ToggleAllESP()
        {
            Hack.ObjectESP.Execute();
            Hack.EnemyESP.Execute();
            Hack.PlayerESP.Execute();
            Hack.BodyESP.Execute();
            Hack.DoorESP.Execute();
            Hack.LandmineESP.Execute();
            Hack.TurretESP.Execute();
            Hack.ShipESP.Execute();
            Hack.SteamHazardESP.Execute();
            Hack.BigDoorESP.Execute();
            Hack.DoorLockESP.Execute();
            Hack.BreakerESP.Execute();
            Hack.SpikeRoofTrapESP.Execute();
            Hack.MineshaftElevatorESP.Execute();
            Hack.EnemyVentESP.Execute();
            Hack.VainShroudESP.Execute();
            Hack.CruiserESP.Execute();
            Hack.ItemDropShipESP.Execute();
        }

        public static void ToggleAllDisplays()
        {
            Hack.DisplayBodies.Execute();
            Hack.DisplayDeadline.Execute();
            Hack.DisplayEnemies.Execute();
            Hack.DisplayMapObjects.Execute();
            Hack.DisplayQuota.Execute();
            Hack.DisplayShipObjects.Execute();
            Hack.DisplayCredits.Execute();
        }

        public static void ModExperience(int amt, ActionType type)
        {
            int newAmt = amt;
            if (type == ActionType.Add) newAmt = HUDManager.Instance.localPlayerXP + amt;
            if (type == ActionType.Remove) newAmt = HUDManager.Instance.localPlayerXP - amt;
            HUDManager.Instance.localPlayerXP = newAmt;
        }

        public static void ToggleCursor()
        {
            if (!(bool)StartOfRound.Instance) return;
            if (Cursor.visible) MenuUtil.HideCursor();
            else MenuUtil.ShowCursor();
        }

        public static void OpenMenu()
        {
            if (Hack.OpenMenu.IsEnabled()) MenuUtil.ShowCursor();
            else MenuUtil.HideCursor();
        }

        public static void ClickKill() => RoundHandler.ClickKill();
        public static void ClickTeleport() => RoundHandler.ClickTeleport();
        public static void UnlockObjects() => RoundHandler.UnlockObjects();
        public static void UnloadMenu() => LethalMenu.Instance.Unload();
        public static void NotifyDeath(PlayerControllerB died, CauseOfDeath cause) => HUDManager.Instance.DisplayTip("Lethal Menu", $"{died.playerUsername} has died from {cause.ToString()}");
        public static void NotifyEnemyDeath(EnemyType enemy) => HUDManager.Instance.DisplayTip("Lethal Menu", $"{enemy.name} has died");
        public static void SpawnEnemy(EnemyType type, int num, bool outside) => RoundHandler.SpawnEnemy(type, num, outside);
        public static void SpawnMaskedEnemy() => RoundHandler.SpawnMimicFromMasks();
        public static void ToggleAllLandmines() => RoundHandler.ToggleAllLandmines();
        public static void ToggleAllTurrets() => RoundHandler.ToggleAllTurrets();
        public static void BlowUpAllLandmines() => RoundHandler.BlowUpAllLandmines();
        public static void FixAllValves() => RoundHandler.FixAllValves();
        public static void ToggleShipHorn() => RoundHandler.ToggleShipHorn();
        public static void ForceBridgeFall() => RoundHandler.ForceBridgeFall();
        public static void ForceSmallBridgeFall() => RoundHandler.ForceSmallBridgeFall();
        public static void ForceTentacleAttack() => RoundHandler.ForceTentacleAttack();
        public static void ModScrap(int value, int type) => RoundHandler.ModScrap(value, type);
        public static void SpawnMoreScrap() => RoundHandler.SpawnScrap();
        public static void ResetShip() => RoundHandler.ResetShip();
        public static void EndGame() => RoundHandler.EndGame();
        public static void StartGame() => RoundHandler.StartGame();
        public static void ModQuota(int amt) => RoundHandler.SetQuota(amt);
        public static void ModDeadline(int amt) => RoundHandler.SetDeadline(amt);
        public static void Message(string msg, int type, int id) => RoundHandler.Message(msg, type, id);
        public static void ModCredits(int amt, ActionType type) => RoundHandler.ModCredits(amt, type);
        public static void BreakAllWebs() => RoundHandler.BreakAllWebs();
        public static void SpawnLandmine() => RoundHandler.SpawnMapObject(MapObject.Landmine);
        public static void SpawnTurret() => RoundHandler.SpawnMapObject(MapObject.TurretContainer);
        public static void SpawnSpikeRoofTrap() => RoundHandler.SpawnMapObject(MapObject.SpikeRoofTrapHazard);
        public static void SpawnMapObjects(MapObject type) => RoundHandler.SpawnMapObjects(type);
        public static void SellQuota() => LethalMenu.localPlayer.Handle().SellQuota();
        public static void SellEverything() => LethalMenu.localPlayer.Handle().PlaceEverythingOnDesk();
        public static void ExplodeClosestMine(PlayerControllerB player) => player.Handle().ExplodeClosestLandmine();
        public static void LureAllEnemies(PlayerControllerB player) => player.Handle().LureAllEnemies();
        public static void SpiderWebPlayer(PlayerControllerB player) => player.Handle().SpawnSpiderWebs(6);
        public static void SpectatePlayer(PlayerControllerB player) => player.Handle().Spectate();
        public static void MiniCam(PlayerControllerB player) => player.Handle().MiniCam();
        public static void SaveTeleportPosition() => LethalMenu.localPlayer.Handle().SavePosition();
        public static void TeleportShip() => LethalMenu.localPlayer.Handle().TeleportShip();
        public static void TeleportSavedPosition() => LethalMenu.localPlayer.Handle().TeleportSaved();
        public static void Teleport(Vector3 pos, bool elevator = false, bool ship = false, bool factory = false) => LethalMenu.localPlayer.Handle().Teleport(pos, elevator, ship, factory);
        public static void KillPlayer(PlayerControllerB player) => player.Handle().Kill();
        public static void HealPlayer(PlayerControllerB player) => player.Handle().Heal();
        public static void DemiGod(PlayerControllerB player) => DemiGodCheat.ToggleDemiGod(player);
        public static void LightningStrikePlayer(PlayerControllerB player) => player.Handle().Strike();
        public static void SpawnHoardingBugInfestation() => RoundHandler.SpawnHoardingBugInfestation();
        public static void ControlEnemy(EnemyAI e) => e.Handle().Control();
        public static void TeleportEnemy(PlayerControllerB player, EnemyAI e) => e.Handle().Teleport(player);
        public static void TeleportAllEnemies(PlayerControllerB player, EnemyAI[] enemies) => enemies.ToList().FindAll(e => !e.isEnemyDead).ForEach(e => e.Handle().Teleport(player));
        public static void StunAllEnemies() => RoundHandler.StunAll();
        public static void KillAllEnemies() => RoundHandler.KillAll();
        public static void KillNearbyEnemies(int distance = -1) => LethalMenu.enemies.FindAll(e => GameUtil.GetDistanceToPlayer(e.transform.position) <= distance).ForEach(enemy => enemy.Handle().Kill());
        public static void ToggleShipLights() => RoundHandler.ToggleShipLights();
        public static void ToggleCarHorn() => RoundHandler.ToggleCarHorn();
        public static void ToggleFactoryLights() => RoundHandler.ToggleFactoryLights();
        public static void TeleportAllItems() => RoundHandler.TeleportAllItems();
        public static void TeleportOneItem() => RoundHandler.TeleportOneItem();
        public static void EjectEveryone() => StartOfRound.Instance.ManuallyEjectPlayersServerRpc();
        public static void JoinLobby(SteamId id) => RoundHandler.JoinLobby(id);
        public static void Disconnect() => RoundHandler.Disconnect();
        public static void VoteShipLeaveEarly() => TimeOfDay.Instance.VoteShipToLeaveEarly();
        public static void BerserkAllTurrets() => RoundHandler.BerserkAllTurrets();
        public static void DropAllItems() => RoundHandler.DropAllItems();
        public static void ToggleTerminalSound() => LethalMenu.Instance.StartCoroutine(RoundHandler.ToggleTerminalSound());
        public static void DeleteHeldItem() => RoundHandler.DeleteHeldItem();
        public static void UnlockAllDoors() => RoundHandler.UnlockAllDoors();
        public static void OpenAllBigDoors() => RoundHandler.OpenAllBigDoors();
        public static void CloseAllBigDoors() => RoundHandler.CloseAllBigDoors();
        public static void ForceMeteorShower() => RoundHandler.ForceMeteorShower();
        public static void ClearMeteorShower() => RoundHandler.ClearMeteorShower();
        public static void ToggleMineshaftElevator() => RoundHandler.ToggleMineshaftElevator();
        public static void ToggleVehicleMagnet() => RoundHandler.ToggleVehicleMagnet();
        public static void SpamShootAllShotguns() => LethalMenu.Instance.StartCoroutine(RoundHandler.SpamShootAllShotguns());
        public static void ShootAllShotguns() => RoundHandler.ShootAllShotguns();
        public static void ExplodeCruiser() => RoundHandler.ExplodeCruiser();
    }
}