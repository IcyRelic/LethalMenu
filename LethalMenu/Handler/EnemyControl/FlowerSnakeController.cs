using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Handler.EnemyControl
{
    internal class FlowerSnakeController : IEnemyController<FlowerSnakeEnemy>
    {
        public void UsePrimarySkill(FlowerSnakeEnemy enemy)
        {
            if (!enemy.Reflect().GetValue<bool>("leaping")) LeapForward(enemy);
            else if (ClingingToPlayer(enemy) || enemy.activatedFlight) enemy.StopClingingServerRpc((int)enemy.clingingToPlayer.actualClientId);
        }

        public void UseSecondarySkill(FlowerSnakeEnemy enemy)
        {
            if (!ClingingToPlayer(enemy) || !enemy.activatedFlight) enemy.Reflect().Invoke("StartLiftingClungPlayer");
            else if (ClingingToPlayer(enemy) || enemy.activatedFlight) enemy.StopClingingServerRpc((int)enemy.clingingToPlayer.actualClientId);
        }

        public bool ClingingToPlayer(FlowerSnakeEnemy enemy)
        {
            return enemy.clingingToPlayer;
        }

        public string GetPrimarySkillName(FlowerSnakeEnemy _)
        {
            return ClingingToPlayer(_) ? "Stop clinging" : "Leap";
        }

        public string GetSecondarySkillName(FlowerSnakeEnemy _) => "Fly";

        public bool CanUseEntranceDoors(FlowerSnakeEnemy _) => true;

        public float InteractRange(FlowerSnakeEnemy _) => 5f;

        private void LeapForward(FlowerSnakeEnemy enemy)
        {
            enemy.StartLeapOnLocalClient((enemy.transform.forward * 2f));
            enemy.StartLeapClientRpc((enemy.transform.forward * 2f));
        }
    }
}
