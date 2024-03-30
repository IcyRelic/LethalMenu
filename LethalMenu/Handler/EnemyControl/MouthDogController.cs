namespace LethalMenu.Handler.EnemyControl;

internal enum DogState
{
    ROAMING,
    SUSPICIOUS,
    CHASE,
    LUNGE
}

internal class MouthDogController : IEnemyController<MouthDogAI>
{
    public void OnMovement(MouthDogAI enemy, bool isMoving, bool isSprinting)
    {
        if (!isSprinting)
        {
            if (!isMoving) return;
            enemy.SetBehaviourState(DogState.ROAMING);
        }

        else
        {
            enemy.SetBehaviourState(DogState.CHASE);
        }
    }

    public void UseSecondarySkill(MouthDogAI enemy)
    {
        enemy.SetBehaviourState(DogState.LUNGE);
    }

    public string GetSecondarySkillName(MouthDogAI _)
    {
        return "Lunge";
    }

    public float InteractRange(MouthDogAI _)
    {
        return 2.5f;
    }
}