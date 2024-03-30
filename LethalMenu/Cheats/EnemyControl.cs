using System;
using System.Collections.Generic;
using LethalMenu.Components;
using LethalMenu.Handler;
using LethalMenu.Handler.EnemyControl;
using LethalMenu.Util;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalMenu.Cheats;

internal class EnemyControl : Cheat
{
    private const float TeleportDoorCooldown = 2.5f;
    private const float DoorInteractionCooldown = 0.7f;
    private static EnemyAI enemy;
    private static GameObject ControllerInstance;
    private static MouseInput mouse;
    private static AIMovement movement;
    private static bool IsAIControlled;
    private static bool NoClipEnabled;
    private float DoorCooldownRemaining;
    private float TeleportCooldownRemaining;

    private static Dictionary<Type, IController> EnemyControllers { get; } = new()
    {
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
        { typeof(RedLocustBees), new RedLocustBeesController() }
    };

    public override void Update()
    {
        StopControl();
        if (!Hack.EnemyControl.IsEnabled()) return;
        if (!enemy) return;
        if (!Hack.FreeCam.IsEnabled()) Hack.FreeCam.Execute();
        if (!Freecam.Camera) return;


        if (!EnemyControllers.TryGetValue(enemy.GetType(), out var controller))
        {
            if (IsAIControlled) return;

            UpdateEnemyPosition();
            UpdateEnemyRotation();

            return;
        }


        if (!enemy.agent) return;

        UpdateCooldowns();

        enemy.ChangeEnemyOwnerServerRpc(LethalMenu.LocalPlayer.actualClientId);
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
        LethalMenu.LocalPlayer.cursorTip.text = controller.GetPrimarySkillName(enemy);

        HandleInput();

        if (IsAIControlled) return;
        if (!controller.IsAbleToMove(enemy)) return;

        if (controller.SyncAnimationSpeedEnabled(enemy)) movement.CharacterSpeed = enemy.agent.speed;

        if (controller.IsAbleToRotate(enemy)) UpdateEnemyRotation();

        UpdateEnemyPosition();
        controller.OnMovement(enemy, movement.IsMoving, movement.IsSprinting);
    }


    public static void Control(EnemyAI enemy)
    {
        if (enemy.isEnemyDead) return;
        EnemyControl.enemy = enemy;
        enemy.ChangeEnemyOwnerServerRpc(LethalMenu.LocalPlayer.actualClientId);
        ControllerInstance = new GameObject("EnemyController")
        {
            transform =
            {
                position = enemy.transform.position,
                rotation = enemy.transform.rotation
            }
        };

        mouse = ControllerInstance.AddComponent<MouseInput>();
        movement = ControllerInstance.AddComponent<AIMovement>();

        movement.Init();
        movement.CalibrateCollision(enemy);
        movement.CharacterSprintSpeed = 5.0f;
        movement.SetNoClipMode(false);
        movement.SetPosition(enemy.transform.position);
        movement.CharacterSprintSpeed = SprintMultiplier();
        SetAIControl(false);
    }

    public static void StopControl()
    {
        if (Hack.EnemyControl.IsEnabled() || enemy is null) return;
        Hack.FreeCam.SetToggle(false);
        if (enemy?.agent is not null)
        {
            enemy.agent.updatePosition = true;
            enemy.agent.updateRotation = true;
            enemy.agent.isStopped = false;
            UpdateEnemyPosition();
            enemy.agent.Warp(enemy.transform.position);
        }

        if (enemy && EnemyControllers.TryGetValue(enemy.GetType(), out var controller))
            controller.OnReleaseControl(enemy);

        IsAIControlled = false;
        Destroy(ControllerInstance);
        enemy = null;
        ControllerInstance = null;
        mouse = null;
        movement = null;
    }

    private void UpdateCooldowns()
    {
        DoorCooldownRemaining = Mathf.Clamp(
            DoorCooldownRemaining - Time.deltaTime,
            0.0f,
            DoorInteractionCooldown
        );

        TeleportCooldownRemaining = Mathf.Clamp(
            TeleportCooldownRemaining - Time.deltaTime,
            0.0f,
            TeleportDoorCooldown
        );
    }

    private void MoveCamera()
    {
        Freecam.Camera.transform.SetPositionAndRotation(
            enemy.transform.position + 3.0f * (Vector3.up - enemy.transform.forward),
            Quaternion.LookRotation(enemy.transform.forward)
        );
    }

    private static void UpdateEnemyPosition()
    {
        var euler = enemy.transform.eulerAngles;
        euler.y = mouse.transform.eulerAngles.y;

        enemy.transform.eulerAngles = euler;
        enemy.transform.position = movement.transform.position;
    }

    private static void UpdateEnemyRotation()
    {
        if (!movement) return;
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

        if (!Keyboard.current.f12Key.wasPressedThisFrame) return;

        Hack.EnemyControl.SetToggle(false);
        enemy.Handle().Kill();
    }

    private static float InteractRange()
    {
        return EnemyControllers.TryGetValue(enemy.GetType(), out var value)
            ? value.InteractRange(enemy)
            : IController.DefaultInteractRange;
    }

    private static float SprintMultiplier()
    {
        return EnemyControllers.TryGetValue(enemy.GetType(), out var value)
            ? value.SprintMultiplier(enemy)
            : IController.DefaultSprintMultiplier;
    }

    private static void ToggleAIControl()
    {
        if (enemy?.agent is null || movement is null || mouse is null) return;

        IsAIControlled = !IsAIControlled;
        SetAIControl(IsAIControlled);
        //this.SendPossessionNotifcation($"AI Control: {(this.IsAIControlled ? "Enabled" : "Disabled")}");
    }

    private static void SetAIControl(bool enableAI)
    {
        if (movement is null || enemy?.agent is null) return;

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

    private void HandleEntranceDoors(EnemyAI enemy, RaycastHit hit)
    {
        if (TeleportCooldownRemaining > 0.0f) return;
        if (!hit.collider.gameObject.TryGetComponent(out EntranceTeleport entrance)) return;

        InteractWithTeleport(enemy, entrance);
        TeleportCooldownRemaining = TeleportDoorCooldown;
    }

    private void InteractWithAmbient(EnemyAI enemy, IController controller)
    {
        if (!Physics.Raycast(enemy.transform.position, enemy.transform.forward, out var hit, InteractRange())) return;
        if (hit.collider.gameObject.TryGetComponent(out DoorLock doorLock) && DoorCooldownRemaining <= 0.0f)
        {
            OpenDoorAsEnemy(doorLock);
            DoorCooldownRemaining = DoorInteractionCooldown;
            return;
        }

        if (controller.CanUseEntranceDoors(enemy)) HandleEntranceDoors(enemy, hit);
    }

    private void OpenDoorAsEnemy(DoorLock door)
    {
        if (door.Reflect().GetValue<bool>("isDoorOpened")) return;
        if (door.gameObject.TryGetComponent(out AnimatedObjectTrigger trigger))
            trigger.TriggerAnimationNonPlayer(false, true);

        door.OpenDoorAsEnemyServerRpc();
    }

    private Transform? GetExitPointFromDoor(EntranceTeleport entrance)
    {
        return LethalMenu.Doors.Find(teleport =>
            teleport.entranceId == entrance.entranceId && teleport.isEntranceToBuilding != entrance.isEntranceToBuilding
        )?.entrancePoint;
    }

    private void InteractWithTeleport(EnemyAI enemy, EntranceTeleport teleport)
    {
        if (movement is not { } aiMovement) return;
        if (GetExitPointFromDoor(teleport) is not { } exitPoint) return;

        aiMovement.SetPosition(exitPoint.position);
        enemy.EnableEnemyMesh(true);
    }


    private void ToggleNoClip()
    {
        NoClipEnabled = !NoClipEnabled;
        movement.SetNoClipMode(NoClipEnabled);
        //this.SendPossessionNotifcation($"NoClip: {(NoClipEnabled ? "Enabled" : "Disabled")}");
    }

    private void UsePrimarySkill()
    {
        if (!EnemyControllers.TryGetValue(enemy.GetType(), out var controller)) return;

        controller.UsePrimarySkill(enemy);
    }

    private void UseSecondarySkill()
    {
        if (!EnemyControllers.TryGetValue(enemy.GetType(), out var controller)) return;

        controller.UseSecondarySkill(enemy);
    }

    private void OnSecondarySkillHold()
    {
        if (!EnemyControllers.TryGetValue(enemy.GetType(), out var controller)) return;

        controller.OnSecondarySkillHold(enemy);
    }

    private void ReleaseSecondarySkill()
    {
        if (!EnemyControllers.TryGetValue(enemy.GetType(), out var controller)) return;

        controller.ReleaseSecondarySkill(enemy);
    }
}