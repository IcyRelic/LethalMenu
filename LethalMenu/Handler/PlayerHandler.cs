using System.Linq;
using GameNetcodeStuff;
using LethalMenu.Cheats;
using UnityEngine;

namespace LethalMenu.Handler;

public class PlayerHandler(PlayerControllerB player)
{
    public void TeleportAllItems()
    {
        LethalMenu.Items.FindAll(i => !i.isHeld && !i.isPocketed && !i.isInShipRoom).ForEach(i =>
        {
            var point = new Ray(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward)
                .GetPoint(1f);

            i.gameObject.transform.position = point;
            i.startFallingPosition = point;
            i.targetFloorPosition = point;
        });
    }

    public void Teleport(Vector3 pos, bool elevator = false, bool ship = false, bool factory = false)
    {
        player.TeleportPlayer(pos);
        player.isInElevator = elevator;
        player.isInHangarShipRoom = ship;
        player.isInsideFactory = factory;
        HUDManager.Instance.DisplayTip("Lethal Menu", "Teleported");
    }

    public void SavePosition()
    {
        Settings.v_savedLocation = player.transform.position;
        HUDManager.Instance.DisplayTip("Lethal Menu", "Teleport Position Saved");
    }

    public void TeleportShip()
    {
        if ((bool)StartOfRound.Instance)
            Teleport(StartOfRound.Instance.playerSpawnPositions[0].transform.position, false, true);
    }

    public void TeleportSaved()
    {
        if (Settings.v_savedLocation == Vector3.zero)
        {
            HUDManager.Instance.DisplayTip("Lethal Menu", "No Saved Position", true);
            return;
        }

        Teleport(Settings.v_savedLocation);
    }

    public void TeleportTo()
    {
        LethalMenu.LocalPlayer.Handle().Teleport(player.transform.position, player.isInElevator,
            player.isInHangarShipRoom, player.isInsideFactory);
    }

    public void Kill()
    {
        player.DamagePlayerFromOtherClientServerRpc(1000, new Vector3(0f, 0f, 0f), 0);
    }

    public void Heal()
    {
        player.DamagePlayerFromOtherClientServerRpc(-100, new Vector3(0f, 0f, 0f), 0);
    }

    public void Strike()
    {
        if ((bool)RoundManager.Instance) RoundManager.Instance.LightningStrikeServerRpc(player.transform.position);
    }

    public void TeleportEnemies(EnemyAI[] enemies = null)
    {
        enemies ??= LethalMenu.Enemies.ToArray();
        enemies.ToList().FindAll(e => !e.isEnemyDead).ForEach(e => e.Handle().Teleport(player));
    }

    public void Spectate()
    {
        SpectatePlayer.spectatingPlayer = (int)player.playerClientId;
    }

    public void MiniCam()
    {
        SpectatePlayer.camPlayer = (int)player.playerClientId;
    }

    public void SpawnSpiderWebs(int count = 1)
    {
        var spider = Object.FindObjectOfType(typeof(SandSpiderAI)) as SandSpiderAI;

        if (spider == null) return;

        for (var i = 0; i < count; i++) spider.SpawnWeb(player.transform.position);
    }

    public void ExplodeClosestLandmine()
    {
        var mine = LethalMenu.Landmines.OrderBy(m => Vector3.Distance(m.transform.position, player.transform.position))
            .FirstOrDefault();
        mine?.ExplodeMineServerRpc();
    }

    public void LureAllEnemies()
    {
        LethalMenu.Enemies.FindAll(e => !e.isEnemyDead).ForEach(e => e.Handle().TargetPlayer(player));
    }

    public void PlaceEverythingOnDesk()
    {
        var desk = Object.FindObjectOfType<DepositItemsDesk>();
        if (!desk) return;

        player.DropAllHeldItems();
        LethalMenu.Items.FindAll(i => i.itemProperties.isScrap && !i.isHeld && !i.isPocketed).ForEach(i =>
        {
            player.currentlyHeldObjectServer = i;
            desk.PlaceItemOnCounter(player);
        });
    }

    public PlayerHandler GetHandler(PlayerControllerB handler)
    {
        return new PlayerHandler(handler);
    }
}

public static class PlayerHandlerExtensions
{
    public static PlayerHandler Handle(this PlayerControllerB player)
    {
        return new PlayerHandler(player);
    }
}