using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Cheats;
using System.Reflection;
using Unity.Netcode;
using UnityEngine;

namespace LethalMenu
{
    [HarmonyPatch]
    internal class Patches
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Disconnect))]
        public static void Disconnect(GameNetworkManager __instance)
        {
            LethalMenu.debugMessage2 = "Disconnect Detected";

            SpectatePlayer.Reset();
        }
        
        

    }
}
