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
        void UseHeldItem(HoarderBugAI enemy)
        {
            if (enemy.heldItem is not { itemGrabbableObject: GrabbableObject grabbable }) return;

            switch (grabbable)
            {
                case ShotgunItem gun:
                    gun.ShootGunAsEnemy(enemy);
                    break;

                default:
                    break;
            }
        }

        public void Update(HoarderBugAI enemy, bool isAIControlled)
        {
            if (isAIControlled) return;
            if (enemy.heldItem?.itemGrabbableObject is null) return;

            enemy.angryTimer = 0.0f;
            enemy.SetBehaviourState(BugState.IDLE);
        }

        void GrabItem(HoarderBugAI enemy, GrabbableObject item)
        {
            Debug.Log("Grabbing item");
            if (!item.TryGetComponent(out NetworkObject netItem)) return;

            Debug.Log("Item is networked");
            Debug.Log(netItem.NetworkObjectId);
            Debug.Log(enemy.enemyType);
            Debug.Log(item.itemProperties.itemName);


            Debug.Log("Calling GrabItem");

            ReflectionUtil<HoarderBugAI> reflect = enemy.Reflect();

            Debug.Log(reflect is null ? "Reflect is null" : "Reflect is not null");


            reflect.Invoke("GrabItem", netItem);
            Debug.Log("Setting sendingGrabOrDropRPC");
            reflect.SetValue("sendingGrabOrDropRPC", true);

            Debug.Log("Switching to behaviour server rpc");
            enemy.SwitchToBehaviourServerRpc(1);

            Debug.Log("Grabbing item server rpc");
            enemy.GrabItemServerRpc(netItem);
        }

        public void OnDeath(HoarderBugAI enemy)
        {
            if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject)) return;

            _ = enemy.Reflect().Invoke(
                "DropItemAndCallDropRPC",
                networkObject,
                false
            );
        }

        public void UsePrimarySkill(HoarderBugAI enemy)
        {
            if (enemy.angryTimer > 0.0f)
            {
                enemy.angryTimer = 0.0f;
                enemy.angryAtPlayer = null;
                enemy.SetBehaviourState(BugState.IDLE);
            }

            if (enemy.heldItem is null && enemy.FindNearbyItem() is GrabbableObject grabbable)
            {
                this.GrabItem(enemy, grabbable);
            }

            else
            {
                this.UseHeldItem(enemy);
            }
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

            if (enemy.heldItem.itemGrabbableObject.TryGetComponent(out NetworkObject networkObject))
            {
                enemy.Reflect().Invoke("DropItemAndCallDropRPC", args: [networkObject, false]);
            }
        }

        public string GetPrimarySkillName(HoarderBugAI enemy) => enemy.heldItem is not null ? "Use item" : "Grab Item";

        public string GetSecondarySkillName(HoarderBugAI enemy) => enemy.heldItem is null ? "" : "Drop item";

        public float InteractRange(HoarderBugAI _) => 1.0f;
    }
}
