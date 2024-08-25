using GameNetcodeStuff;
using System.Linq;
using UnityEngine;
using LethalMenu.Util;

namespace LethalMenu.Handler
{
    public class PlayerHandler
    {
        private PlayerControllerB player;

        public PlayerHandler(PlayerControllerB player)
        {
            this.player = player;
        }

        public void TeleportAllItems()
        {
            LethalMenu.items.FindAll(i => !i.isHeld && !i.isPocketed && !i.isInShipRoom).ForEach(i =>
            {
                Vector3 point = new Ray(player.gameplayCamera.transform.position, player.gameplayCamera.transform.forward).GetPoint(1f);

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
            if ((bool)StartOfRound.Instance) Teleport(StartOfRound.Instance.playerSpawnPositions[0].transform.position, false, true, false);
        }

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

        public void Kill() => player.DamagePlayerFromOtherClientServerRpc(1000, new Vector3(0f, 0f, 0f), 0);

        public void Heal() => player.DamagePlayerFromOtherClientServerRpc(-100, new Vector3(0f, 0f, 0f), 0);

        public void Strike()
        {
            if ((bool)RoundManager.Instance) RoundManager.Instance.LightningStrikeServerRpc(player.transform.position);
        }

        public void Spectate() => Cheats.SpectatePlayer.spectatingPlayer = (int)player.playerClientId;
        public void MiniCam() => Cheats.SpectatePlayer.camPlayer = (int)player.playerClientId;

        public void SpawnSpiderWebs(int count = 1)
        {
            SandSpiderAI spider = Object.FindObjectOfType(typeof(SandSpiderAI)) as SandSpiderAI;

            if (spider == null) return;

            for (int i = 0; i < count; i++)
            {
                spider.SpawnWeb(player.transform.position);
            }

        }

        public void ExplodeClosestLandmine()
        {
            Landmine mine = LethalMenu.landmines.OrderBy(m => Vector3.Distance(m.transform.position, player.transform.position)).FirstOrDefault();
            mine.ExplodeMineServerRpc();
        }

        public void ForceBleed()
        {
            if (Hack.ForceBleed.IsEnabled())
            {
                player.hasBeenCriticallyInjured = false;
                player.criticallyInjured = false;
                player.playerBodyAnimator.SetBool("Limp", false);
                if (LethalMenu.localPlayer.IsHost)
                {
                    player.MakeCriticallyInjuredServerRpc();
                    return;
                }
                player.MakeCriticallyInjured(true);
            }
            else
            {
                player.hasBeenCriticallyInjured = false;
                if (LethalMenu.localPlayer.IsHost)
                {
                    player.HealServerRpc();
                    return;
                }
                player.MakeCriticallyInjured(false);
            }
        }

        public void LureAllEnemies() => LethalMenu.enemies.FindAll(e => !e.isEnemyDead).ForEach(e => e.Handle().TargetPlayer(player));

        public void PlaceEverythingOnDesk()
        {
            DepositItemsDesk desk = Object.FindObjectOfType<DepositItemsDesk>();
            if (desk == null) return;

            player.DropAllHeldItems();
            LethalMenu.items.FindAll(i => i.itemProperties.isScrap && !i.isHeld && !i.isPocketed).ForEach(i =>
            {
                player.currentlyHeldObjectServer = i;
                desk.PlaceItemOnCounter(player);
            });
        }

        public PlayerHandler GetHandler(PlayerControllerB player) => new PlayerHandler(player);
    }



    public static class PlayerHandlerExtensions
    {
        public static PlayerHandler Handle(this PlayerControllerB player) => new PlayerHandler(player);
    }
}
