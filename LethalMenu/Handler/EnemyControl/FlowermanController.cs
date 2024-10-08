﻿namespace LethalMenu.Handler.EnemyControl
{

    enum FlowermanState
    {
        SCOUTING,
        STAND,
        ANGER
    }
    internal class FlowermanController : IEnemyController<FlowermanAI>
    {
        public void UsePrimarySkill(FlowermanAI enemy)
        {
            if (!enemy.carryingPlayerBody)
            {
                enemy.SetBehaviourState(FlowermanState.ANGER);
            }

            enemy.DropPlayerBodyServerRpc();
        }

        public void UseSecondarySkill(FlowermanAI enemy) => enemy.SetBehaviourState(FlowermanState.STAND);

        public void ReleaseSecondarySkill(FlowermanAI enemy) => enemy.SetBehaviourState(FlowermanState.SCOUTING);

        public bool IsAbleToMove(FlowermanAI enemy) => !enemy.inSpecialAnimation;

        public string GetPrimarySkillName(FlowermanAI enemy) => enemy.carryingPlayerBody ? "Drop body" : "";

        public string GetSecondarySkillName(FlowermanAI _) => "Stand";

        public float InteractRange(FlowermanAI _) => 1.5f;

        public bool SyncAnimationSpeedEnabled(FlowermanAI _) => false;

        public bool CanUseEntranceDoors(FlowermanAI _) => false;
    }
}
