namespace LethalMenu.Handler.EnemyControl;

internal enum BeesState
{
    IDLE,
    DEFENSIVE,
    ATTACK
}

internal class RedLocustBeesController : IEnemyController<RedLocustBees>
{
    public bool CanUseEntranceDoors(RedLocustBees _)
    {
        return true;
    }

    public float InteractRange(RedLocustBees _)
    {
        return 2.5f;
    }

    public void UsePrimarySkill(RedLocustBees enemy)
    {
        enemy.SetBehaviourState(BeesState.ATTACK);
        enemy.EnterAttackZapModeServerRpc(-1);
    }

    public void UseSecondarySkill(RedLocustBees enemy)
    {
        enemy.SetBehaviourState(BeesState.IDLE);
    }
}