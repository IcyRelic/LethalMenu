using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl
{
    internal class DoublewingController : IEnemyController<DoublewingAI>
    {
        public void UsePrimarySkill(DoublewingAI enemy)
        {
            enemy.Reflect().Invoke("BirdScreech");
        }

        public void UseSecondarySkill(DoublewingAI enemy)
        {
        }

        public string GetPrimarySkillName(DoublewingAI _) => "Screech";
        public string GetSecondarySkillName(DoublewingAI _) => "";
        public bool CanUseEntranceDoors(DoublewingAI _) => false;
        public float InteractRange(DoublewingAI _) => 2.5f;
    }
}
