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
            if (!Hack.GodMode.IsEnabled() || LethalMenu.localPlayer == null) return;
            LethalMenu.localPlayer.health = 100;
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.DamagePlayer)), HarmonyPrefix]
        public static bool DamagePlayer()
        {    
            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPatch(typeof(PlayerControllerB), nameof(PlayerControllerB.KillPlayer)), HarmonyPrefix]
        public static bool KillPlayer()
        {
            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPatch(typeof(FlowermanAI), nameof(FlowermanAI.KillPlayerAnimationServerRpc)), HarmonyPrefix]
        public static bool FlowermanAIKillPlayerAnimationServerRpc(int playerObjectId)
        {
            if (LethalMenu.localPlayer == null || playerObjectId != (int)LethalMenu.localPlayer.playerClientId) return true;
            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPatch(typeof(ForestGiantAI), nameof(ForestGiantAI.GrabPlayerServerRpc)), HarmonyPrefix]
        public static bool ForestGiantAIGrabPlayerServerRpc(int playerId)
        {
            if (LethalMenu.localPlayer == null || playerId != (int)LethalMenu.localPlayer.playerClientId) return true;
            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPatch(typeof(JesterAI), nameof(JesterAI.KillPlayerServerRpc)), HarmonyPrefix]
        public static bool JesterAIKillPlayerServerRpc(int playerId)
        {
            if (LethalMenu.localPlayer == null || playerId != (int)LethalMenu.localPlayer.playerClientId) return true;
            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPatch(typeof(MaskedPlayerEnemy), nameof(MaskedPlayerEnemy.KillPlayerAnimationServerRpc)), HarmonyPrefix]
        public static bool MaskedPlayerEnemyKillPlayerAnimationServerRpc(int playerObjectId)
        {
            if (LethalMenu.localPlayer == null || playerObjectId != (int)LethalMenu.localPlayer.playerClientId) return true;

            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPatch(typeof(MouthDogAI), nameof(MouthDogAI.OnCollideWithPlayer)), HarmonyPrefix]
        public static bool MouthDogAIOnCollideWithPlayer(MouthDogAI __instance, Collider other)
        {
            PlayerControllerB player = __instance.MeetsStandardPlayerCollisionConditions(other);
            if (player == null || LethalMenu.localPlayer == null || player.playerClientId != LethalMenu.localPlayer.playerClientId) return true;
            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPatch(typeof(CentipedeAI), nameof(CentipedeAI.OnCollideWithPlayer)), HarmonyPrefix]
        public static bool CentipedeAIOnCollideWithPlayer(CentipedeAI __instance, Collider other)
        {
            PlayerControllerB player = __instance.MeetsStandardPlayerCollisionConditions(other);
            if (player == null || LethalMenu.localPlayer == null || player.playerClientId != LethalMenu.localPlayer.playerClientId) return true;
            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPatch(typeof(RadMechAI), nameof(RadMechAI.OnCollideWithPlayer)), HarmonyPrefix]
        public static bool RadMechAIOnCollideWithPlayer(RadMechAI __instance, Collider other)
        {
            PlayerControllerB player = __instance.MeetsStandardPlayerCollisionConditions(other);
            if (player == null || LethalMenu.localPlayer == null || player.playerClientId != LethalMenu.localPlayer.playerClientId) return true;
            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPatch(typeof(BushWolfEnemy), nameof(BushWolfEnemy.OnCollideWithPlayer)), HarmonyPrefix]
        public static bool BushWolfEnemyOnCollideWithPlayer(BushWolfEnemy __instance, Collider other)
        {
            PlayerControllerB player = __instance.MeetsStandardPlayerCollisionConditions(other);
            if (player == null || LethalMenu.localPlayer == null || player.playerClientId != LethalMenu.localPlayer.playerClientId) return true;
            return !Hack.GodMode.IsEnabled();
        }

        [HarmonyPatch(typeof(CaveDwellerAI), nameof(CaveDwellerAI.OnCollideWithPlayer)), HarmonyPrefix]
        public static bool CaveDwellerAIOnCollideWithPlayer(CaveDwellerAI __instance, Collider other)
        {
            PlayerControllerB player = __instance.MeetsStandardPlayerCollisionConditions(other);
            if (player == null || LethalMenu.localPlayer == null || player.playerClientId != LethalMenu.localPlayer.playerClientId) return true;
            return !Hack.GodMode.IsEnabled();
        }
    }
}
