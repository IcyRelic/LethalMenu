namespace LethalMenu.Handler.EnemyControl
{
    internal class RadMechController : IEnemyController<RadMechAI>
    {
        public string GetPrimarySkillName(RadMechAI _) => "Shoot Gun";

        public string GetSecondarySkillName(RadMechAI _) => "Aim Gun";

        public bool CanUseEntranceDoors(RadMechAI _) => false;

        public float InteractRange(RadMechAI _) => 10f;
    }
}
