namespace LethalMenu.Handler.EnemyControl;

internal enum NutcrackerState
{
    WALKING,
    SENTRY
}

internal class NutcrackerController : IEnemyController<NutcrackerEnemyAI>
{
    private bool InSentryMode { get; set; }

    public void Update(NutcrackerEnemyAI enemy, bool isAIControlled)
    {
        if (isAIControlled) return;
        if (InSentryMode) return;
        enemy.SwitchToBehaviourServerRpc((int)NutcrackerState.WALKING); // See #415
    }

    public bool IsAbleToRotate(NutcrackerEnemyAI enemy)
    {
        return !enemy.IsBehaviourState(NutcrackerState.SENTRY);
    }

    public bool IsAbleToMove(NutcrackerEnemyAI enemy)
    {
        return !enemy.IsBehaviourState(NutcrackerState.SENTRY);
    }

    public void UsePrimarySkill(NutcrackerEnemyAI enemy)
    {
        if (enemy.gun is not { } shotgun) return;

        shotgun.gunShootAudio.volume = 0.25f;
        enemy.FireGunServerRpc();
    }

    public void OnSecondarySkillHold(NutcrackerEnemyAI enemy)
    {
        enemy.SetBehaviourState(NutcrackerState.SENTRY);
        InSentryMode = true;
    }

    public void ReleaseSecondarySkill(NutcrackerEnemyAI enemy)
    {
        enemy.SetBehaviourState(NutcrackerState.WALKING);
        InSentryMode = false;
    }

    public void OnReleaseControl(NutcrackerEnemyAI enemy)
    {
        InSentryMode = false;
    }

    public string GetPrimarySkillName(NutcrackerEnemyAI enemy)
    {
        return enemy.gun is null ? "" : "Fire";
    }

    public string GetSecondarySkillName(NutcrackerEnemyAI _)
    {
        return "(HOLD) Sentry mode";
    }

    public float InteractRange(NutcrackerEnemyAI _)
    {
        return 1.5f;
    }
}