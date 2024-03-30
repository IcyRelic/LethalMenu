namespace LethalMenu.Handler.EnemyControl;

internal class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy>
{
    public void UsePrimarySkill(MaskedPlayerEnemy enemy)
    {
        enemy.SetHandsOutServerRpc(!enemy.creatureAnimator.GetBool("HandsOut"));
    }

    public void UseSecondarySkill(MaskedPlayerEnemy enemy)
    {
        enemy.SetCrouchingServerRpc(!enemy.creatureAnimator.GetBool("Crouching"));
    }

    public float InteractRange(MaskedPlayerEnemy _)
    {
        return 1.0f;
    }

    public bool SyncAnimationSpeedEnabled(MaskedPlayerEnemy _)
    {
        return false;
    }
}