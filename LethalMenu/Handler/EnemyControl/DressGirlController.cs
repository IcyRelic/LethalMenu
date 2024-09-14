using GameNetcodeStuff;
using UnityEngine;
using System.Linq;
using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl
{
    internal class DressGirlController : IEnemyController<DressGirlAI>
    {
        public void UsePrimarySkill(DressGirlAI enemy)
        {
            PlayerControllerB player = GetClosestPlayer(enemy);
            if (player != null) enemy.hauntingPlayer = player;
            else return;
            if (enemy.hauntingPlayer != null && enemy.currentBehaviourStateIndex != 1)
            {
                enemy.EnableEnemyMesh(true, true);
                enemy.SwitchToBehaviourStateOnLocalClient(0);
                enemy.Reflect().Invoke("BeginChasing");
            }
            else if (enemy.hauntingPlayer != null && enemy.currentBehaviourStateIndex == 1) enemy.Reflect().Invoke("StopChasing");
        }

        public void UseSecondarySkill(DressGirlAI enemy)
        {
        }

        public string GetPrimarySkillName(DressGirlAI enemy)
        {
            PlayerControllerB player = GetClosestPlayer(enemy);
            return player == null ? "No Players Alive" : (enemy.currentBehaviourStateIndex == 1 ? "Stop Chase" : "Begin Chase");
        }

        public string GetSecondarySkillName(DressGirlAI _) => "";

        public bool CanUseEntranceDoors(DressGirlAI _) => true;

        public float InteractRange(DressGirlAI _) => 5f;

        public static PlayerControllerB GetClosestPlayer(DressGirlAI enemy)
        {
            return LethalMenu.players.Where(p => p != null && !p.isPlayerDead).OrderBy(p => Vector3.Distance(enemy.transform.position, p.transform.position)).FirstOrDefault();
        }
    }
}