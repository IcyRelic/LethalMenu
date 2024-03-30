using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl;

internal enum GiantState
{
    DEFAULT = 0,
    CHASE = 1
}

internal class ForestGiantController : IEnemyController<ForestGiantAI>
{
    private bool IsUsingSecondarySkill { get; set; }

    public void OnMovement(ForestGiantAI enemy, bool isMoving, bool isSprinting)
    {
        if (!IsUsingSecondarySkill) enemy.SetBehaviourState(GiantState.DEFAULT);
    }

    public void OnSecondarySkillHold(ForestGiantAI enemy)
    {
        IsUsingSecondarySkill = true;
        enemy.SetBehaviourState(GiantState.CHASE);
    }

    public void ReleaseSecondarySkill(ForestGiantAI enemy)
    {
        IsUsingSecondarySkill = false;
        enemy.SetBehaviourState(GiantState.DEFAULT);
    }

    public bool IsAbleToMove(ForestGiantAI enemy)
    {
        return !enemy.Reflect().GetValue<bool>("inEatingPlayerAnimation");
    }

    public string GetSecondarySkillName(ForestGiantAI _)
    {
        return "(HOLD) Chase";
    }

    public bool CanUseEntranceDoors(ForestGiantAI _)
    {
        return false;
    }

    public float InteractRange(ForestGiantAI _)
    {
        return 0.0f;
    }

    public bool SyncAnimationSpeedEnabled(ForestGiantAI _)
    {
        return false;
    }

    public void OnReleaseControl(ForestGiantAI enemy)
    {
        IsUsingSecondarySkill = false;
    }
}