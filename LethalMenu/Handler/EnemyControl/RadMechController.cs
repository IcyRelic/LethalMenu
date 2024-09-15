namespace LethalMenu.Handler.EnemyControl
{
    internal class RadMechController : IEnemyController<RadMechAI>
    {
        // This isn't possible atm
        public void OnTakeControl(RadMechAI enemy)
        {
        }

        public void OnReleaseControl(RadMechAI enemy)
        {
        }

        public string GetPrimarySkillName(RadMechAI _) => "Fire";

        public void UsePrimarySkill(RadMechAI enemy)
        {
        }

        public string GetSecondarySkillName(RadMechAI _) => "";

        public bool CanUseEntranceDoors(RadMechAI _) => false;

        public float InteractRange(RadMechAI _) => 10f;
    }
}