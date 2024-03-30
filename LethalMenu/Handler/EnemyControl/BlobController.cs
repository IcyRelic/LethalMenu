using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl;

internal class BlobController : IEnemyController<BlobAI>
{
    public void OnSecondarySkillHold(BlobAI enemy)
    {
        SetAngeredTimer(enemy, 0.0f);
        SetTamedTimer(enemy, 2.0f);
    }

    public void ReleaseSecondarySkill(BlobAI enemy)
    {
        SetTamedTimer(enemy, 0.0f);
    }

    public void UsePrimarySkill(BlobAI enemy)
    {
        SetAngeredTimer(enemy, 18.0f);
    }

    public float InteractRange(BlobAI _)
    {
        return 3.5f;
    }

    public float SprintMultiplier(BlobAI _)
    {
        return 9.8f;
    }

    private void SetTamedTimer(BlobAI enemy, float time)
    {
        enemy.Reflect().SetValue("tamedTimer", time);
    }

    private void SetAngeredTimer(BlobAI enemy, float time)
    {
        enemy.Reflect().SetValue("angeredTimer", time);
    }
}