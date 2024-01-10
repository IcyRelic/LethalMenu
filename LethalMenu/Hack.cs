using GameNetcodeStuff;
using LethalMenu.Handler;
using LethalMenu.Manager;
using LethalMenu.Types;
using LethalMenu.Util;
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
        LootThroughWalls,
        InteractThroughWalls,
        Reach,
        UnlockDoors,
        UnlockDoorAction,
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
        //UnlimitedPresents,
        UnlimitedAmmo,
        FreeCam,
        NoCooldown,
        InstantInteract,
        SuperShovel,
        StrongHands,
        Invisibility,
        AntiKick,

        /** Server Tab **/
        DisplayBodyCount,
        DisplayEnemyCount,
        DisplayObjectScan,
        DisplayShipScan,
        DisplayQuota,
        DisplayBuyingRate,
        ModifyCredits,
        ModifyQuota,
        EndGame,
        StartGame,
        SpawnMoreScrap,
        UnlockUnlockable,

        /** Troll Tab **/
        ToggleShipLights,
        ToggleFactoryLights,
        ToggleShipHorn,
        ForceBridgeFall,
        BlowUpAllLandmines,
        ToggleAllLandmines,
        ToggleAllTurrets,
        KillAllEnemies,
        KillNearbyEnemies,
        StunAllEnemies,
        TeleportAllItems,
        ForceTentacleAttack,
        FlickerLights,
        FixAllValves,
        ModifyScrap,
        SpawnMaskedEnemy,
        BreakAllWebs,
        SpawnLandmine,
        SpawnTurret,
        SpawnMapObjects,
        SellEverything,

        /** Visuals Tab **/
        ToggleAllESP,
        ObjectESP,
        EnemyESP,
        PlayerESP,
        DoorESP,
        LandmineESP,
        TurretESP,
        ShipESP,
        BreakerESP,
        SteamHazardESP,
        BigDoorESP,
        DoorLockESP,
        AlwaysShowClock,
        SimpleClock,
        Crosshair,
        Breadcrumbs,
        NoFog,

        /** Player Tab **/
        KillPlayer,
        HealPlayer,
        LightningStrikePlayer,
        DeathNotifications,
        DeathNotify,
        SpectatePlayer,
        MiniCam,
        TeleportEnemy,
        SpiderWebPlayer,
        LureAllEnemies,
        ExplodeClosestMine,

        EnemyControl
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
            Hack.UnlockUnlockable,
            Hack.ModifyScrap,
            Hack.SpiderWebPlayer,
            Hack.TeleportEnemy,
            Hack.EnemyControl,
            Hack.LureAllEnemies,
            Hack.ExplodeClosestMine,
            Hack.SpawnMapObjects
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
            {Hack.LootThroughWalls, false},
            {Hack.InteractThroughWalls, false},
            {Hack.Reach, false},
            {Hack.UnlockDoors, false},
            {Hack.AlwaysShowClock, false},
            {Hack.SimpleClock, false},
            {Hack.SuperSpeed, false},
            {Hack.NoClip, false},
            {Hack.ObjectESP, false},
            {Hack.EnemyESP, false},
            {Hack.PlayerESP, false},
            {Hack.DoorESP, false},
            {Hack.LandmineESP, false},
            {Hack.TurretESP, false},
            {Hack.ShipESP, false},
            {Hack.BreakerESP, false},
            {Hack.SteamHazardESP, false},
            {Hack.BigDoorESP, false},
            {Hack.DoorLockESP, false},
            {Hack.Crosshair, false},
            {Hack.DisplayBodyCount, false},
            {Hack.DisplayEnemyCount, false},
            {Hack.DisplayObjectScan, false},
            {Hack.DisplayShipScan, false},
            {Hack.DisplayQuota, false},
            {Hack.DisplayBuyingRate, false},
            {Hack.ToggleAllLandmines, false},
            {Hack.ToggleAllTurrets, false},
            {Hack.ToggleShipHorn, false},
            {Hack.Breadcrumbs, false},
            {Hack.DeathNotifications, false},
            {Hack.NoFog, false},
            {Hack.BuildAnywhere, false},
            {Hack.Weight, false},
            {Hack.GhostMode, false},
            //{Hack.UnlimitedPresents, false},
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
            {Hack.AntiKick, false},


        };

        private static readonly Dictionary<Hack, Delegate> Executors = new Dictionary<Hack, Delegate>()
        {
            {Hack.ToggleCursor, (Action) HackExecutor.ToggleCursor},
            {Hack.UnloadMenu, (Action) HackExecutor.UnloadMenu},

            {Hack.Experience, (Action<int, ActionType>) HackExecutor.ModExperience},
            {Hack.Teleport, (Action<Vector3, bool, bool, bool>) HackExecutor.Teleport},
            {Hack.TeleportShip, (Action) HackExecutor.TeleportShip},
            {Hack.TeleportSavedPosition, (Action) HackExecutor.TeleportSavedPosition},
            {Hack.SaveTeleportPosition, (Action) HackExecutor.SaveTeleportPosition},
            {Hack.ModifyCredits, (Action<int, ActionType>) HackExecutor.ModCredits},
            {Hack.ModifyQuota, (Action<int>) HackExecutor.ModQuota},
            {Hack.SpawnMoreScrap, (Action) HackExecutor.SpawnMoreScrap},
            {Hack.EndGame, (Action) HackExecutor.EndGame},
            {Hack.StartGame, (Action) HackExecutor.StartGame},
            {Hack.ToggleShipLights, (Action) HackExecutor.ToggleShipLights},
            {Hack.ToggleFactoryLights, (Action) HackExecutor.ToggleFactoryLights},
            {Hack.ToggleShipHorn, (Action) HackExecutor.ToggleShipHorn},
            {Hack.ForceBridgeFall, (Action) HackExecutor.ForceBridgeFall},
            {Hack.BlowUpAllLandmines, (Action) HackExecutor.BlowUpAllLandmines},
            {Hack.KillAllEnemies, (Action) HackExecutor.KillAllEnemies},
            {Hack.KillNearbyEnemies, (Action<int>) HackExecutor.KillNearbyEnemies},
            {Hack.StunAllEnemies, (Action) HackExecutor.StunAllEnemies},
            {Hack.TeleportAllItems, (Action) HackExecutor.TeleportAllItems},
            {Hack.ForceTentacleAttack, (Action) HackExecutor.ForceTentacleAttack},
            {Hack.ToggleAllLandmines, (Action) HackExecutor.ToggleAllLandmines},
            {Hack.ToggleAllTurrets, (Action) HackExecutor.ToggleAllTurrets},
            {Hack.KillPlayer, (Action<PlayerControllerB>) HackExecutor.KillPlayer},
            {Hack.HealPlayer, (Action<PlayerControllerB>) HackExecutor.HealPlayer},
            {Hack.LightningStrikePlayer, (Action<PlayerControllerB>) HackExecutor.LightningStrikePlayer},
            {Hack.UnlockDoorAction, (Action) HackExecutor.UnlockDoor},
            {Hack.ToggleAllESP, (Action) HackExecutor.ToggleAllESP},
            {Hack.DeathNotify, (Action<PlayerControllerB, CauseOfDeath>) HackExecutor.NotifyDeath},
            {Hack.SpectatePlayer, (Action<PlayerControllerB>) HackExecutor.SpectatePlayer},
            {Hack.MiniCam, (Action<PlayerControllerB>) HackExecutor.MiniCam},
            {Hack.UnlockUnlockable, (Action<Unlockable>) HackExecutor.UnlockUnlockable},
            {Hack.FlickerLights, (Action) HackExecutor.FlickerLights},
            {Hack.FixAllValves, (Action) HackExecutor.FixAllValves},
            {Hack.ModifyScrap, (Action<int, int>) HackExecutor.ModScrap},
            {Hack.TeleportEnemy, (Action<PlayerControllerB, EnemyAI[]>) HackExecutor.TeleportEnemy},
            {Hack.EnemyControl, (Action<EnemyAI>) HackExecutor.ControlEnemy},
            {Hack.SpawnMaskedEnemy, (Action) HackExecutor.SpawnMaskedEnemy},
            {Hack.SpiderWebPlayer, (Action<PlayerControllerB>) HackExecutor.SpiderWebPlayer},
            {Hack.BreakAllWebs, (Action) HackExecutor.BreakAllWebs},
            {Hack.LureAllEnemies, (Action<PlayerControllerB>) HackExecutor.LureAllEnemies},
            {Hack.SpawnMapObjects, (Action<MapObject>) HackExecutor.SpawnMapObjects},
            {Hack.SpawnLandmine, (Action) HackExecutor.SpawnLandmine},
            {Hack.SpawnTurret, (Action) HackExecutor.SpawnTurret},
            {Hack.ExplodeClosestMine, (Action<PlayerControllerB>) HackExecutor.ExplodeClosestMine},
            {Hack.SellEverything, (Action) HackExecutor.SellEverything},

        };

        public static readonly Dictionary<Hack, ButtonControl> KeyBinds = new Dictionary<Hack, ButtonControl>()
        {

            {Hack.OpenMenu, Keyboard.current.insertKey},
            {Hack.ToggleCursor, Keyboard.current.leftAltKey},
            {Hack.UnloadMenu, Keyboard.current.pauseKey},
            {Hack.UnlockDoorAction, Keyboard.current.f1Key}
        };

        public static void Execute(this Hack hack, params object[] param)
        {
            if(hack.CanBeToggled()) hack.Toggle();
            
            
            if(Executors.TryGetValue(hack, out var method))
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

        public static bool Button(this Hack hack, string text)
        {
            return GUILayout.Button(text);
        }

        public static bool Button(this Hack hack)
        {
            return GUILayout.Button(hack.ToBtnText());
        }

        public static bool Button(this Hack hack, bool b)
        {
            return GUILayout.Button(hack.ToBtnText(b));
        }

        public static string ToBtnText(this Hack hack)
        {
            return hack.CanBeToggled() ? ToggleFlags[hack] ? "Disable" : "Enable" : "Execute";
        }

        public static string ToBtnText(this Hack hack, bool b)
        {
            return b ? "Disable" : "Enable";
        }

        public static string ToStatus(this Hack hack)
        {
            return hack.CanBeToggled() ? ToggleFlags[hack] ? "Enabled" : "Disabled" : "No Status";
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
            if(b && !Waiting.Contains(hack)) Waiting.Add(hack);
            
            if(!b && Waiting.Contains(hack)) Waiting.Remove(hack);
        }

        public static bool KeyBindInUse(ButtonControl btn)
        {
            foreach(Hack hack in KeyBinds.Keys)
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
            Hack.DoorESP.Execute();
            Hack.LandmineESP.Execute();
            Hack.TurretESP.Execute();
            Hack.ShipESP.Execute();
            Hack.SteamHazardESP.Execute();
            Hack.BigDoorESP.Execute();
            Hack.DoorLockESP.Execute();
            Hack.BreakerESP.Execute();
        }
        public static void UnlockDoor()
        {
            if (!Hack.UnlockDoors.IsEnabled()) return;

            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
            if (player == null) return;

            RaycastHit hitInfo;
            if (Physics.Raycast(new Ray(CameraManager.ActiveCamera.transform.position, CameraManager.ActiveCamera.transform.forward), out hitInfo, 5f, LayerMask.GetMask("InteractableObject")))
            {
                DoorLock doorLock = hitInfo.transform.GetComponent<DoorLock>();
                if (doorLock != null && doorLock.isLocked && !doorLock.isPickingLock)
                {
                    doorLock.UnlockDoorSyncWithServer();
                    HUDManager.Instance.DisplayTip("Lethal Menu", "Door Unlocked");
                }
            } 
            else
            {
                LethalMenu.turrets.ForEach(turret => turret.gameObject.GetComponent<TerminalAccessibleObject>().CallFunctionFromTerminal());

                foreach (TerminalAccessibleObject obj in LethalMenu.allTerminalObjects)
                {
                    Vector3 directionToObject = obj.transform.position - player.transform.position;

                    float angle = Vector3.Angle(player.transform.forward, directionToObject);

                    if (angle < 60f && directionToObject.magnitude < 5f)
                    {
                        string type = "Terminal Object";

                        if (obj.isBigDoor) type = "Big Door";
                        else if (obj.name == "TurretScript") type = "Turret";
                        else if (obj.name == "Landmine") type = "Landmine";
                        else type = obj.name;

                        obj.CallFunctionFromTerminal();
                        HUDManager.Instance.DisplayTip("Lethal Menu", type + " ( " + obj.objectCode + " ) has been called from the terminal.");
                    }

                }


            }

        }
        public static void ModExperience(int amt, ActionType type)
        {
            int newAmt = amt;
            if(type == ActionType.Add) newAmt = HUDManager.Instance.localPlayerXP + amt;
            if(type == ActionType.Remove) newAmt = HUDManager.Instance.localPlayerXP - amt;

            HUDManager.Instance.localPlayerXP = newAmt;
        }
        public static void ToggleCursor()
        {
            if(Hack.ToggleCursor.IsEnabled() && (bool)StartOfRound.Instance)
            {
                GameNetworkManager.Instance.localPlayerController.playerActions.Disable();
                Cursor.visible = true;
                Settings.clm_lastCursorState = Cursor.lockState;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {

                GameNetworkManager.Instance.localPlayerController.playerActions.Enable();
                Cursor.visible = false;
                Cursor.lockState = Settings.clm_lastCursorState;
            }
        }

        public static void UnloadMenu() => LethalMenu.Instance.Unload();
        public static void NotifyDeath(PlayerControllerB died, CauseOfDeath cause) => HUDManager.Instance.DisplayTip("Lethal Menu", died.playerUsername + " has died from " + cause.ToString());

        public static void SpawnEnemy(EnemyType type, int num, bool outside) => RoundHandler.SpawnEnemy(type, num, outside);
        public static void SpawnMaskedEnemy() => RoundHandler.SpawnMimicFromMasks();
        public static void ToggleAllLandmines() => RoundHandler.ToggleAllLandmines();
        public static void ToggleAllTurrets() => RoundHandler.ToggleAllTurrets();
        public static void BlowUpAllLandmines() => RoundHandler.BlowUpAllLandmines();
        public static void FixAllValves() => RoundHandler.FixAllValves();
        public static void ToggleShipHorn() => RoundHandler.ToggleShipHorn();
        public static void ForceBridgeFall() => RoundHandler.ForceBridgeFall();
        public static void ForceTentacleAttack() => RoundHandler.ForceTentacleAttack();
        public static void ModScrap(int value, int type) => RoundHandler.ModScrap(value, type);
        public static void FlickerLights() => RoundHandler.FlickerLights();
        public static void SpawnMoreScrap() => RoundHandler.SpawnScrap();
        public static void UnlockUnlockable(Unlockable unlockable) => RoundHandler.BuyUnlockable(unlockable);
        public static void EndGame() => RoundHandler.EndGame();
        public static void StartGame() => RoundHandler.StartGame();
        public static void ModQuota(int amt) => RoundHandler.SetQuota(amt);
        public static void ModCredits(int amt, ActionType type) => RoundHandler.ModCredits(amt, type);
        public static void BreakAllWebs() => RoundHandler.BreakAllWebs();
        public static void SpawnLandmine() => RoundHandler.SpawnMapObject(MapObject.Landmine);
        public static void SpawnTurret() => RoundHandler.SpawnMapObject(MapObject.TurretContainer);
        public static void SpawnMapObjects(MapObject type) => RoundHandler.SpawnMapObjects(type);

        public static void SellEverything() => LethalMenu.localPlayer.Handle().PlaceEverythingOnDesk();
        public static void ExplodeClosestMine(PlayerControllerB player) => player.Handle().ExplodeClosestLandmine();
        public static void LureAllEnemies(PlayerControllerB player) => player.Handle().LureAllEnemies();
        public static void SpiderWebPlayer(PlayerControllerB player) => player.Handle().SpawnSpiderWebs(6);
        public static void TeleportAllItems() => LethalMenu.localPlayer.Handle().TeleportAllItems();
        public static void SpectatePlayer(PlayerControllerB player) => player.Handle().Spectate();
        public static void MiniCam(PlayerControllerB player) => player.Handle().MiniCam();
        public static void SaveTeleportPosition() => LethalMenu.localPlayer.Handle().SavePosition();
        public static void TeleportShip() => LethalMenu.localPlayer.Handle().TeleportShip();
        public static void TeleportSavedPosition() => LethalMenu.localPlayer.Handle().TeleportSaved();
        public static void Teleport(Vector3 pos, bool elevator = false, bool ship = false, bool factory = false) => LethalMenu.localPlayer.Handle().Teleport(pos, elevator, ship, factory);
        public static void KillPlayer(PlayerControllerB player) => player.Handle().Kill();
        public static void HealPlayer(PlayerControllerB player) => player.Handle().Heal();
        public static void LightningStrikePlayer(PlayerControllerB player) => player.Handle().Strike();
        public static void ControlEnemy(EnemyAI enemy) => enemy.Handle().Control();
        public static void TeleportEnemy(PlayerControllerB player, EnemyAI[] enemies) => enemies.ToList().FindAll(e => !e.isEnemyDead).ForEach(e => e.Handle().Teleport(player));

        public static void StunAllEnemies() => LethalMenu.enemies.ForEach(enemy => enemy.Handle().Stun());
        public static void KillAllEnemies() => LethalMenu.enemies.ForEach(enemy => enemy.Handle().Kill());
        public static void KillNearbyEnemies(int distance = -1) => LethalMenu.enemies.FindAll(e => GameUtil.GetDistanceToPlayer(e.transform.position) <= distance).ForEach(enemy => enemy.Handle().Kill());
        public static void ToggleShipLights() => RoundHandler.ToggleShipLights();
        public static void ToggleFactoryLights() => RoundHandler.ToggleFactoryLights();

    }
}
