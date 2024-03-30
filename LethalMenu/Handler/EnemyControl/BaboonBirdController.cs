using LethalMenu.Util;
using Unity.Netcode;
using UnityEngine;

namespace LethalMenu.Handler.EnemyControl
{
    internal class BaboonBirdController : IEnemyController<BaboonBirdAI>
    {
        Vector3 CustomCamp { get; } = new Vector3(1000.0f, 0.0f, 0.0f);
        Vector3 OriginalCamp { get; set; } = Vector3.zero;

        public void OnDeath(BaboonBirdAI enemy)
        {
            if (enemy.heldScrap is not null)
                enemy.Reflect().Invoke("DropHeldItemAndSync");
        }

        public void OnTakeControl(BaboonBirdAI _)
        {
            if (BaboonBirdAI.baboonCampPosition != this.CustomCamp) return;

            this.OriginalCamp = BaboonBirdAI.baboonCampPosition;
            BaboonBirdAI.baboonCampPosition = this.CustomCamp;
        }

        public void OnReleaseControl(BaboonBirdAI _)
        {
            if (BaboonBirdAI.baboonCampPosition == this.OriginalCamp) return;
            BaboonBirdAI.baboonCampPosition = this.OriginalCamp;
        }

        public void UsePrimarySkill(BaboonBirdAI enemy)
        {
            if (enemy.heldScrap is null && enemy.FindNearbyItem() is GrabbableObject grabbable)
            {
                GrabItemAndSync(enemy, grabbable);
                return;
            }

            if (enemy.heldScrap is ShotgunItem shotgun)
                shotgun.ShootGunAsEnemy(enemy);
        }

        public void UseSecondarySkill(BaboonBirdAI enemy)
        {
            if (enemy.heldScrap is null) return;
            enemy.Reflect().Invoke("DropHeldItemAndSync");
        }

        public string GetPrimarySkillName(BaboonBirdAI enemy) => enemy.heldScrap is not null ? "Use item" : "Grab Item";

        public string GetSecondarySkillName(BaboonBirdAI enemy) => enemy.heldScrap is null ? "" : "Drop item";

        public float InteractRange(BaboonBirdAI _) => 1.5f;

        void GrabItemAndSync(BaboonBirdAI enemy, GrabbableObject item)
        {
            ReflectionUtil<BaboonBirdAI> reflect = enemy.Reflect();
            NetworkObject netItem = item.GetComponent<NetworkObject>();
            if (enemy == null || item == null || netItem == null || reflect is null)
            {
                return;
            }

            enemy.SwitchToBehaviourServerRpc(1);

            reflect.Invoke("GrabItemAndSync", netItem);
        }
    }
}
