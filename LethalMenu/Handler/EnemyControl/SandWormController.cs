namespace LethalMenu.Handler.EnemyControl;

internal class SandWormController : IEnemyController<SandWormAI>
{
    public void UseSecondarySkill(SandWormAI enemy)
    {
        if (IsEmerged(enemy)) return;
        enemy.StartEmergeAnimation();
    }

    public string GetSecondarySkillName(SandWormAI _)
    {
        return "Emerge";
    }

    public bool CanUseEntranceDoors(SandWormAI _)
    {
        return false;
    }

    public float InteractRange(SandWormAI _)
    {
        return 0.0f;
    }

    public bool SyncAnimationSpeedEnabled(SandWormAI _)
    {
        return false;
    }

    private bool IsEmerged(SandWormAI enemy)
    {
        return enemy.inEmergingState || enemy.emerged;
    }
}