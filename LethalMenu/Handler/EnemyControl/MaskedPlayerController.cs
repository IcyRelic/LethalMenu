using UnityEngine;

namespace LethalMenu.Handler.EnemyControl;

internal class MaskedPlayerController : IEnemyController<MaskedPlayerEnemy>
{
    private static readonly int HandsOut = Animator.StringToHash("HandsOut");
    private static readonly int Crouching = Animator.StringToHash("Crouching");

    public void UsePrimarySkill(MaskedPlayerEnemy enemy)
    {
        enemy.SetHandsOutServerRpc(!enemy.creatureAnimator.GetBool(HandsOut));
    }

    public void UseSecondarySkill(MaskedPlayerEnemy enemy)
    {
        enemy.SetCrouchingServerRpc(!enemy.creatureAnimator.GetBool(Crouching));
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