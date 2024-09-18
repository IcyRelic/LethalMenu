namespace LethalMenu.Handler.EnemyControl
{
    internal class DocileLocustBeesController : IEnemyController<DocileLocustBeesAI>
    {
        public bool CanUseEntranceDoors(DocileLocustBeesAI _) => false;
        public float InteractRange(DocileLocustBeesAI _) => 2.5f;
    }
}
