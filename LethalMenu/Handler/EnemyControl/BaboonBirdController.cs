using LethalMenu.Util;
using Unity.Netcode;
using UnityEngine;

namespace LethalMenu.Handler.EnemyControl;

internal class BaboonBirdController : IEnemyController<BaboonBirdAI>
{
    private Vector3 CustomCamp { get; } = new(1000.0f, 0.0f, 0.0f);
    private Vector3 OriginalCamp { get; set; } = Vector3.zero;

    public void OnDeath(BaboonBirdAI enemy)
    {
        if (enemy.heldScrap is not null) enemy.Reflect().Invoke("DropHeldItemAndSync");
    }

    public void OnTakeControl(BaboonBirdAI _)
    {
        if (BaboonBirdAI.baboonCampPosition != CustomCamp) return;

        OriginalCamp = BaboonBirdAI.baboonCampPosition;
        BaboonBirdAI.baboonCampPosition = CustomCamp;
    }

    public void OnReleaseControl(BaboonBirdAI _)
    {
        if (BaboonBirdAI.baboonCampPosition == OriginalCamp) return;
        BaboonBirdAI.baboonCampPosition = OriginalCamp;
    }

    public void UsePrimarySkill(BaboonBirdAI enemy)
    {
        if (enemy.heldScrap is null && enemy.FindNearbyItem() is { } grabbable)
        {
            GrabItemAndSync(enemy, grabbable);
            return;
        }

        if (enemy.heldScrap is ShotgunItem shotgun) shotgun.ShootGunAsEnemy(enemy);
    }

    public void UseSecondarySkill(BaboonBirdAI enemy)
    {
        if (enemy.heldScrap is null) return;
        enemy.Reflect().Invoke("DropHeldItemAndSync");
    }

    public string GetPrimarySkillName(BaboonBirdAI enemy)
    {
        return enemy.heldScrap is not null ? "" : "Grab Item";
    }

    public string GetSecondarySkillName(BaboonBirdAI enemy)
    {
        return enemy.heldScrap is null ? "" : "Drop item";
    }

    public float InteractRange(BaboonBirdAI _)
    {
        return 1.5f;
    }

    private static void GrabItemAndSync(BaboonBirdAI enemy, Component item)
    {
        if (!item.TryGetComponent(out NetworkObject netItem)) return;
        enemy.Reflect().Invoke("GrabItemAndSync", netItem);
    }
}