using LethalMenu.Util;

namespace LethalMenu.Handler.EnemyControl
{
    internal class CaveDwellerController : IEnemyController<CaveDwellerAI>
    {
        public void UsePrimarySkill(CaveDwellerAI enemy)
        {
            if (!enemy.adultContainer.activeSelf) TransformIntoAdult(enemy);
        }

        public void UseSecondarySkill(CaveDwellerAI enemy)
        {
        }

        public string GetPrimarySkillName(CaveDwellerAI enemy) => !enemy.adultContainer.activeSelf ? "Transform To Adult" : "";

        public string GetSecondarySkillName(CaveDwellerAI enemy) => "";

        public bool CanUseEntranceDoors(CaveDwellerAI _) => true;

        public float InteractRange(CaveDwellerAI _) => 5f;

        public void TransformIntoAdult(CaveDwellerAI enemy)
        {
            enemy.SwitchToBehaviourStateOnLocalClient(1);
            enemy.TurnIntoAdultServerRpc();
            enemy.Reflect().Invoke("StartTransformationAnim");
        }
    }
}
