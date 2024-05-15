namespace LethalMenu.Handler.EnemyControl
{
    internal class DoublewingController : IEnemyController<DoublewingAI>
    {
        public bool CanUseEntranceDoors(DoublewingAI _) => true;

        public float InteractRange(DoublewingAI _) => 2.5f;
    }
}
