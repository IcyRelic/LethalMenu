using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl;

internal class SpringManController : IEnemyController<SpringManAI>
{
    public void OnSecondarySkillHold(SpringManAI enemy)
    {
        enemy.SetAnimationGoServerRpc();
    }

    public void ReleaseSecondarySkill(SpringManAI enemy)
    {
        enemy.SetAnimationStopServerRpc();
    }

    public bool IsAbleToMove(SpringManAI enemy)
    {
        return !GetStoppingMovement(enemy);
    }

    public bool IsAbleToRotate(SpringManAI enemy)
    {
        return !GetStoppingMovement(enemy);
    }

    public float InteractRange(SpringManAI _)
    {
        return 1.5f;
    }

    private bool GetStoppingMovement(SpringManAI enemy)
    {
        return enemy.Reflect().GetValue<bool>("stoppingMovement");
    }
}