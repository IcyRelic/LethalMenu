using LethalMenu.Util;
using Unity.Netcode;

namespace LethalMenu.Handler.EnemyControl;

internal enum BugState
{
    IDLE,
    SEARCHING_FOR_ITEMS,
    RETURNING_TO_NEST,
    CHASING_PLAYER,
    WATCHING_PLAYER,
    AT_NEST
}

internal class HoarderBugController : IEnemyController<HoarderBugAI>
{
    public void Update(HoarderBugAI enemy, bool isAIControlled)
    {
        if (isAIControlled) return;
        if (enemy.heldItem?.itemGrabbableObject is null) return;

        enemy.angryTimer = 0.0f;
        enemy.SetBehaviourState(BugState.IDLE);
    }


    public void OnDeath(HoarderBugAI enemy)
    {
        if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject)) return;

        _ = enemy.Reflect().Invoke(
            "DropItemAndCallDropRPC",
            networkObject,
            false
        );
    }

    public void UsePrimarySkill(HoarderBugAI enemy)
    {
        if (enemy == null) return;

        if (enemy.angryTimer > 0.0f)
        {
            enemy.angryTimer = 0.0f;
            enemy.angryAtPlayer = null;
            enemy.SetBehaviourState(BugState.IDLE);
        }

        var nearbyItem = enemy.FindNearbyItem();
        if (nearbyItem != null)
            GrabItem(enemy, nearbyItem);
        else
            UseHeldItem(enemy);
    }

    public void UseSecondarySkill(HoarderBugAI enemy)
    {
        if (enemy.heldItem?.itemGrabbableObject is null)
        {
            var hostPlayer = StartOfRound.Instance.allPlayerScripts[0];
            enemy.watchingPlayer = hostPlayer;
            enemy.angryAtPlayer = hostPlayer;
            enemy.angryTimer = 15.0f;
            enemy.SetBehaviourState(BugState.CHASING_PLAYER);
            return;
        }

        if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject))
            enemy.Reflect().Invoke("DropItemAndCallDropRPC", [networkObject, false]);
    }

    public string GetPrimarySkillName(HoarderBugAI enemy)
    {
        return enemy.heldItem is not null ? "Use item" : "Grab Item";
    }

    public string GetSecondarySkillName(HoarderBugAI enemy)
    {
        return enemy.heldItem is null ? "" : "Drop item";
    }

    public float InteractRange(HoarderBugAI _)
    {
        return 1.5f;
    }

    private void UseHeldItem(HoarderBugAI enemy)
    {
        if (enemy.heldItem == null || enemy.heldItem.itemGrabbableObject == null) return;

        if (enemy.heldItem.itemGrabbableObject is ShotgunItem gun) gun.ShootGunAsEnemy(enemy);
    }

    private void GrabItem(HoarderBugAI enemy, GrabbableObject item)
    {
        var reflect = enemy.Reflect();
        var netItem = item.GetComponent<NetworkObject>();
        if (enemy == null || item == null || netItem == null || reflect is null) return;

        enemy.SwitchToBehaviourServerRpc(1);

        enemy.GrabItemServerRpc(netItem);
    }
}