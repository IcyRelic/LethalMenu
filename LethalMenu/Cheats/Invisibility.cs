using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats;

internal class Invisibility : Cheat
{
    public override void Update()
    {
        if (!LethalMenu.LocalPlayer || !Hack.Invisibility.IsEnabled()) return;

        var position = StartOfRound.Instance.shipHasLanded
            ? StartOfRound.Instance.notSpawnedPosition.position
            : Vector3.zero;

        LethalMenu.LocalPlayer.Reflect().Invoke("UpdatePlayerPositionServerRpc", position, true, false, false, true);
    }
}