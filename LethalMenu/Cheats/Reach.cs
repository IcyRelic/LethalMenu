using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class Reach : Cheat
    {
        public override void Update()
        {
            
            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
            if (player == null) return;
            if (Settings.f_defaultGrabDistance == -1f) Settings.f_defaultGrabDistance = player.grabDistance;

            if(!Hack.LootThroughWalls.IsEnabled() && !Hack.InteractThroughWalls.IsEnabled()) 
                player.grabDistance = Hack.Reach.IsEnabled() ? Settings.f_grabDistance : Settings.f_defaultGrabDistance;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "LateUpdate")]
        public static void PlayerLateUpdate(PlayerControllerB __instance)
        {
            if (LethalMenu.localPlayer == null || LethalMenu.localPlayer.playerClientId != __instance.playerClientId || !Hack.Reach.IsEnabled()) return;

            if (!Hack.LootThroughWalls.IsEnabled() && !Hack.InteractThroughWalls.IsEnabled())
                __instance.grabDistance = Hack.Reach.IsEnabled() ? Settings.f_grabDistance : Settings.f_defaultGrabDistance;
        }
    }
}
