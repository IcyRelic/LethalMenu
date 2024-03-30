using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl;

internal enum CentipedeState
{
    SEARCHING,
    HIDING,
    CHASING,
    CLINGING
}

internal class CentipedeController : IEnemyController<CentipedeAI>
{
    public void UsePrimarySkill(CentipedeAI enemy)
    {
        if (!enemy.IsBehaviourState(CentipedeState.HIDING)) return;
        enemy.SetBehaviourState(CentipedeState.CHASING);
    }

    public void UseSecondarySkill(CentipedeAI enemy)
    {
        if (IsClingingToSomething(enemy)) return;
        _ = enemy.Reflect().Invoke("RaycastToCeiling");
        enemy.SetBehaviourState(CentipedeState.HIDING);
    }

    public bool IsAbleToMove(CentipedeAI enemy)
    {
        return !IsClingingToSomething(enemy);
    }

    public bool IsAbleToRotate(CentipedeAI enemy)
    {
        return !IsClingingToSomething(enemy);
    }

    public string GetPrimarySkillName(CentipedeAI _)
    {
        return "Drop";
    }

    public string GetSecondarySkillName(CentipedeAI _)
    {
        return "Attach to ceiling";
    }

    public float InteractRange(CentipedeAI _)
    {
        return 1.5f;
    }

    public bool CanUseEntranceDoors(CentipedeAI _)
    {
        return false;
    }

    public bool SyncAnimationSpeedEnabled(CentipedeAI _)
    {
        return false;
    }

    private static bool IsClingingToSomething(CentipedeAI enemy)
    {
        var centipedeReflector = enemy.Reflect();

        return enemy.clingingToPlayer is not null || enemy.inSpecialAnimation ||
               centipedeReflector.GetValue<bool>("clingingToDeadBody") ||
               centipedeReflector.GetValue<bool>("clingingToCeiling") ||
               centipedeReflector.GetValue<bool>("startedCeilingAnimationCoroutine") ||
               centipedeReflector.GetValue<bool>("inDroppingOffPlayerAnim");
    }
}