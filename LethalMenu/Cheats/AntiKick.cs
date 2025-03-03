using GameNetcodeStuff;
using HarmonyLib;
using Steamworks;
using LethalMenu.Util;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Steamworks.Data;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class AntiKick : Cheat
    {
        public static List<ulong> HostKickedPlayerList = new List<ulong>();
        private static bool HostQuit = false;
        private static bool HostKicked = false;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerControllerB), "SendNewPlayerValuesServerRpc")]
        public static bool SendNewPlayerValuesServerRpc(PlayerControllerB __instance, ulong newPlayerSteamId)
        {
            if (Hack.AntiKick.IsEnabled() && HostKicked)
            {
                __instance.sentPlayerValues = true;
                ulong[] playerSteamIds = __instance.playersManager.allPlayerScripts.Select(p => p.playerSteamId).ToArray();
                playerSteamIds[__instance.playerClientId] = SteamClient.SteamId;
                __instance.Reflect().Invoke("SendNewPlayerValuesClientRpc", playerSteamIds);
                HostKicked = false;
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Disconnect))]
        public static void Disconnect(GameNetworkManager __instance)
        {
            if (GameNetworkManager.Instance.disconnectReason == 1)
            {
                if (!HostQuit) HostKickedPlayerList.Add(Settings.s_lobbyid);
                HostQuit = false;
            }
            if (GameNetworkManager.Instance.disconnectReason == 3) HostKickedPlayerList.Add(Settings.s_lobbyid);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.OnClientDisconnect))]
        public static void OnClientDisconnect(StartOfRound __instance, ulong clientId)
        {
            if (StartOfRound.Instance.ClientPlayerList.TryGetValue(clientId, out var value3)) if (value3 == 0 && !GameNetworkManager.Instance.isDisconnecting) HostQuit = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.JoinLobby))]
        public static void JoinLobby(GameNetworkManager __instance, Lobby lobby, SteamId id)
        {
            if (HostKickedPlayerList.Contains(lobby.Owner.Id)) HostKicked = true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.StartClient))]
        public static void StartClient(GameNetworkManager __instance, SteamId id)
        {
            if (HostKickedPlayerList.Contains(id)) HostKicked = true;
        }
    }
}
