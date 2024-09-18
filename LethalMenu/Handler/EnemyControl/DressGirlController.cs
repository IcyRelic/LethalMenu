using GameNetcodeStuff;
using UnityEngine;
using System.Linq;
using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl
{
    internal class DressGirlController : IEnemyController<DressGirlAI>
    {
        // Buggy I'll attempt to redo this again later on
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

        public void OnTakeControl(DressGirlAI enemy)
        {
            if (!LethalMenu.localPlayer.IsHost)
            {
                HUDManager.Instance.DisplayTip("Lethal Menu", "This controller is host only. I'm sorry :C");
                Cheats.EnemyControl.StopControl();
                return;
            }
        }

        public void OnReleaseControl(DressGirlAI enemy)
        {
            enemy.EnableEnemyMesh(false, true);
        }

        public void OnDeath(DressGirlAI enemy)
        {
            enemy.EnableEnemyMesh(false, true);
        }

        public bool CanUseEntranceDoors(DressGirlAI _) => true;

        public float InteractRange(DressGirlAI _) => 5f;

        public static PlayerControllerB GetClosestPlayer(DressGirlAI enemy)
        {
            return LethalMenu.players.Where(p => p != null && !p.isPlayerDead).OrderBy(p => Vector3.Distance(enemy.transform.position, p.transform.position)).FirstOrDefault();
        }
    }
}