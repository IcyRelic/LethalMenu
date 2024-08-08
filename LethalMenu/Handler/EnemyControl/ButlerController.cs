namespace LethalMenu.Handler.EnemyControl
{
    internal class ButlerController : IEnemyController<ButlerEnemyAI>
    {
        public void UsePrimarySkill(ButlerEnemyAI enemy)
        {
            enemy.SyncSearchingMadlyServerRpc(isSearching: true);
        }

        public void UseSecondarySkill(ButlerEnemyAI enemy)
        {
            enemy.SetSweepingAnimClientRpc(sweeping: true);
        }

        public string GetPrimarySkillName(ButlerEnemyAI _) => "Search Mad";

        public string GetSecondarySkillName(ButlerEnemyAI _) => "Sweep";

        public bool CanUseEntranceDoors(ButlerEnemyAI _) => false;

        public float InteractRange(ButlerEnemyAI _) => 5f;
    }
}
