using GameNetcodeStuff;
using LethalMenu.Cheats;
using LethalMenu.Manager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.HID;

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

        public void Heal() => player.DamagePlayerClientRpc(0, 100);

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
            List<GrabbableObject> items = LethalMenu.items.Where(i => i != null && i.isInShipRoom && !i.isHeld && !i.isPocketed && i.itemProperties.isScrap && !InfoDisplay.DefaultShipItem(i)).ToList();
            items.ForEach(i =>        
            {
                if (quotaLeft < 0) return;
                Patches.SellQuota = true;
                quotaLeft -= (int)(i.scrapValue * StartOfRound.Instance.companyBuyingRate);
                PlaceOnDesk(i);
            });
        }

        public void PlaceEverythingOnDesk()
        {
            player.DropAllHeldItems();
            LethalMenu.items.FindAll(i => i.itemProperties.isScrap && !i.isHeld && !i.isPocketed && i.itemProperties.isScrap && !InfoDisplay.DefaultShipItem(i)).ForEach(PlaceOnDesk);
        }

        public void PlaceOnDesk(GrabbableObject item)
        {
            DepositItemsDesk desk = Object.FindObjectOfType<DepositItemsDesk>();
            if (desk == null) return;
            player.currentlyHeldObjectServer = item;
            desk.PlaceItemOnCounter(player);
        }

        public bool HasLineOfSightToPosition(Component o) => player != null && o != null && player.HasLineOfSightToPosition(o.transform.position);
        public bool HasLineOfSight(Component o) => player != null && o != null && player.HasLineOfSight(o.transform.position);
        public PlayerHandler GetHandler(PlayerControllerB player) => new PlayerHandler(player);
    }



    public static class PlayerHandlerExtensions
    {
        public static PlayerHandler Handle(this PlayerControllerB player) => new PlayerHandler(player);
        public static bool HasLineOfSight(this PlayerControllerB player, Vector3 pos, float width = 45f, int range = 60)
        {
            if (Vector3.Distance(player.transform.position, pos) < (float)range && Vector3.Angle(player.playerEye.transform.forward, pos - CameraManager.ActiveCamera.transform.position) < width && !Physics.Linecast(player.playerEye.transform.position, pos, out RaycastHit hit, StartOfRound.Instance.collidersRoomDefaultAndFoliage, QueryTriggerInteraction.Ignore)) return true;
            return false;
        }
    }
}
