using GameNetcodeStuff;
using LethalMenu.Cheats;
using LethalMenu.Manager;
using LethalMenu.Util;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using Vector3 = UnityEngine.Vector3;

namespace LethalMenu.Handler
{
    public class PlayerHandler
    {  
        private PlayerControllerB player;

        public PlayerHandler(PlayerControllerB player)
        {
            this.player = player;
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

        public void TeleportShip() => Teleport((bool)StartOfRound.Instance ? StartOfRound.Instance.playerSpawnPositions[0].transform.position : Vector3.zero, false, true, false);

        public void TeleportSaved()
        {
            if(Settings.v_savedLocation == Vector3.zero)
            {
                HUDManager.Instance.DisplayTip("Lethal Menu", "No Saved Position", true);
                return;
            }
            Teleport(Settings.v_savedLocation, false, false, false);
        }

        public void TeleportTo() => LethalMenu.localPlayer.Handle().Teleport(player.transform.position, player.isInElevator, player.isInHangarShipRoom, player.isInsideFactory);

        public void Kill() => player.DamagePlayerFromOtherClientServerRpc(player.health, new Vector3(0f, 0f, 0f), -1);

        public void Heal()
        {
            if (LethalMenu.localPlayer.IsHost()) player.DamagePlayerServerRpc(0, 100);
            else player.DamagePlayerFromOtherClientServerRpc(-100, new Vector3(0f, 0f, 0f), -1);
        }

        public void Strike() => RoundManager.Instance?.LightningStrikeServerRpc(player.transform.position);

        public void Spectate() => Cheats.SpectatePlayer.spectatingPlayer = (int)player.playerClientId;
        public void MiniCam() => Cheats.SpectatePlayer.camPlayer = (int)player.playerClientId;

        public void SpawnSpiderWebs(int count = 1)
        {
            SandSpiderAI spider = Object.FindObjectOfType(typeof(SandSpiderAI)) as SandSpiderAI;
            if (spider == null) return;
            for (int i = 0; i < count; i++) spider.SpawnWeb(player.transform.position);
        }

        public void ExplodeClosestLandmine()
        {
            Landmine mine = LethalMenu.landmines.OrderBy(m => Vector3.Distance(m.transform.position, player.transform.position)).FirstOrDefault();
            mine.ExplodeMineServerRpc();
        }

        public void LureAllEnemies() => LethalMenu.enemies.FindAll(e => !e.isEnemyDead).ForEach(e => e.Handle().TargetPlayer(player));

        public void SellQuota()
        {
            int quotaLeft = TimeOfDay.Instance.profitQuota - TimeOfDay.Instance.quotaFulfilled;
            List<GrabbableObject> items = LethalMenu.items.Where(i => i != null && i.isInShipRoom && !i.isHeld && !i.isPocketed && i.itemProperties.isScrap && !i.IsDefaultShipItem()).ToList();
            items.ForEach(i =>        
            {
                if (quotaLeft < 0) return;
                Patches.SellQuota = true;
                quotaLeft -= (int)(i.scrapValue * StartOfRound.Instance.companyBuyingRate);
                i.PlaceOnDesk();
            });
        }

        public void PlaceEverythingOnDesk()
        {
            player.DropAllHeldItems();
            LethalMenu.items.Where(i => i.itemProperties.isScrap && !i.isHeld && !i.isPocketed && i.itemProperties.isScrap && !i.IsDefaultShipItem()).ToList().ForEach(i =>
            {
                i.PlaceOnDesk();
            });
        }

        public bool HasLineOfSight(Component o) => player != null && o != null && player.HasLineOfSightToPosition(o.transform.position);
        public PlayerHandler GetHandler(PlayerControllerB player) => new PlayerHandler(player);
    }



    public static class PlayerHandlerExtensions
    {
        public static PlayerHandler Handle(this PlayerControllerB player) => new PlayerHandler(player);

        public static bool IsRealPlayer(this PlayerControllerB player)
        {
            PlayerControllerB host = LethalMenu.players.FirstOrDefault(p => p != null && p.IsHost());
            if (Vector3.Distance(player.serverPlayerPosition, StartOfRound.Instance.notSpawnedPosition.position) <= 10f || host != null && host != player && player.playerSteamId == host.playerSteamId || player.playerSteamId == 0) return false;
            return true;
        }

        public static bool IsHost(this PlayerControllerB player)
        {
            return player != null && player.IsHost || player.actualClientId == 0;
        }
    }
}
