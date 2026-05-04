using System;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class HearAllAlivePeople : Cheat
    {
        [HarmonyPatch(typeof(StartOfRound), "UpdatePlayerVoiceEffects"), HarmonyPostfix]
        public static void UpdatePlayerVoiceEffects(StartOfRound __instance)
        {
            if (Hack.HearAllAlivePeople.IsEnabled() && !StartOfRound.Instance.shipIsLeaving)
            {
                foreach (PlayerControllerB player in LethalMenu.players)
                {
                    if (player == null || player.isPlayerDead) continue;
                    AudioSource currentVoiceChatAudioSource = player.currentVoiceChatAudioSource;
                    currentVoiceChatAudioSource.GetComponent<AudioLowPassFilter>().enabled = false;
                    currentVoiceChatAudioSource.GetComponent<AudioHighPassFilter>().enabled = false;
                    currentVoiceChatAudioSource.panStereo = 0f;
                    currentVoiceChatAudioSource.spatialBlend = 0f;
                    player.currentVoiceChatIngameSettings.set2D = true;
                    player.voicePlayerState.Volume = 1f;
                }
            }
        }
    }
}
