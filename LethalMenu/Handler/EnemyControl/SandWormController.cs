namespace LethalMenu.Handler.EnemyControl
{
    internal class SandWormController : IEnemyController<SandWormAI>
    {
        public void Update(SandWormAI enemy, bool isAIControlled)
        {
            if (enemy == null || isAIControlled) return;
            enemy.chaseTimer = 2f;
        }

        public void UsePrimarySkill(SandWormAI enemy)
        {
            if (enemy == null || enemy.inEmergingState) return;
            enemy.StartEmergeAnimation();
        }

        public string GetPrimarySkillName(SandWormAI _) => "Emerge";

        public bool CanUseEntranceDoors(SandWormAI _) => false;

        public float InteractRange(SandWormAI _) => 0.0f;

        public bool SyncAnimationSpeedEnabled(SandWormAI _) => false;
    }
}
