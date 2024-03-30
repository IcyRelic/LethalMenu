namespace LethalMenu.Handler.EnemyControl;

internal enum FlowermanState
{
    SCOUTING,
    STAND,
    ANGER
}

internal class FlowermanController : IEnemyController<FlowermanAI>
{
    public void UsePrimarySkill(FlowermanAI enemy)
    {
        if (!enemy.carryingPlayerBody) enemy.SetBehaviourState(FlowermanState.ANGER);

        enemy.DropPlayerBodyServerRpc();
    }

    public void UseSecondarySkill(FlowermanAI enemy)
    {
        enemy.SetBehaviourState(FlowermanState.STAND);
    }

    public void ReleaseSecondarySkill(FlowermanAI enemy)
    {
        enemy.SetBehaviourState(FlowermanState.SCOUTING);
    }

    public bool IsAbleToMove(FlowermanAI enemy)
    {
        return !enemy.inSpecialAnimation;
    }

    public string GetPrimarySkillName(FlowermanAI enemy)
    {
        return enemy.carryingPlayerBody ? "Drop body" : "";
    }

    public string GetSecondarySkillName(FlowermanAI _)
    {
        return "Stand";
    }

    public float InteractRange(FlowermanAI _)
    {
        return 1.5f;
    }

    public bool SyncAnimationSpeedEnabled(FlowermanAI _)
    {
        return false;
    }
}