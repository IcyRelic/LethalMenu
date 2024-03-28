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
            if (enemy.heldItem == null || enemy.heldItem.itemGrabbableObject == null) return;

            if (enemy.heldItem.itemGrabbableObject is ShotgunItem gun)
            {
                gun.ShootGunAsEnemy(enemy);
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
            ReflectionUtil<HoarderBugAI> reflect = enemy.Reflect();
            NetworkObject netItem = item.GetComponent<NetworkObject>();
            if (enemy == null || item == null || netItem == null || reflect is null)
            {
                return;
            }

            enemy.SwitchToBehaviourServerRpc(1);

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
            if (enemy == null) return;

            if (enemy.angryTimer > 0.0f)
            {
                enemy.angryTimer = 0.0f;
                enemy.angryAtPlayer = null;
                enemy.SetBehaviourState(BugState.IDLE);
            }

            GrabbableObject nearbyItem = enemy.FindNearbyItem();
            if (nearbyItem != null)
            {
                GrabItem(enemy, nearbyItem);
            }
            else
            {
                UseHeldItem(enemy);
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

        public float InteractRange(HoarderBugAI _) => 1.5f;
    }
}
