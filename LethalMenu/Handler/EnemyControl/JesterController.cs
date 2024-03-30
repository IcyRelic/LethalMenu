using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl;

internal enum JesterState
{
    CLOSED,
    CRANKING,
    OPEN
}

internal class JesterController : IEnemyController<JesterAI>
{
    public void UsePrimarySkill(JesterAI enemy)
    {
        enemy.SetBehaviourState(JesterState.CLOSED);
        SetNoPlayerChasetimer(enemy, 0.0f);
    }

    public void OnSecondarySkillHold(JesterAI enemy)
    {
        if (!enemy.IsBehaviourState(JesterState.CLOSED)) return;
        enemy.SetBehaviourState(JesterState.CRANKING);
    }

    public void ReleaseSecondarySkill(JesterAI enemy)
    {
        if (!enemy.IsBehaviourState(JesterState.CRANKING)) return;
        enemy.SetBehaviourState(JesterState.OPEN);
    }

    public void Update(JesterAI enemy, bool isAIControlled)
    {
        SetNoPlayerChasetimer(enemy, 100.0f);
    }

    public void OnReleaseControl(JesterAI enemy)
    {
        SetNoPlayerChasetimer(enemy, 5.0f);
    }

    public bool IsAbleToMove(JesterAI enemy)
    {
        return !enemy.IsBehaviourState(JesterState.CRANKING);
    }

    public bool IsAbleToRotate(JesterAI enemy)
    {
        return !enemy.IsBehaviourState(JesterState.CRANKING);
    }

    public string GetPrimarySkillName(JesterAI _)
    {
        return "Close box";
    }

    public string GetSecondarySkillName(JesterAI _)
    {
        return "(HOLD) Begin cranking";
    }

    public float InteractRange(JesterAI _)
    {
        return 1.0f;
    }

    private void SetNoPlayerChasetimer(JesterAI enemy, float value)
    {
        enemy.Reflect().SetValue("noPlayersToChaseTimer", value);
    }
}