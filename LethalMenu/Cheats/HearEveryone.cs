using System;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats;

[HarmonyPatch(typeof(StartOfRound), "UpdatePlayerVoiceEffects")]
internal class HearEveryone : Cheat
{
    public static void Postfix(StartOfRound __instance)
    {
        if (!Hack.HearEveryone.IsEnabled() || StartOfRound.Instance.shipIsLeaving) return;
        foreach (var playerControllerB in __instance.allPlayerScripts)
        {
            if (!playerControllerB || !playerControllerB.currentVoiceChatAudioSource) continue;

            var currentVoiceChatAudioSource = playerControllerB.currentVoiceChatAudioSource;

            var lowPassFilter = currentVoiceChatAudioSource.GetComponent<AudioLowPassFilter>();
            var highPassFilter = currentVoiceChatAudioSource.GetComponent<AudioHighPassFilter>();

            if (lowPassFilter) lowPassFilter.enabled = false;

            if (highPassFilter) highPassFilter.enabled = false;

            currentVoiceChatAudioSource.panStereo = 0f;
            SoundManager.Instance.playerVoicePitchTargets[(int)(IntPtr)playerControllerB.playerClientId] = 1f;
            SoundManager.Instance.SetPlayerPitch(1f, unchecked((int)playerControllerB.playerClientId));
            currentVoiceChatAudioSource.spatialBlend = 0f;
            playerControllerB.currentVoiceChatIngameSettings.set2D = true;
            playerControllerB.voicePlayerState.Volume = 1f;
        }
    }
}