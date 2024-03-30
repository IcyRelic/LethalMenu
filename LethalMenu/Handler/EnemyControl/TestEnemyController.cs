namespace LethalMenu.Handler.EnemyControl;

internal class TestEnemyController : IEnemyController<TestEnemy>
{
    public bool CanUseEntranceDoors(TestEnemy _)
    {
        return true;
    }

    public float InteractRange(TestEnemy _)
    {
        return 4.5f;
    }
}