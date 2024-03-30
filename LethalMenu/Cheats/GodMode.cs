using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats;

[HarmonyPatch]
internal class GodMode : Cheat
{
    public override void Update()
    {
        if (!Hack.GodMode.IsEnabled()) return;

        var player = GameNetworkManager.Instance.localPlayerController;
        if (!player) return;
        player.health = 100;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer))]
    public static bool PrefixDamagePlayer(int damageNumber, bool hasDamageSfx = true, bool callRPC = true,
        CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, int deathAnimation = 0, bool fallDamage = false,
        Vector3 force = default)
    {
        return !Hack.GodMode.IsEnabled();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayer))]
    public static bool PrefixKillPlayer(Vector3 bodyVelocity, bool spawnBody = true,
        CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, int deathAnimation = 0)
    {
        return !Hack.GodMode.IsEnabled();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationServerRpc))]
    public static bool PrefixFlowermanKill(int playerObjectId)
    {
        if (!LethalMenu.LocalPlayer || playerObjectId != (int)LethalMenu.LocalPlayer.playerClientId) return true;


        return !Hack.GodMode.IsEnabled();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerServerRpc))]
    public static bool PrefixGiantKill(int playerId)
    {
        if (!LethalMenu.LocalPlayer || playerId != (int)LethalMenu.LocalPlayer.playerClientId) return true;


        return !Hack.GodMode.IsEnabled();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.KillPlayerServerRpc))]
    public static bool PrefixJesterKill(int playerId)
    {
        if (!LethalMenu.LocalPlayer || playerId != (int)LethalMenu.LocalPlayer.playerClientId) return true;


        return !Hack.GodMode.IsEnabled();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.KillPlayerAnimationServerRpc))]
    public static bool PrefixMaskedPlayerKill(int playerObjectId)
    {
        if (!LethalMenu.LocalPlayer || playerObjectId != (int)LethalMenu.LocalPlayer.playerClientId) return true;


        return !Hack.GodMode.IsEnabled();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.OnCollideWithPlayer))]
    public static bool PrefixDogKill(MouthDogAI __instance, Collider other)
    {
        var player = __instance.MeetsStandardPlayerCollisionConditions(other);

        if (!LethalMenu.LocalPlayer ||
            player.playerClientId != LethalMenu.LocalPlayer.playerClientId) return true;

        return !Hack.GodMode.IsEnabled();
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(CentipedeAI), nameof(CentipedeAI.OnCollideWithPlayer))]
    public static bool PrefixCentipedeCling(CentipedeAI __instance, Collider other)
    {
        if (!other) return true;

        var player = __instance.MeetsStandardPlayerCollisionConditions(other);

        if (!LethalMenu.LocalPlayer ||
            player.playerClientId != LethalMenu.LocalPlayer.playerClientId) return true;


        return !Hack.GodMode.IsEnabled();
    }
}