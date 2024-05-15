using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class GodMode : Cheat
    {
        public override void Update()
        {
            if(!Hack.GodMode.IsEnabled()) return;
            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
            if (player == null) return;
            player.health = 100;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer))]
        public static bool PrefixDamagePlayer(int damageNumber, bool hasDamageSFX = true, bool callRPC = true, CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, int deathAnimation = 0, bool fallDamage = false, Vector3 force = default(Vector3))
        {    
            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayer))]
        public static bool PrefixKillPlayer(Vector3 bodyVelocity, bool spawnBody = true, CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, int deathAnimation = 0)
        {
            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationServerRpc))]
        public static bool PrefixFlowermanKill(int playerObjectId)
        {
            if (LethalMenu.localPlayer == null || playerObjectId != (int) LethalMenu.localPlayer.playerClientId) return true;

            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerServerRpc))]
        public static bool PrefixGiantKill(int playerId)
        {
            if (LethalMenu.localPlayer == null || playerId != (int)LethalMenu.localPlayer.playerClientId) return true;

            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.KillPlayerServerRpc))]
        public static bool PrefixJesterKill(int playerId)
        {
            if (LethalMenu.localPlayer == null || playerId != (int)LethalMenu.localPlayer.playerClientId) return true;

            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.KillPlayerAnimationServerRpc))]
        public static bool PrefixMaskedPlayerKill(int playerObjectId)
        {
            if (LethalMenu.localPlayer == null || playerObjectId != (int)LethalMenu.localPlayer.playerClientId) return true;

            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.OnCollideWithPlayer))]
        public static bool PrefixDogKill(MouthDogAI __instance, Collider other)
        {
            PlayerControllerB player = __instance.MeetsStandardPlayerCollisionConditions(other);

            if (LethalMenu.localPlayer == null || player.playerClientId != LethalMenu.localPlayer.playerClientId) return true;

            return !Hack.GodMode.IsEnabled();
        }

        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(CentipedeAI), nameof(CentipedeAI.OnCollideWithPlayer))]
        public static bool PrefixCentipedeCling(CentipedeAI __instance, Collider other)
        {
            if(other == null) return true;

            PlayerControllerB player = __instance.MeetsStandardPlayerCollisionConditions(other);

            if (LethalMenu.localPlayer == null || player.playerClientId != LethalMenu.localPlayer.playerClientId) return true;

            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(RadMechAI), nameof(RadMechAI.OnCollideWithPlayer))]
        public static bool PrefixRadMechKill(RadMechAI __instance, Collider other)
        {
            PlayerControllerB player = __instance.MeetsStandardPlayerCollisionConditions(other);

            if (LethalMenu.localPlayer == null || player.playerClientId != LethalMenu.localPlayer.playerClientId) return true;

            return !Hack.GodMode.IsEnabled();
        }
    }
}
