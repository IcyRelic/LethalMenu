using UnityEngine;
using Random = UnityEngine.Random;

namespace LethalMenu.Handler.EnemyControl;

internal class SandSpiderController : IEnemyController<SandSpiderAI>
{
    public void OnMovement(SandSpiderAI enemy, bool isMoving, bool isSprinting)
    {
        enemy.creatureAnimator.SetBool("moving", true);
        // spider is too slow, make it like 6f default, 8f sprinting
        var speed = isSprinting ? 8.0f : 6.0f;
        enemy.agent.speed = speed;
        enemy.spiderSpeed = speed;
        enemy.SyncMeshContainerPositionToClients();
    }

    public void UsePrimarySkill(SandSpiderAI enemy)
    {
        PlaceWebTrap(enemy);
    }

    private void PlaceWebTrap(SandSpiderAI enemy)
    {
        if (!(bool)StartOfRound.Instance) return;

        var randomDirection = Random.onUnitSphere;
        randomDirection.y = Mathf.Min(0.0f, randomDirection.y * Random.Range(0.5f, 1.0f));

        Ray ray = new(enemy.abdomen.position + Vector3.up * 0.4f, randomDirection);

        if (!Physics.Raycast(ray, out var raycastHit, 7.0f, StartOfRound.Instance.collidersAndRoomMask)) return;

        if (raycastHit.distance < 2.0f) return;

        if (!Physics.Raycast(enemy.abdomen.position, Vector3.down, out var groundHit, 10.0f,
                StartOfRound.Instance.collidersAndRoomMask)) return;

        var floorPosition = groundHit.point + Vector3.up * 0.2f;
        enemy.SpawnWebTrapServerRpc(floorPosition, raycastHit.point);
    }
}