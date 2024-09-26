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
            DropHeldItemAndSync(enemy);
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
            DropHeldItemAndSync(enemy);
        }

        public string GetPrimarySkillName(BaboonBirdAI enemy) => enemy.heldScrap is ShotgunItem ? "Use item" : (enemy.heldScrap != null ? "Drop item (Right Click)" : "Grab Item");

        public string GetSecondarySkillName(BaboonBirdAI enemy) => enemy.heldScrap != null ? "" : "Drop Item";

        public float InteractRange(BaboonBirdAI _) => 5f;

        public bool CanUseEntranceDoors(BaboonBirdAI _) => false;

        public static void GrabItemAndSync(BaboonBirdAI enemy, NetworkObject netitem)
        {
            if (netitem == null) return;
            GrabScrap(enemy, netitem);
            enemy.GrabScrapServerRpc(netitem, (int)LethalMenu.localPlayer.playerClientId);
        }

        public static void GrabScrap(BaboonBirdAI enemy, NetworkObject netitem)
        {
            if (enemy.heldScrap != null) DropScrap(enemy, enemy.heldScrap.GetComponent<NetworkObject>(), enemy.heldScrap.GetItemFloorPosition());
            GrabbableObject item = (enemy.heldScrap = netitem.gameObject.GetComponent<GrabbableObject>());
            item.parentObject = enemy.grabTarget;
            item.hasHitGround = false;
            item.GrabItemFromEnemy(enemy);
            item.isHeldByEnemy = true;
            item.EnablePhysics(false);
        }

        public static void DropHeldItemAndSync(BaboonBirdAI enemy)
        {
            NetworkObject netobject = enemy.heldScrap.NetworkObject;
            Vector3 itemFloorPosition = enemy.heldScrap.GetItemFloorPosition();
            DropScrap(enemy, netobject, itemFloorPosition);
            enemy.DropScrapServerRpc(netobject, itemFloorPosition, (int)LethalMenu.localPlayer.playerClientId);
        }

        public static void DropScrap(BaboonBirdAI enemy, NetworkObject item, Vector3 targetFloorPosition)
        {
            if (enemy.heldScrap == null) return;
            if (enemy.heldScrap.isHeld)
            {
                enemy.heldScrap.DiscardItemFromEnemy();
                enemy.heldScrap.isHeldByEnemy = false;
                enemy.heldScrap = null;
                return;
            }
            enemy.heldScrap.parentObject = null;
            enemy.heldScrap.transform.SetParent(StartOfRound.Instance.propsContainer, true);
            enemy.heldScrap.EnablePhysics(true);
            enemy.heldScrap.fallTime = 0f;
            enemy.heldScrap.startFallingPosition = enemy.heldScrap.transform.parent.InverseTransformPoint(enemy.heldScrap.transform.position);
            enemy.heldScrap.targetFloorPosition = enemy.heldScrap.transform.parent.InverseTransformPoint(targetFloorPosition);
            enemy.heldScrap.floorYRot = -1;
            enemy.heldScrap.DiscardItemFromEnemy();
            enemy.heldScrap.isHeldByEnemy = false;
            enemy.heldScrap = null;
        }
    }
}
