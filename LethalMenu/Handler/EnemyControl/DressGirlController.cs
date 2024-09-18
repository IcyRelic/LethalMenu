using GameNetcodeStuff;
using UnityEngine;
using System.Linq;
using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl
{
    internal class DressGirlController : IEnemyController<DressGirlAI>
    {
        public void Update(DressGirlAI enemy, bool AIControlled)
        {
            if (AIControlled || enemy == null) return;
            Collider collider = enemy.GetComponent<Collider>();
            if (collider == null) return;
            PlayerControllerB player = enemy.MeetsStandardPlayerCollisionConditions(collider, false, true);
            if (player != null && player == enemy.hauntingPlayer && enemy.currentBehaviourStateIndex == 1)
            {
                player.KillPlayer(Vector3.zero, true, CauseOfDeath.Unknown, 1);
                enemy.Reflect().Invoke("StopChasing");
                enemy.EnableEnemyMesh(false, true);
                enemy.creatureSFX.Stop();
            }
        }

        public void UsePrimarySkill(DressGirlAI enemy)
        {
            if (enemy == null) return;
            PlayerControllerB player = GetClosestPlayer(enemy);
            if (player != null) enemy.hauntingPlayer = player;
            else return;
            if (enemy.hauntingPlayer != null && !enemy.creatureAnimator.GetBool("Walk"))
            {
                enemy.EnableEnemyMesh(true, true);
                enemy.SwitchToBehaviourStateOnLocalClient(0);
                enemy.Reflect().Invoke("BeginChasing");
            }
            else if (enemy.hauntingPlayer != null && enemy.creatureAnimator.GetBool("Walk")) enemy.Reflect().Invoke("StopChasing");
        }

        public string GetPrimarySkillName(DressGirlAI enemy)
        {
            PlayerControllerB player = GetClosestPlayer(enemy);
            return player == null ? "" : (enemy.currentBehaviourStateIndex == 1 ? "Stop Chase" : "Begin Chase");
        }

        public void OnReleaseControl(DressGirlAI enemy)
        {
            if (enemy.creatureAnimator.GetBool("Walk")) enemy.Reflect().Invoke("StopChasing");
            enemy.EnableEnemyMesh(false, true);
            enemy.hauntingPlayer = null;
        }

        public void OnDeath(DressGirlAI enemy)
        {
            if (enemy.creatureAnimator.GetBool("Walk")) enemy.Reflect().Invoke("StopChasing");
            enemy.EnableEnemyMesh(false, true);
            enemy.hauntingPlayer = null;
        }

        public bool CanUseEntranceDoors(DressGirlAI _) => true;

        public float InteractRange(DressGirlAI _) => 5f;

        public static PlayerControllerB GetClosestPlayer(DressGirlAI e) => LethalMenu.players.Where(p => p != null && !p.isPlayerDead).OrderBy(p => Vector3.Distance(e.transform.position, p.transform.position)).FirstOrDefault();
    }
}