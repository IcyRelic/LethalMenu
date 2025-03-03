using GameNetcodeStuff;
using LethalMenu.Cheats;
using LethalMenu.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LethalMenu.Util
{
    public static class GameUtil
    {
        public static List<EnemyType> GetEnemyTypes()
        {
            List<EnemyType> types = new List<EnemyType>();

            if (!(bool)StartOfRound.Instance) return types;

            if (LethalMenu.quickMenuManager != null) types.AddRange(LethalMenu.quickMenuManager.testAllEnemiesLevel.OutsideEnemies.Where(e => e.enemyType.enemyName == "Bush Wolf").Select(e => e.enemyType));

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

            if (StartOfRound.Instance == null || StartOfRound.Instance.levels == null) return types;

            foreach (var level in StartOfRound.Instance.levels)
            {
                if (level.spawnableMapObjects == null) continue;
                foreach (var spawnableObject in level.spawnableMapObjects)
                {
                    if (!types.Any(x => x.prefabToSpawn.name == spawnableObject.prefabToSpawn.name))
                    {
                        types.Add(spawnableObject);
                    }
                }
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
            return (float)Mathf.Round(Vector3.Distance(CameraManager.ActiveCamera.transform.position, position));
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

        public static RaycastHit StraightCastForward(this Transform transform, float distance = 1000f)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, transform.forward, out hit, distance);
            return hit;
        }

        public static void RenderPlayerModels()
        {
            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;

            if (Hack.SpectatePlayer.IsEnabled() || Hack.FreeCam.IsEnabled())
            {
                localPlayer.DisablePlayerModel(localPlayer.gameObject, true);
                localPlayer.thisPlayerModelArms.enabled = false;
            }
            else
            {
                localPlayer.DisablePlayerModel(localPlayer.gameObject);
                localPlayer.thisPlayerModelArms.enabled = true;
            }

            foreach (PlayerControllerB player in localPlayer.playersManager.allPlayerScripts)
            {
                if (localPlayer.playerClientId == player.playerClientId) continue;

                if ((SpectatePlayer.isSpectatingPlayer(player) || SpectatePlayer.isCamPlayer(player)) && Settings.b_disableSpectatorModels)
                {
                    player.DisablePlayerModel(player.gameObject);
                    player.thisPlayerModelArms.enabled = true;
                }
                else
                {
                    player.DisablePlayerModel(player.gameObject, true, true);
                    player.thisPlayerModelArms.enabled = false;
                }
            }
        }
    }
}
