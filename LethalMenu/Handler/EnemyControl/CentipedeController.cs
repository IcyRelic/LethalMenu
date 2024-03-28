using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl
{
    enum CentipedeState
    {
        SEARCHING,
        HIDING,
        CHASING,
        CLINGING
    }
    internal class CentipedeController : IEnemyController<CentipedeAI>
    {
        bool IsClingingToSomething(CentipedeAI enemy)
        {
            ReflectionUtil<CentipedeAI> centipedeReflector = enemy.Reflect();

            return enemy.clingingToPlayer is not null || enemy.inSpecialAnimation ||
                   centipedeReflector.GetValue<bool>("clingingToDeadBody") ||
                   centipedeReflector.GetValue<bool>("clingingToCeiling") ||
                   centipedeReflector.GetValue<bool>("startedCeilingAnimationCoroutine") ||
                   centipedeReflector.GetValue<bool>("inDroppingOffPlayerAnim");
        }

        public void UsePrimarySkill(CentipedeAI enemy)
        {
            if (!enemy.IsBehaviourState(CentipedeState.HIDING)) return;
            enemy.SetBehaviourState(CentipedeState.CHASING);
        }

        public void UseSecondarySkill(CentipedeAI enemy)
        {
            if (this.IsClingingToSomething(enemy)) return;
            _ = enemy.Reflect().Invoke("RaycastToCeiling");
            enemy.SetBehaviourState(CentipedeState.HIDING);
        }

        public bool IsAbleToMove(CentipedeAI enemy) => !this.IsClingingToSomething(enemy);

        public bool IsAbleToRotate(CentipedeAI enemy) => !this.IsClingingToSomething(enemy);

        public string GetPrimarySkillName(CentipedeAI _) => "Drop";

        public string GetSecondarySkillName(CentipedeAI _) => "Attach to ceiling";

        public float InteractRange(CentipedeAI _) => 1.5f;

        public bool CanUseEntranceDoors(CentipedeAI _) => false;

        public bool SyncAnimationSpeedEnabled(CentipedeAI _) => false;
    }
}
