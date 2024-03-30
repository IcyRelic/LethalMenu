using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats;

internal class Invisibility : Cheat
{
    public override void Update()
    {
        if (LethalMenu.localPlayer == null || !Hack.Invisibility.IsEnabled()) return;

        var pos = StartOfRound.Instance.shipHasLanded
            ? StartOfRound.Instance.notSpawnedPosition.position
            : Vector3.zero;

        LethalMenu.localPlayer.Reflect().Invoke("UpdatePlayerPositionServerRpc", pos, true, false, false, true);
    }
}