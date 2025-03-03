using System.Collections;
using UnityEngine;

namespace LethalMenu.Handler.EnemyControl
{
    internal class SandWormController : IEnemyController<SandWormAI>
    {
        public static bool EmergeCooldown = false;
        private static int Seconds = 20;

        public void Update(SandWormAI enemy, bool isAIControlled)
        {
            if (enemy == null || isAIControlled) return;
            enemy.chaseTimer = 2f;
        }

        public void UsePrimarySkill(SandWormAI enemy)
        {
            if (enemy == null || enemy.inEmergingState || EmergeCooldown) return;
            enemy.StartEmergeAnimation();
            LethalMenu.Instance.StartCoroutine(StartEmergeCoolDown());
        }

        public string GetPrimarySkillName(SandWormAI _) => EmergeCooldown ? $"Emerge Cooldown ({Seconds} seconds)" : "Emerge";

        public bool CanUseEntranceDoors(SandWormAI _) => false;

        public float InteractRange(SandWormAI _) => 0.0f;

        public bool SyncAnimationSpeedEnabled(SandWormAI _) => false;

        private IEnumerator StartEmergeCoolDown()
        {
            EmergeCooldown = true;
            Seconds = 20;
            while (Seconds > 0)
            {
                yield return new WaitForSeconds(1f);
                Seconds--;
            }
            EmergeCooldown = false;
        }
    }
}
