using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LethalMenu.Util
{
    public static class GameUtil
    {

        public static List<EnemyType> GetEnemyTypes()
        {
            List<EnemyType> types = new List<EnemyType>();

            if (!(bool)StartOfRound.Instance) return types;

            foreach (var item in StartOfRound.Instance.levels)
            {
                item.Enemies.ForEach(enemy => { if (!types.Contains(enemy.enemyType)) types.Add(enemy.enemyType); });
                item.DaytimeEnemies.ForEach(enemy => { if (!types.Contains(enemy.enemyType)) types.Add(enemy.enemyType); });
                item.OutsideEnemies.ForEach(enemy => { if (!types.Contains(enemy.enemyType)) types.Add(enemy.enemyType); });
            }

            return types;
        }

        public static List<SpawnableMapObject> GetSpawnableMapObjects()
        {
            List<SpawnableMapObject> types = new List<SpawnableMapObject>();

            if (!(bool)StartOfRound.Instance) return types;

            foreach (var level in StartOfRound.Instance.levels)
            {
                level.spawnableMapObjects.ToList().ForEach(o => { if (!types.Any(x => x.prefabToSpawn.name == o.prefabToSpawn.name)) types.Add(o); });
            }

            return types;
        }

        public static void InitializeMapObjects()
        {
            if (!(bool)StartOfRound.Instance) return;

            LethalMenu.turrets.ForEach(t => t.GetComponent<TerminalAccessibleObject>().InitializeValues());
            LethalMenu.landmines.ForEach(l => l.GetComponent<TerminalAccessibleObject>().InitializeValues());
        }

        public static float GetDistanceToPlayer(Vector3 position)
        {
            return (float)Mathf.Round(Vector3.Distance(GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.position, position));
        }

        public static float GetDistance(Vector3 from, Vector3 to)
        {
            return (float)Mathf.Round(Vector3.Distance(from, to));
        }

        public static int SphereCastForward(this RaycastHit[] array, Transform transform, float sphereRadius = 1.0f)
        {
            try
            {
                return Physics.SphereCastNonAlloc(
                    transform.position + (transform.forward * (sphereRadius + 1.75f)),
                    sphereRadius,
                    transform.forward,
                    array,
                    float.MaxValue
                );
            }

            catch (NullReferenceException)
            {
                return 0;
            }
        }

        public static RaycastHit[] SphereCastForward(this Transform transform, float sphereRadius = 1.0f)
        {
            try
            {
                return Physics.SphereCastAll(
                    transform.position + (transform.forward * (sphereRadius + 1.75f)),
                    sphereRadius,
                    transform.forward,
                    float.MaxValue
                );
            }

            catch (NullReferenceException)
            {
                return null;
            }
        }

    }
}
