using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl
{
    internal class DoublewingController : IEnemyController<DoublewingAI>
    {
        // haven't tested yet
        public void UsePrimarySkill(DoublewingAI enemy)
        {
            enemy.AlertBirdServerRpc();
        }

        public void UseSecondarySkill(DoublewingAI enemy)
        {
            enemy.Reflect().Invoke("BirdScreech");
        }

        public string GetPrimarySkillName(DoublewingAI _)
        {
            return _.Reflect().GetValue<bool>("alertingBird") ? "Alert Bird" : "Screech (Right Click)";
        }
        public string GetSecondarySkillName(DoublewingAI _) => "Screech";
        public bool CanUseEntranceDoors(DoublewingAI _) => false;
        public float InteractRange(DoublewingAI _) => 2.5f;
    }
}
