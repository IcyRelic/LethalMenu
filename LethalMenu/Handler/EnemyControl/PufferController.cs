namespace LethalMenu.Handler.EnemyControl
{
    public enum PufferState
    {
        IDLE,
        ALERTED,
        HOSTILE
    }
    internal class PufferController : IEnemyController<PufferAI>
    {
        public void UsePrimarySkill(PufferAI enemy)
        {
            enemy.SetBehaviourState(PufferState.HOSTILE);
            enemy.StompServerRpc();
        }

        public void UseSecondarySkill(PufferAI enemy)
        {
            enemy.SetBehaviourState(PufferState.HOSTILE);
            enemy.ShakeTailServerRpc();
        }

        public string GetPrimarySkillName(PufferAI _) => "Stomp";

        public string GetSecondarySkillName(PufferAI _) => "Smoke";

        public float InteractRange(PufferAI _) => 2.5f;

        public bool CanUseEntranceDoors(PufferAI _) => false;
    }
}
