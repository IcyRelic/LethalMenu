using GameNetcodeStuff;
using LethalMenu.Util;
using Unity.Netcode;
using UnityEngine;

namespace LethalMenu.Handler.EnemyControl
{
    enum BugState
    {
        IDLE,
        SEARCHING_FOR_ITEMS,
        RETURNING_TO_NEST,
        CHASING_PLAYER,
        WATCHING_PLAYER,
        AT_NEST
    }

    internal class HoarderBugController : IEnemyController<HoarderBugAI>
    {
        public void UseHeldItem(HoarderBugAI enemy)
        {
            if (enemy.heldItem == null || enemy.heldItem.itemGrabbableObject == null) return;
            if (enemy.heldItem.itemGrabbableObject is ShotgunItem shotgun) shotgun.ShootGunAsEnemy(enemy);
        }

        public void Update(HoarderBugAI enemy, bool isAIControlled)
        {
            if (enemy == null || isAIControlled) return;
            enemy.angryTimer = 0.0f;
            enemy.SetBehaviourState(BugState.IDLE);
        }

        public void OnDeath(HoarderBugAI enemy)
        {
            if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject netitem)) return;
            DropItemAndCallDropRPC(enemy, netitem, false);
        }

        public void UsePrimarySkill(HoarderBugAI enemy)
        {
            if (enemy == null) return;
            if (enemy.angryTimer > 0.0f)
            {
                enemy.angryTimer = 0.0f;
                enemy.angryAtPlayer = null;
                enemy.SetBehaviourState(BugState.IDLE);
            }
            GrabbableObject item = enemy.FindNearbyItem(5);
            if (item != null && item.TryGetComponent(out NetworkObject netitem)) GrabItemAndCallGrabRPC(enemy, netitem);
            else UseHeldItem(enemy);
        }

        public void UseSecondarySkill(HoarderBugAI enemy)
        {
            if (enemy.heldItem?.itemGrabbableObject is null)
            {
                PlayerControllerB hostPlayer = StartOfRound.Instance.allPlayerScripts[0];
                enemy.watchingPlayer = hostPlayer;
                enemy.angryAtPlayer = hostPlayer;
                enemy.angryTimer = 15.0f;
                enemy.SetBehaviourState(BugState.CHASING_PLAYER);
                return;
            }
            if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject netitem)) DropItemAndCallDropRPC(enemy, netitem, false);
        }

        public string GetPrimarySkillName(HoarderBugAI enemy) => (enemy == null || enemy.heldItem == null) ? "Grab Item" : (enemy.heldItem.itemGrabbableObject is ShotgunItem ? "Use item" : "Drop item (Right Click)");

        public string GetSecondarySkillName(HoarderBugAI enemy) => enemy.heldItem.itemGrabbableObject != null ? "" : "Drop Item";

        public float InteractRange(HoarderBugAI _) => 1.5f;

        public bool CanUseEntranceDoors(HoarderBugAI _) => false;

        public static void GrabItemAndCallGrabRPC(HoarderBugAI enemy, NetworkObject item)
        {
            GrabItem(enemy, item);
            enemy.Reflect().SetValue("sendingGrabOrDropRPC", true);
            enemy.GrabItemServerRpc(item);
        }

        public static void GrabItem(HoarderBugAI enemy, NetworkObject item)
        {
            if (enemy.Reflect().GetValue<bool>("sendingGrabOrDropRPC"))
            {
                enemy.Reflect().SetValue("sendingGrabOrDropRPC", false);
                return;
            }
            if (enemy.heldItem != null) DropItem(enemy, enemy.heldItem.itemGrabbableObject.GetComponent<NetworkObject>(), enemy.heldItem.itemGrabbableObject.GetItemFloorPosition());
            enemy.targetItem = null;
            GrabbableObject component = item.gameObject.GetComponent<GrabbableObject>();
            HoarderBugAI.HoarderBugItems.Add(new HoarderBugItem(component, HoarderBugItemStatus.Owned, enemy.nestPosition));
            enemy.heldItem = HoarderBugAI.HoarderBugItems[HoarderBugAI.HoarderBugItems.Count - 1];
            component.parentObject = enemy.grabTarget;
            component.hasHitGround = false;
            component.GrabItemFromEnemy(enemy);
            component.EnablePhysics(false);
            HoarderBugAI.grabbableObjectsInMap.Remove(component.gameObject);
        }

        private void DropItemAndCallDropRPC(HoarderBugAI enemy, NetworkObject dropItemNetworkObject, bool droppedInNest = true)
        {
            Vector3 targetFloorPosition = RoundManager.Instance.RandomlyOffsetPosition(enemy.heldItem.itemGrabbableObject.GetItemFloorPosition(), 1.2f, 0.4f);
            DropItem(enemy, dropItemNetworkObject, targetFloorPosition);
            enemy.Reflect().SetValue("sendingGrabOrDropRPC", true);
            enemy.DropItemServerRpc(dropItemNetworkObject, targetFloorPosition, droppedInNest);
        }

        public static void DropItem(HoarderBugAI enemy, NetworkObject item, Vector3 targetFloorPosition, bool droppingInNest = true)
        {
            if (enemy.Reflect().GetValue<bool>("sendingGrabOrDropRPC"))
            {
                enemy.Reflect().SetValue("sendingGrabOrDropRPC", false);
                return;
            }
            if (enemy.heldItem == null)return;
            GrabbableObject itemGrabbableObject = enemy.heldItem.itemGrabbableObject;
            itemGrabbableObject.parentObject = null;
            itemGrabbableObject.transform.SetParent(StartOfRound.Instance.propsContainer, true);
            itemGrabbableObject.EnablePhysics(enable: true);
            itemGrabbableObject.fallTime = 0f;
            itemGrabbableObject.startFallingPosition = itemGrabbableObject.transform.parent.InverseTransformPoint(itemGrabbableObject.transform.position);
            itemGrabbableObject.targetFloorPosition = itemGrabbableObject.transform.parent.InverseTransformPoint(targetFloorPosition);
            itemGrabbableObject.floorYRot = -1;
            itemGrabbableObject.DiscardItemFromEnemy();
            enemy.heldItem = null;
            if (!droppingInNest) HoarderBugAI.grabbableObjectsInMap.Add(itemGrabbableObject.gameObject);
        }
    }
}
