using System;
using System.Reflection;
using UnityEngine;

namespace LethalMenu.Handler.EnemyControl
{
    internal class DressGirlController : IEnemyController<DressGirlAI>
    {
        public void UsePrimarySkill(DressGirlAI enemy)
        {
        }

        public void UseSecondarySkill(DressGirlAI enemy)
        {
        }

        public string GetPrimarySkillName(DressGirlAI _) => "";

        public string GetSecondarySkillName(DressGirlAI _) => "";

        public bool CanUseEntranceDoors(DressGirlAI _) => true;

        public float InteractRange(DressGirlAI _) => 5f;
    }
}
