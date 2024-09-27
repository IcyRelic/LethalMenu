using HarmonyLib;
using LethalMenu.Components;
using LethalMenu.Handler;
using LethalMenu.Handler.EnemyControl;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalMenu.Cheats
{
    internal class EnemyControl : Cheat
    {
        private static EnemyAI enemy = null;
        private static GameObject ControllerInstance = null;
        private static MouseInput mouse = null;
        private static AIMovement movement = null;
        private static AudioListener audioListener = null;
        public static bool IsAIControlled = false;
        private static bool NoClipEnabled = false;
        private const float TeleportDoorCooldown = 2.5f;
        private const float DoorInteractionCooldown = 0.7f;
        private float DoorCooldownRemaining = 0.0f;
        private float TeleportCooldownRemaining = 0.0f;

        private static Dictionary<Type, IController> EnemyControllers { get; } = new() {
            { typeof(CentipedeAI), new CentipedeController() },
            { typeof(FlowermanAI), new FlowermanController() },
            { typeof(ForestGiantAI), new ForestGiantController() },
            { typeof(HoarderBugAI), new HoarderBugController() },
            { typeof(JesterAI), new JesterController() },
            { typeof(NutcrackerEnemyAI), new NutcrackerController() },
            { typeof(PufferAI), new PufferController() },
            { typeof(BaboonBirdAI), new BaboonBirdController() },
            { typeof(SandWormAI), new SandWormController() },
            { typeof(MouthDogAI), new MouthDogController() },
            { typeof(MaskedPlayerEnemy), new MaskedPlayerController() },
            { typeof(SpringManAI), new SpringManController() },
            { typeof(BlobAI), new BlobController() },
            { typeof(TestEnemy), new TestEnemyController() },
            { typeof(LassoManAI), new LassoManController() },
            { typeof(CrawlerAI), new CrawlerController() },
            { typeof(SandSpiderAI), new SandSpiderController() },
            { typeof(RedLocustBees), new RedLocustBeesController() },
            //{ typeof(RadMechAI), new RadMechController() },
            { typeof(ButlerEnemyAI), new ButlerController() },
            { typeof(ButlerBeesEnemyAI), new ButlerBeesController() },
            { typeof(FlowerSnakeEnemy), new FlowerSnakeController() },
            { typeof(DoublewingAI), new DoublewingController() },
            { typeof(DressGirlAI), new DressGirlController() },
            { typeof(ClaySurgeonAI), new ClaySurgeonController() },
            { typeof(CaveDwellerAI), new CaveDwellerController() },
            { typeof(DocileLocustBeesAI), new DocileLocustBeesController() },
        };

        public static void Control(EnemyAI enemy)
        {
            if (enemy.isEnemyDead || enemy == null) return;
            EnemyControl.enemy = enemy;

            if (!EnemyControllers.TryGetValue(enemy.GetType(), out IController controller) || controller == null)
            {
                Hack.EnemyControl.SetToggle(false);
                StopControl();
                HUDManager.Instance.DisplayTip("Lethal Menu", $"No Controller for enemy :C");
                return;
            }

            controller.OnTakeControl(enemy);

            LethalMenu.localPlayer.activeAudioListener.enabled = false;

            if (enemy.IsSpawned) enemy.ChangeEnemyOwnerServerRpc(LethalMenu.localPlayer.actualClientId);

            ControllerInstance = new GameObject("EnemyController");
            ControllerInstance.transform.position = enemy.transform.position;
            ControllerInstance.transform.rotation = enemy.transform.rotation;

            mouse = ControllerInstance.AddComponent<MouseInput>();
            movement = ControllerInstance.AddComponent<AIMovement>();
            audioListener = ControllerInstance.AddComponent<AudioListener>();

            movement.Init();
            movement.CalibrateCollision(enemy);
            movement.CharacterSprintSpeed = 5.0f;
            movement.SetNoClipMode(false);
            movement.SetPosition(enemy.transform.position);
            movement.CharacterSprintSpeed = SprintMultiplier();   
            SetAIControl(false);
            if (audioListener == null) return;
            ChangeAudioListener(audioListener);
        }

        public static void StopControl()
        {
            if (Hack.EnemyControl.IsEnabled() || enemy == null) return;
            Hack.FreeCam.SetToggle(false);

            if (enemy?.agent != null && enemy.agent.isOnNavMesh)
            {
                enemy.agent.updatePosition = true;
                enemy.agent.updateRotation = true;
                enemy.agent.isStopped = false;
                UpdateEnemyPosition();
                enemy.agent.Warp(enemy.transform.position);
            }

            if (!EnemyControllers.TryGetValue(enemy.GetType(), out IController controller) || controller == null) return;

            controller.OnReleaseControl(enemy);

            LethalMenu.localPlayer.activeAudioListener.enabled = true;

            ChangeAudioListener(LethalMenu.localPlayer.activeAudioListener);

            audioListener.enabled = false;

            if (LethalMenu.localPlayer.isPlayerDead) HUDManager.Instance.holdButtonToEndGameEarlyMeter.gameObject.SetActive(true);
            LethalMenu.localPlayer.cursorTip.text = "";
            IsAIControlled = false;
            Destroy(ControllerInstance);
            enemy = null;
            ControllerInstance = null;
            mouse = null;
            movement = null;
            audioListener = null;
        }

        public override void Update()
        {
            StopControl();
            if (Hack.EnemyControl.IsEnabled() && enemy.isEnemyDead || enemy == null)
            {
                Hack.EnemyControl.SetToggle(false);
                Hack.FreeCam.SetToggle(false);              
                return;
            }
            if (!Hack.EnemyControl.IsEnabled()) return;
            if (enemy == null) return;
            if (!Hack.FreeCam.IsEnabled()) Hack.FreeCam.Execute();
            if (Freecam.camera == null) return;
            if (HUDManager.Instance == null) return;
            if (!EnemyControllers.TryGetValue(enemy.GetType(), out IController controller)) return;

            if (controller == null)
            {
                if (!IsAIControlled)
                {
                    UpdateEnemyPosition();
                    UpdateEnemyRotation();
                }
                return;
            }

            if (LethalMenu.localPlayer.isPlayerDead) HUDManager.Instance.Reflect().SetValue("holdButtonToEndGameEarlyHoldTime", 0f);
            if (LethalMenu.localPlayer.isPlayerDead) HUDManager.Instance.holdButtonToEndGameEarlyMeter.gameObject.SetActive(false);

            if (!(bool)enemy.agent) return;

            UpdateCooldowns();

            if (enemy.IsSpawned) enemy.ChangeEnemyOwnerServerRpc(LethalMenu.localPlayer.actualClientId);
            MoveCamera();

            if (enemy.isEnemyDead)
            {
                controller.OnDeath(enemy);
                Hack.EnemyControl.SetToggle(false);
                StopControl();
                return;
            }

            controller.Update(enemy, false);
            InteractWithAmbient(enemy, EnemyControllers[enemy.GetType()]);
            LethalMenu.localPlayer.cursorTip.text = controller.GetPrimarySkillName(enemy);

            HandleInput();

            if (IsAIControlled) return;
            if (!controller.IsAbleToMove(enemy)) return;

            if (controller.SyncAnimationSpeedEnabled(enemy)) movement.CharacterSpeed = enemy.agent.speed;

            if (controller.IsAbleToRotate(enemy)) UpdateEnemyRotation();

            UpdateEnemyPosition();
            controller.OnMovement(enemy, movement.IsMoving, movement.IsSprinting);
        }

        private static void ChangeAudioListener(AudioListener audioListener)
        {
            StartOfRound.Instance.audioListener = audioListener;
        }

        private static IController GetControllerForEnemy(EnemyAI enemy) => enemy != null && EnemyControllers.TryGetValue(enemy.GetType(), out var controller) ? controller : null;

        private void UpdateCooldowns()
        {
            DoorCooldownRemaining = Mathf.Clamp(
                this.DoorCooldownRemaining - Time.deltaTime,
                0.0f,
                DoorInteractionCooldown
            );

            TeleportCooldownRemaining = Mathf.Clamp(
                this.TeleportCooldownRemaining - Time.deltaTime,
                0.0f,
                TeleportDoorCooldown
            );
        }

        private void MoveCamera()
        {
            Freecam.camera.transform.SetPositionAndRotation(
                enemy.transform.position + (3.0f * (Vector3.up - enemy.transform.forward)),
                Quaternion.LookRotation(enemy.transform.forward)
            );
        }

        private static void UpdateEnemyPosition()
        {
            if (enemy == null || movement == null || mouse == null) return;
            Vector3 euler = enemy.transform.eulerAngles;
            euler.y = mouse.transform.eulerAngles.y;
            enemy.transform.eulerAngles = euler;
            enemy.transform.position = movement.transform.position;
        }

        private static void UpdateEnemyRotation()
        {
            if (movement == null) return;
            movement.transform.rotation = mouse.transform.rotation;
        }

        private void HandleInput()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame) UsePrimarySkill();
            if (Mouse.current.rightButton.wasPressedThisFrame) UseSecondarySkill();
            if (Mouse.current.rightButton.isPressed) OnSecondarySkillHold();
            if (Mouse.current.rightButton.wasReleasedThisFrame) ReleaseSecondarySkill();

            if (Keyboard.current.f9Key.wasPressedThisFrame) ToggleAIControl();
            if (Keyboard.current.f10Key.wasPressedThisFrame) ToggleNoClip();
            if (Keyboard.current.f11Key.wasPressedThisFrame) Hack.EnemyControl.SetToggle(false);
            if (Keyboard.current.f12Key.wasPressedThisFrame)
            {
                Hack.EnemyControl.SetToggle(false);
                enemy.Handle().Kill();
            }
        }

        public static float InteractRange() => GetControllerForEnemy(enemy).InteractRange(enemy);

        public static float SprintMultiplier() => GetControllerForEnemy(enemy).SprintMultiplier(enemy);

        public void ToggleAIControl()
        {
            if (enemy?.agent is null || movement is null || mouse is null) return;

            IsAIControlled = !IsAIControlled;
            SetAIControl(IsAIControlled);
            //this.SendPossessionNotifcation($"AI Control: {(this.IsAIControlled ? "Enabled" : "Disabled")}");
        }

        private static void SetAIControl(bool enableAI)
        {
            if (movement is null || enemy is null || enemy.agent is null) return;

            if (enableAI)
            {
                _ = enemy.agent.Warp(enemy.transform.position);
                enemy.SyncPositionToClients();
            }

            if (NoClipEnabled)
            {
                NoClipEnabled = false;
                movement.SetNoClipMode(false);
            }

            enemy.agent.updatePosition = enableAI;
            enemy.agent.updateRotation = enableAI;
            enemy.agent.isStopped = !enableAI;
            movement.SetPosition(enemy.transform.position);
            movement.enabled = !enableAI;
        }

        public void HandleEntranceDoors(EnemyAI enemy, RaycastHit hit)
        {
            if (this.TeleportCooldownRemaining > 0.0f) return;
            if (!hit.collider.gameObject.TryGetComponent(out EntranceTeleport entrance)) return;

            this.InteractWithTeleport(enemy, entrance);
            this.TeleportCooldownRemaining = EnemyControl.TeleportDoorCooldown;
        }

        public void InteractWithAmbient(EnemyAI enemy, IController controller)
        {
            if (!Physics.Raycast(enemy.transform.position, enemy.transform.forward, out RaycastHit hit, InteractRange())) return;
            if (hit.collider.gameObject.TryGetComponent(out DoorLock doorLock) && this.DoorCooldownRemaining <= 0.0f)
            {
                this.OpenDoorAsEnemy(doorLock);
                this.DoorCooldownRemaining = EnemyControl.DoorInteractionCooldown;
                return;
            }

            if (controller.CanUseEntranceDoors(enemy))
            {
                this.HandleEntranceDoors(enemy, hit);
                return;
            }
        }

        public void OpenDoorAsEnemy(DoorLock door)
        {
            if (door.Reflect().GetValue<bool>("isDoorOpened")) return;
            if (door.gameObject.TryGetComponent(out AnimatedObjectTrigger trigger))
            {
                trigger.TriggerAnimationNonPlayer(false, true, false);
            }
            door.OpenDoorAsEnemyServerRpc();
        }

        Transform? GetExitPointFromDoor(EntranceTeleport entrance) =>
            LethalMenu.doors.Find(teleport =>
                teleport.entranceId == entrance.entranceId && teleport.isEntranceToBuilding != entrance.isEntranceToBuilding
            )?.entrancePoint;

        public void InteractWithTeleport(EnemyAI enemy, EntranceTeleport teleport)
        {
            if (movement is not AIMovement aiMovement) return;
            if (this.GetExitPointFromDoor(teleport) is not Transform exitPoint) return;

            aiMovement.SetPosition(exitPoint.position);
            enemy.EnableEnemyMesh(true, false);
        }

        public void ToggleNoClip()
        {
            NoClipEnabled = !NoClipEnabled;
            movement.SetNoClipMode(NoClipEnabled);
        }

        public void UsePrimarySkill()
        {
            if (enemy == null) return;
            if (IsAIControlled) return;

            GetControllerForEnemy(enemy).UsePrimarySkill(enemy);
        }

        public void UseSecondarySkill()
        {
            if (enemy == null) return;
            if (IsAIControlled) return;

            GetControllerForEnemy(enemy).UseSecondarySkill(enemy);
        }

        public void OnSecondarySkillHold()
        {
            if (enemy == null) return;
            if (IsAIControlled) return;

            GetControllerForEnemy(enemy).OnSecondarySkillHold(enemy);
        }

        public void ReleaseSecondarySkill()
        {
            if (enemy == null) return;
            if (IsAIControlled) return;

            GetControllerForEnemy(enemy).ReleaseSecondarySkill(enemy);
        }
    }

    [HarmonyPatch]
    internal class EnemyControlPatches
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(HUDManager), "CanPlayerScan")]
        public static void CanPlayerScan(ref bool __result)
        {
            if (Hack.EnemyControl.IsEnabled()) __result = false;
        }
    }
}
