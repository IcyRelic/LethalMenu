namespace LethalMenu.Handler.EnemyControl
{
    internal class FlowerSnakeController : IEnemyController<FlowerSnakeEnemy>
    {
        public void UsePrimarySkill(FlowerSnakeEnemy enemy, int playerId, int setClingPosition, float clingTime)
        {
            enemy.ClingToPlayerClientRpc(playerId, setClingPosition, clingTime);
        }

        public void UseSecondarySkill(FlowerSnakeEnemy enemy)
        {
            enemy.StartFlyingClientRpc();
        }

        public string GetPrimarySkillName(FlowerSnakeEnemy _) => "Cling";

        public string GetSecondarySkillName(FlowerSnakeEnemy _) => "Fly";

        public bool CanUseEntranceDoors(FlowerSnakeEnemy _) => true;

        public float InteractRange(FlowerSnakeEnemy _) => 5f;
    }
}
