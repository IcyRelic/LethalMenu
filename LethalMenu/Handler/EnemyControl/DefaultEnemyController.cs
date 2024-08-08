namespace LethalMenu.Handler.EnemyControl
{
    internal class DefaultEnemyController : IEnemyController<EnemyAI>
    { 
        public float SprintMultiplier(EnemyAI _) => IController.DefaultSprintMultiplier;
        public bool CanUseEntranceDoors(EnemyAI _) => true;

        public float InteractRange(EnemyAI _) => IController.DefaultInteractRange;
    }
}
