using GameNetcodeStuff;
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
            if (enemy == null || enemy.heldScrap == null) return;
            enemy.Reflect().Invoke("DropHeldItemAndSync");
        }
        public void OnTakeControl(BaboonBirdAI enemy)
        {
            if (enemy == null || BaboonBirdAI.baboonCampPosition != CustomCamp) return;
            OriginalCamp = BaboonBirdAI.baboonCampPosition;
            BaboonBirdAI.baboonCampPosition = CustomCamp;
        }

        public void OnReleaseControl(BaboonBirdAI enemy)
        {
            if (enemy == null || BaboonBirdAI.baboonCampPosition == OriginalCamp) return;
            BaboonBirdAI.baboonCampPosition = OriginalCamp;
        }

        public void UsePrimarySkill(BaboonBirdAI enemy)
        {
            if (enemy == null) return;
            if (enemy.heldScrap is ShotgunItem shotgun) shotgun.ShootGunAsEnemy(enemy);
            else if (enemy.FindNearbyItem(5) is GrabbableObject i && i.GetComponent<NetworkObject>() is NetworkObject netitem)
            {
                GrabItemAndSync(enemy, netitem);
            }
        }

        public void UseSecondarySkill(BaboonBirdAI enemy)
        {
            if (enemy == null || enemy.heldScrap == null) return;
            enemy.Reflect().Invoke("DropHeldItemAndSync");
        }

        public string GetPrimarySkillName(BaboonBirdAI enemy) => enemy.heldScrap is ShotgunItem ? "Use item" : (enemy.heldScrap != null ? "Drop item (Right Click)" : "Grab Item");

        public string GetSecondarySkillName(BaboonBirdAI enemy) => enemy.heldScrap != null ? "" : "Drop Item";

        public float InteractRange(BaboonBirdAI _) => 5f;

        public bool CanUseEntranceDoors(BaboonBirdAI _) => false;

        public static void GrabItemAndSync(BaboonBirdAI enemy, NetworkObject netitem)
        {
            if (netitem == null) return;
            GrabScrap(enemy, netitem);
            enemy.GrabScrapServerRpc(netitem, (int)(LethalMenu.localPlayer?.playerClientId ?? 0));
        }

        public static void GrabScrap(BaboonBirdAI enemy, NetworkObject netitem)
        {
            if (enemy.heldScrap != null) enemy.Reflect().Invoke("DropHeldItemAndSync");
            GrabbableObject item = (enemy.heldScrap = netitem.gameObject.GetComponent<GrabbableObject>());
            item.parentObject = enemy.grabTarget;
            item.hasHitGround = false;
            item.GrabItemFromEnemy(enemy);
            item.isHeldByEnemy = true;
            item.EnablePhysics(false);
        }
    }
}
