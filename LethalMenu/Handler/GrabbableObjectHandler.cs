namespace LethalMenu.Handler;

internal class GrabbableObjectHandler
{
}

public static class ShotgunItemExtensions
{
    public static void ShootGunAsEnemy(this ShotgunItem shotgun, EnemyAI enemy)
    {
        shotgun.gunShootAudio.volume = 0.15f;
        shotgun.shotgunRayPoint = enemy.transform;
        shotgun.ShootGunAndSync(false);
    }
}