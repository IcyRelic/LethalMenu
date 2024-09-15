using LethalMenu.Util;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalMenu.Handler.EnemyControl
{
    internal class ClaySurgeonController : IEnemyController<ClaySurgeonAI>
    {
        public void OnTakeControl(ClaySurgeonAI enemy)
        {
            enemy.SyncMasterClaySurgeonClientRpc();
        }

        public string GetPrimarySkillName(ClaySurgeonAI _) => "";

        public string GetSecondarySkillName(ClaySurgeonAI _) => "";

        public void OnMovement(ClaySurgeonAI enemy, bool isMoving, bool isSprinting)
        {
            if (isSprinting || isMoving && enemy != null && !IsJumpingAndSnipping(enemy)) return;
        }

        public bool IsJumpingAndSnipping(ClaySurgeonAI enemy)
        {
            return enemy.isJumping && enemy.agent.speed > 0.0;
        }

        public bool CanUseEntranceDoors(ClaySurgeonAI _) => false;

        public float InteractRange(ClaySurgeonAI _) => 2.5f;
    }
}
