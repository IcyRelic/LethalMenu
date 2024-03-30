using System;
using System.Collections.Generic;
using System.Linq;
using LethalMenu.Cheats;
using UnityEngine;

namespace LethalMenu.Util;

public static class GameUtil
{
    public static List<EnemyType> GetEnemyTypes()
    {
        var types = new List<EnemyType>();

        if (!(bool)StartOfRound.Instance) return types;

        foreach (var item in StartOfRound.Instance.levels)
        {
            item.Enemies.ForEach(enemy =>
            {
                if (!types.Contains(enemy.enemyType)) types.Add(enemy.enemyType);
            });
            item.DaytimeEnemies.ForEach(enemy =>
            {
                if (!types.Contains(enemy.enemyType)) types.Add(enemy.enemyType);
            });
            item.OutsideEnemies.ForEach(enemy =>
            {
                if (!types.Contains(enemy.enemyType)) types.Add(enemy.enemyType);
            });
        }

        return types;
    }

    public static IEnumerable<SpawnableMapObject> GetSpawnableMapObjects()
    {
        var types = new List<SpawnableMapObject>();

        if (!(bool)StartOfRound.Instance) return types;

        foreach (var level in StartOfRound.Instance.levels)
            level.spawnableMapObjects.ToList().ForEach(o =>
            {
                if (types.All(x => x.prefabToSpawn.name != o.prefabToSpawn.name)) types.Add(o);
            });

        return types;
    }

    public static void InitializeMapObjects()
    {
        if (!(bool)StartOfRound.Instance) return;

        LethalMenu.Turrets.ForEach(t => t.GetComponent<TerminalAccessibleObject>().InitializeValues());
        LethalMenu.Landmines.ForEach(l => l.GetComponent<TerminalAccessibleObject>().InitializeValues());
    }

    public static float GetDistanceToPlayer(Vector3 position)
    {
        return Mathf.Round(Vector3.Distance(
            GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.position, position));
    }

    public static float GetDistance(Vector3 from, Vector3 to)
    {
        return Mathf.Round(Vector3.Distance(from, to));
    }

    public static int SphereCastForward(this RaycastHit[] array, Transform transform, float sphereRadius = 1.0f)
    {
        try
        {
            return Physics.SphereCastNonAlloc(
                transform.position + transform.forward * (sphereRadius + 1.75f),
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
                transform.position + transform.forward * (sphereRadius + 1.75f),
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

    public static void RenderPlayerModels()
    {
        var localPlayer = GameNetworkManager.Instance.localPlayerController;

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

        foreach (var player in localPlayer.playersManager.allPlayerScripts)
        {
            if (localPlayer.playerClientId == player.playerClientId) continue;

            if ((SpectatePlayer.isSpectatingPlayer(player) || SpectatePlayer.isCamPlayer(player)) &&
                Settings.b_disableSpectatorModels)
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