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
            if (BaboonBirdAI.baboonCampPosition != CustomCamp) return;
            OriginalCamp = BaboonBirdAI.baboonCampPosition;
            BaboonBirdAI.baboonCampPosition = CustomCamp;
        }

        public void OnReleaseControl(BaboonBirdAI _)
        {
            if (BaboonBirdAI.baboonCampPosition == OriginalCamp) return;
            BaboonBirdAI.baboonCampPosition = OriginalCamp;
        }

        public void UsePrimarySkill(BaboonBirdAI enemy)
        {
            if (enemy.heldScrap is null && enemy.FindNearbyItem() is GrabbableObject grabbable)
            {
                GrabItemAndSync(enemy, grabbable);
                return;
            }
            if (enemy.heldScrap is ShotgunItem shotgun)
            {
               shotgun.ShootGunAsEnemy(enemy);
            }
        }

        public void UseSecondarySkill(BaboonBirdAI enemy)
        {
            if (enemy.heldScrap is null) return;
            enemy.Reflect().Invoke("DropHeldItemAndSync");
        }

        public string GetPrimarySkillName(BaboonBirdAI enemy) => enemy.heldScrap is not null ? "Use item" : "Grab Item";

        public string GetSecondarySkillName(BaboonBirdAI enemy) => enemy.heldScrap is null ? "" : "Drop item";

        public float InteractRange(BaboonBirdAI _) => 5f;

        public void GrabItemAndSync(BaboonBirdAI enemy, GrabbableObject item)
        {
            NetworkObject netItem = item.GetComponent<NetworkObject>();
            if (enemy.heldScrap != null || item == null || netItem == null) return;
            enemy.SwitchToBehaviourServerRpc(1);
            enemy.Reflect().Invoke("GrabItemAndSync", netItem);
        }
    }
}
