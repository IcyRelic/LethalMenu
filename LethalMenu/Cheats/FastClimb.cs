using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats;

[HarmonyPatch]
internal class FastClimb : Cheat
{
    public override void Update()
    {
        if (!LethalMenu.LocalPlayer) return;
        if (Mathf.Approximately(Settings.f_defaultClimbSpeed, -1f))
            Settings.f_defaultClimbSpeed = LethalMenu.LocalPlayer.climbSpeed;

        LethalMenu.LocalPlayer.climbSpeed =
            Hack.FastClimb.IsEnabled() ? Settings.f_climbSpeed : Settings.f_defaultClimbSpeed;
    }
}