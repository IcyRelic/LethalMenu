using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl
{
    internal class SpringManController : IEnemyController<SpringManAI>
    {
        bool GetStoppingMovement(SpringManAI enemy) => enemy.Reflect().GetValue<bool>("stoppingMovement");

        public void OnSecondarySkillHold(SpringManAI enemy) => enemy.SetAnimationGoServerRpc();

        public void ReleaseSecondarySkill(SpringManAI enemy) => enemy.SetAnimationStopServerRpc();

        public bool IsAbleToMove(SpringManAI enemy) => !this.GetStoppingMovement(enemy);

        public bool IsAbleToRotate(SpringManAI enemy) => !this.GetStoppingMovement(enemy);

        public float InteractRange(SpringManAI _) => 1.5f;

        public bool CanUseEntranceDoors(SpringManAI _) => false;
    }
}
