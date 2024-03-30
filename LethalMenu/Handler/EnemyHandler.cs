using System;
using System.Collections;
using System.Collections.Generic;
using GameNetcodeStuff;
using LethalMenu.Util;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LethalMenu.Handler;

internal enum Behaviour
{
    Idle = 0,
    Chase = 1,
    Aggravated = 2,
    Unknown = 3
}

public class EnemyHandler
{
    private readonly EnemyAI _enemy;
    private PlayerControllerB _target;

    private EnemyHandler(EnemyAI enemy)
    {
        _enemy = enemy;
    }

    private void HandleLureCrawler()
    {
        var crawler = _enemy as CrawlerAI;

        crawler?.BeginChasingPlayerServerRpc((int)_target.playerClientId);
    }

    private void HandleLureMouthDog()
    {
        var dog = _enemy as MouthDogAI;

        dog?.ReactToOtherDogHowl(_target.transform.position);
    }

    private void HandleLureBaboonBird()
    {
        var baboon = _enemy as BaboonBirdAI;

        Threat threat = new()
        {
            threatScript = _target,
            lastSeenPosition = _target.transform.position,
            threatLevel = int.MaxValue,
            type = ThreatType.Player,
            focusLevel = int.MaxValue,
            timeLastSeen = Time.time,
            distanceToThreat = 0.0f,
            distanceMovedTowardsBaboon = float.MaxValue,
            interestLevel = int.MaxValue,
            hasAttacked = true
        };

        baboon?.SetAggressiveModeServerRpc(1);
        baboon.Reflect().Invoke("ReactToThreat", threat);
    }

    private void HandleLureForestGiant()
    {
        var giant = _enemy as ForestGiantAI;

        if (!giant) return;

        giant.SwitchToBehaviourServerRpc((int)Behaviour.Chase);
        giant.StopSearch(giant.roamPlanet, false);
        giant.chasingPlayer = _target;
        giant.investigating = true;

        giant.SetDestinationToPosition(_target.transform.position);
        giant.Reflect().SetValue("lostPlayerInChase", false);
    }

    private void HandleLureCentipede()
    {
        var centipede = _enemy as CentipedeAI;

        if (!centipede) return;

        centipede.SwitchToBehaviourServerRpc((int)Behaviour.Aggravated);

        var clingingToCeiling = centipede.Reflect().GetValue<bool>("clingingToCeiling");

        if (clingingToCeiling) centipede.TriggerCentipedeFallServerRpc(_target.playerClientId);
    }

    private void HandleLureFlowerman()
    {
        var enemy = _enemy as FlowermanAI;

        if (!enemy) return;

        enemy.SwitchToBehaviourServerRpc((int)Behaviour.Aggravated);
        enemy.EnterAngerModeServerRpc(20);
    }

    private void HandleLureSandSpider()
    {
        var spider = _enemy as SandSpiderAI;
        spider?.SwitchToBehaviourServerRpc((int)Behaviour.Chase);
        //spider.meshContainer.position = target.transform.position;
        //spider.SyncMeshContainerPositionToClients();

        var web = spider.SpawnWeb(_target.transform.position);

        spider?.webTraps.ForEach(webTrap => spider.PlayerTripWebServerRpc(webTrap.trapID, (int)_target.playerClientId));


        //spider.Reflect().SetValue("onWall", false).SetValue("watchFromDistance", false);
    }

    private void HandleLureRedLocustBees()
    {
        var bees = _enemy as RedLocustBees;
        bees?.SwitchToBehaviourServerRpc((int)Behaviour.Aggravated);
        if (bees) bees.hive.isHeld = true;
    }

    private void HandleLureHoarderBug()
    {
        var bug = _enemy as HoarderBugAI;

        if (!bug) return;

        bug.SwitchToBehaviourServerRpc((int)Behaviour.Aggravated);
        bug.angryAtPlayer = _target;
        bug.angryTimer = float.MaxValue;

        bug.Reflect().SetValue("lostPlayerInChase", false)
            ?.Invoke("SyncNestPositionServerRpc", _target.transform.position);
    }

    private void HandleLureNutcrackerEnemy()
    {
        var nutcracker = _enemy as NutcrackerEnemyAI;
        nutcracker?.SwitchToBehaviourServerRpc((int)Behaviour.Aggravated);

        nutcracker.Reflect().SetValue("lastSeenPlayerPos", _target.transform.position)
            ?.Invoke("timeSinceSeeingTarget", 0);
    }

    private void HandleLureMaskedPlayerEnemy()
    {
        var masked = _enemy as MaskedPlayerEnemy;
        masked?.SwitchToBehaviourServerRpc((int)Behaviour.Chase);
    }

    private void HandleLureSpringMan()
    {
        var spring = _enemy as SpringManAI;
        spring?.SwitchToBehaviourServerRpc((int)Behaviour.Chase);
    }

    private void HandleLurePuffer()
    {
        var puffer = _enemy as PufferAI;
        puffer?.SwitchToBehaviourServerRpc((int)Behaviour.Aggravated);
    }

    private void HandleLureJester()
    {
        var jester = _enemy as JesterAI;
        jester?.SwitchToBehaviourServerRpc((int)Behaviour.Aggravated);
    }

    private void HandleLureSandWorm()
    {
        var worm = _enemy as SandWormAI;
        worm?.SwitchToBehaviourServerRpc((int)Behaviour.Chase);
    }

    private void HandleLureEnemyByType()
    {
        switch (_enemy)
        {
            case CrawlerAI:
                HandleLureCrawler();
                break;
            case MouthDogAI:
                HandleLureMouthDog();
                break;
            case BaboonBirdAI:
                HandleLureBaboonBird();
                break;
            case ForestGiantAI:
                HandleLureForestGiant();
                break;
            case CentipedeAI:
                HandleLureCentipede();
                break;
            case FlowermanAI:
                HandleLureFlowerman();
                break;
            case SandSpiderAI:
                HandleLureSandSpider();
                break;
            case RedLocustBees:
                HandleLureRedLocustBees();
                break;
            case HoarderBugAI:
                HandleLureHoarderBug();
                break;
            case NutcrackerEnemyAI:
                HandleLureNutcrackerEnemy();
                break;
            case MaskedPlayerEnemy:
                HandleLureMaskedPlayerEnemy();
                break;
            case SpringManAI:
                HandleLureSpringMan();
                break;
            case PufferAI:
                HandleLurePuffer();
                break;
            case JesterAI:
                HandleLureJester();
                break;
            case SandWormAI:
                HandleLureSandWorm();
                break;
            default:
                _enemy.SwitchToBehaviourServerRpc((int)Behaviour.Chase);
                break;
        }
    }

    private void HandleSandWormKillPlayer()
    {
        var worm = _enemy as SandWormAI;
        Teleport(_target);
        worm?.StartEmergeAnimation();
    }

    private void HandleKillPlayerByType()
    {
        switch (_enemy)
        {
            case MouthDogAI dog:
                dog.KillPlayerServerRpc((int)_target.playerClientId);
                break;
            case ForestGiantAI giant:
                giant.GrabPlayerServerRpc((int)_target.playerClientId);
                break;
            case FlowermanAI flowerman:
                flowerman.KillPlayerAnimationServerRpc((int)_target.playerClientId);
                break;
            case RedLocustBees bees:
                bees.BeeKillPlayerServerRpc((int)_target.playerClientId);
                break;
            case NutcrackerEnemyAI nutcracker:
                nutcracker.LegKickPlayerServerRpc((int)_target.playerClientId);
                break;
            case MaskedPlayerEnemy masked:
                masked.KillPlayerAnimationServerRpc((int)_target.playerClientId);
                break;
            case JesterAI jester:
                jester.KillPlayerServerRpc((int)_target.playerClientId);
                break;
            case SandWormAI worm:
                HandleSandWormKillPlayer();
                break;
            case CentipedeAI centipede:
                centipede.SwitchToBehaviourServerRpc((int)Behaviour.Aggravated);
                centipede.ClingToPlayerServerRpc(_target.playerClientId);
                break;
            case BlobAI blob:
                blob.SlimeKillPlayerEffectServerRpc((int)_target.playerClientId);
                break;
        }
    }

    public bool HasInstaKill()
    {
        List<Type> types =
        [
            typeof(MouthDogAI),
            typeof(ForestGiantAI),
            typeof(FlowermanAI),
            typeof(RedLocustBees),
            typeof(NutcrackerEnemyAI),
            typeof(MaskedPlayerEnemy),
            typeof(JesterAI),
            typeof(SandWormAI),
            typeof(BlobAI),
            typeof(CentipedeAI)
        ];

        return types.Contains(_enemy.GetType());
    }

    public void Control()
    {
        if (_enemy.isEnemyDead) return;

        Cheats.EnemyControl.Control(_enemy);
    }

    public void Kill(bool despawn = false)
    {
        var forceDespawn = _enemy.GetType() == typeof(ForestGiantAI)
                           || _enemy.GetType() == typeof(SandWormAI)
                           || _enemy.GetType() == typeof(BlobAI)
                           || _enemy.GetType() == typeof(DressGirlAI)
                           || _enemy.GetType() == typeof(PufferAI)
                           || _enemy.GetType() == typeof(SpringManAI)
                           || _enemy.GetType() == typeof(DocileLocustBeesAI)
                           || _enemy.GetType() == typeof(DoublewingAI)
                           || _enemy.GetType() == typeof(RedLocustBees)
                           || _enemy.GetType() == typeof(LassoManAI)
                           || _enemy.GetType() == typeof(JesterAI);

        _enemy.KillEnemyServerRpc(forceDespawn || despawn);
    }

    public void Stun()
    {
        if (!_enemy.enemyType.canBeStunned) return;
        _enemy.SetEnemyStunned(true, 5);
    }

    public void Teleport(PlayerControllerB player)
    {
        if (_enemy.GetType() != typeof(MaskedPlayerEnemy) && ((_enemy.isOutside && player.isInsideFactory) ||
                                                              (!_enemy.isOutside && !player.isInsideFactory))) return;

        _enemy.ChangeEnemyOwnerServerRpc(LethalMenu.LocalPlayer.actualClientId);
        _enemy.transform.position = player.transform.position;
        _enemy.SyncPositionToClients();
    }


    public void TargetPlayer(PlayerControllerB player)
    {
        _target = player;
        _enemy.targetPlayer = player;
        _enemy.ChangeEnemyOwnerServerRpc(LethalMenu.LocalPlayer.actualClientId);
        _enemy.SetMovingTowardsTargetPlayer(player);
        HandleLureEnemyByType();
    }

    public void KillPlayer(PlayerControllerB player)
    {
        _target = player;
        _enemy.targetPlayer = player;
        _enemy.ChangeEnemyOwnerServerRpc(LethalMenu.LocalPlayer.actualClientId);
        HandleKillPlayerByType();
    }


    public static EnemyHandler GetHandler(EnemyAI enemy)
    {
        return new EnemyHandler(enemy);
    }
}

public static class EnemyAIExtensions
{
    public static EnemyHandler Handle(this EnemyAI enemy)
    {
        return EnemyHandler.GetHandler(enemy);
    }

    public static bool IsBehaviourState(this EnemyAI enemy, Enum state)
    {
        return enemy.currentBehaviourStateIndex == Convert.ToInt32(state);
    }

    public static void SetBehaviourState(this EnemyAI enemy, Enum state)
    {
        if (enemy.IsBehaviourState(state)) return;
        enemy.SwitchToBehaviourServerRpc(Convert.ToInt32(state));
    }

    public static GrabbableObject? FindNearbyItem(this EnemyAI enemy, float grabRange = 1.0f)
    {
        foreach (var collider in Physics.OverlapSphere(enemy.transform.position, grabRange))
        {
            if (!collider.TryGetComponent(out GrabbableObject item)) continue;
            if (!item.TryGetComponent(out NetworkObject _)) continue;

            return item;
        }

        return null;
    }
}

public static class HoarderBugAIExtensions
{
    public static void StealAllItems(this HoarderBugAI bug)
    {
        bug.ChangeEnemyOwnerServerRpc(LethalMenu.LocalPlayer.actualClientId);

        LethalMenu.Instance.StartCoroutine(StealItems(bug));
    }

    private static IEnumerator StealItems(HoarderBugAI bug)
    {
        var items = LethalMenu.Items.FindAll(i => !i.isHeld && !i.isPocketed && !i.isInShipRoom && i.isInFactory)
            .ConvertAll(i => i.NetworkObject);

        foreach (var obj in items)
        {
            yield return new WaitForSeconds(0.2f);
            bug.GrabItemServerRpc(obj);
            bug.DropItemServerRpc(obj, bug.nestPosition, true);
        }
    }
}

public static class SandSpiderAIExtensions
{
    public static int SpawnWeb(this SandSpiderAI spider, Vector3 position)
    {
        spider.ChangeEnemyOwnerServerRpc(LethalMenu.LocalPlayer.actualClientId);

        var ray = new Ray(position, Vector3.Scale(Random.onUnitSphere, new Vector3(1f, Random.Range(0.6f, 1f), 1f)));

        if (!Physics.Raycast(ray, out var rayHit, 7f, StartOfRound.Instance.collidersAndRoomMask) ||
            !(rayHit.distance >= 1.5)) return -1;

        var point = rayHit.point;

        if (!Physics.Raycast(position, Vector3.down, out rayHit, 10f,
                StartOfRound.Instance.collidersAndRoomMask)) return -1;

        spider.SpawnWebTrapServerRpc(rayHit.point, point);

        return spider.webTraps.Count - 1;
    }

    public static void BreakAllWebs(this SandSpiderAI spider)
    {
        spider.webTraps.ForEach(web => spider.BreakWebServerRpc(web.trapID, -1));
    }
}