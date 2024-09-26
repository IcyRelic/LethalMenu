using System.Threading.Tasks;

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
            StartEmergeCooldown();
        }

        public string GetPrimarySkillName(SandWormAI _) => EmergeCooldown ? $"Emerge Cooldown ({Seconds} seconds)" : "Emerge";

        public bool CanUseEntranceDoors(SandWormAI _) => false;

        public float InteractRange(SandWormAI _) => 0.0f;

        public bool SyncAnimationSpeedEnabled(SandWormAI _) => false;


        public static async void StartEmergeCooldown() => await EmergeCoolDown();

        public static async Task EmergeCoolDown()
        {
            EmergeCooldown = true;
            while (Seconds > 0)
            {
                await Task.Delay(1000);
                Seconds--;
            }
            EmergeCooldown = false;
            Seconds = 20;
        }
    }
}
