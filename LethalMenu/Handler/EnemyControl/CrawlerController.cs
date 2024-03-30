namespace LethalMenu.Handler.EnemyControl;

internal class CrawlerController : IEnemyController<CrawlerAI>
{
    public float InteractRange(CrawlerAI _)
    {
        return 1.5f;
    }

    public bool SyncAnimationSpeedEnabled(CrawlerAI _)
    {
        return false;
    }

    public void UseSecondarySkill(CrawlerAI enemy)
    {
        enemy.MakeScreechNoiseServerRpc();
    }

    public void UsePrimarySkill(CrawlerAI enemy)
    {
        enemy.CollideWithWallServerRpc();
    }
}