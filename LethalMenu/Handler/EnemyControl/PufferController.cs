using GameNetcodeStuff;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Handler.EnemyControl
{
    public enum PufferState
    {
        IDLE,
        ALERTED,
        HOSTILE
    }

    internal class PufferController : IEnemyController<PufferAI>
    {
        private static bool playerIsInLOS = false;
        private static PlayerControllerB player;

        public void UsePrimarySkill(PufferAI enemy)
        {
            enemy.SetBehaviourState(PufferState.HOSTILE);
            player = DressGirlController.GetClosestPlayer(enemy);
            if (player == null) return;
            playerIsInLOS = player;
            enemy.Reflect().SetValue("closestSeenPlayer", player);
            enemy.Reflect().SetValue("playerIsInLOS", playerIsInLOS);
            enemy.StompServerRpc();
            player = null;
            playerIsInLOS = false;
        }

        public void UseSecondarySkill(PufferAI enemy)
        {
            enemy.SetBehaviourState(PufferState.HOSTILE);
            player = DressGirlController.GetClosestPlayer(enemy);
            if (player == null) return;
            playerIsInLOS = player;
            enemy.Reflect().SetValue("closestSeenPlayer", player);
            enemy.Reflect().SetValue("playerIsInLOS", playerIsInLOS);
            enemy.ShakeTailServerRpc();
            player = null;
            playerIsInLOS = false;
        }

        public string GetPrimarySkillName(PufferAI _) => "Stomp";

        public string GetSecondarySkillName(PufferAI _) => "Smoke";

        public float InteractRange(PufferAI _) => 2.5f;

        public bool CanUseEntranceDoors(PufferAI _) => false;
    }
}
