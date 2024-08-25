namespace LethalMenu.Handler.EnemyControl
{
    internal class RadMechController : IEnemyController<RadMechAI>
    {
        public string GetPrimarySkillName(RadMechAI _) => "";

        public string GetSecondarySkillName(RadMechAI _) => "";

        public bool CanUseEntranceDoors(RadMechAI _) => false;

        public float InteractRange(RadMechAI _) => 10f;
    }
}
