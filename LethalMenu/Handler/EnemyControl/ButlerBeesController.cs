namespace LethalMenu.Handler.EnemyControl
{
    internal class ButlerBeesController : IEnemyController<ButlerBeesEnemyAI>
    {
        public bool CanUseEntranceDoors(ButlerBeesEnemyAI _) => true;

        public float InteractRange(ButlerBeesEnemyAI _) => 2.5f;
    }
}
