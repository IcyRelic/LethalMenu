using GameNetcodeStuff;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class Invisibility : Cheat
    {

        public override void Update()
        {
            PlayerControllerB? localPlayer = LethalMenu.localPlayer;
            if (!Hack.Invisibility.IsEnabled() || localPlayer == null) return;
            Vector3 position = StartOfRound.Instance.shipHasLanded ? StartOfRound.Instance.notSpawnedPosition.position : Vector3.zero;
            LethalMenu.localPlayer.Reflect().Invoke("UpdatePlayerPositionRpc", position, localPlayer.isInElevator, localPlayer.isInHangarShipRoom, localPlayer.isExhausted, localPlayer.thisController.isGrounded);
        }
    }
}
