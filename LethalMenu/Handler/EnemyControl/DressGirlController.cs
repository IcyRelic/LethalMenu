using GameNetcodeStuff;
using UnityEngine;
using System.Linq;
using LethalMenu.Util;
using HarmonyLib;

namespace LethalMenu.Handler.EnemyControl
{
    internal class DressGirlController : IEnemyController<DressGirlAI>
    {
        public void UsePrimarySkill(DressGirlAI enemy)
        {
            if (enemy == null) return;
            PlayerControllerB player = GetClosestPlayer(enemy);
            if (player != null) enemy.hauntingPlayer = player;
            else return;
            if (enemy.hauntingPlayer != null && enemy.currentBehaviourStateIndex != 1)
            {
                enemy.EnableEnemyMesh(true, true);
                enemy.SwitchToBehaviourStateOnLocalClient(0);
                enemy.Reflect().Invoke("BeginChasing");
            }
            else if (enemy.hauntingPlayer != null && enemy.currentBehaviourStateIndex == 1) enemy.Reflect().Invoke("StopChasing");
        }

        public string GetPrimarySkillName(DressGirlAI enemy)
        {
            PlayerControllerB player = GetClosestPlayer(enemy);
            return player == null ? "" : (enemy.currentBehaviourStateIndex == 1 ? "Stop Chase" : "Begin Chase");
        }

        public void OnReleaseControl(DressGirlAI enemy)
        {
            if (enemy == null) return;
            if (enemy.currentBehaviourStateIndex == 1) enemy.Reflect().Invoke("StopChasing");
            enemy.Handle().Teleport(new Vector3(-1000, -1000));
            enemy.EnableEnemyMesh(false, true);
            enemy.hauntingPlayer = null;
        }

        public void OnDeath(DressGirlAI enemy)
        {
            if (enemy.currentBehaviourStateIndex == 1) enemy.Reflect().Invoke("StopChasing");
            enemy.Handle().Teleport(new Vector3(-1000, -1000));
            enemy.EnableEnemyMesh(false, true);
            enemy.hauntingPlayer = null;
        }

        public bool CanUseEntranceDoors(DressGirlAI _) => true;

        public float InteractRange(DressGirlAI _) => 5f;

        public static PlayerControllerB GetClosestPlayer(EnemyAI e) => LethalMenu.players.Where(p => p != null && !p.isPlayerDead).OrderBy(p => Vector3.Distance(e.transform.position, p.transform.position)).FirstOrDefault();
    }

    [HarmonyPatch]
    public class DressGirlAIPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DressGirlAI), "Update")]
        public static bool Update(DressGirlAI __instance)
        {
            if (!Cheats.EnemyControl.IsAIControlled)
            {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(DressGirlAI), "OnCollideWithPlayer")]
        public static bool OnCollideWithPlayer(DressGirlAI __instance, Collider other)
        {
            if (!Cheats.EnemyControl.IsAIControlled)
            {
                PlayerControllerB player = DressGirlController.GetClosestPlayer(__instance);
                if (player != null && __instance.hauntingPlayer != null && other.gameObject == __instance.hauntingPlayer.gameObject && __instance.currentBehaviourStateIndex == 1)
                {
                    player.KillPlayer(Vector3.zero, true, CauseOfDeath.Unknown, 1);
                    __instance.Reflect().Invoke("StopChasing");
                    __instance.EnableEnemyMesh(false, true);
                    __instance.creatureSFX.Stop();
                }
                return false;
            }
            return true;
        }
    }
}