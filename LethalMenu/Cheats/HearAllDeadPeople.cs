using System;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class HearAllDeadPeople : Cheat
    {
        [HarmonyPatch(typeof(StartOfRound), "UpdatePlayerVoiceEffects")]
        public static class StartOfRoundUpdatePlayerVoiceEffectsPatch
        {
            public static void Postfix(StartOfRound __instance)
            {
                if (Hack.HearAllDeadPeople.IsEnabled() && !StartOfRound.Instance.shipIsLeaving)
                {
                    for (int i = 0; i < __instance.allPlayerScripts.Length; i++)
                    {
                        PlayerControllerB player = __instance.allPlayerScripts[i];

                        if (player != null && player.currentVoiceChatAudioSource != null && player.isPlayerDead)
                        {
                            AudioSource currentVoiceChatAudioSource = player.currentVoiceChatAudioSource;
                            AudioLowPassFilter lowPassFilter = currentVoiceChatAudioSource.GetComponent<AudioLowPassFilter>();
                            AudioHighPassFilter highPassFilter = currentVoiceChatAudioSource.GetComponent<AudioHighPassFilter>();
                            if (lowPassFilter != null)
                            {
                                lowPassFilter.enabled = false;
                            }
                            if (highPassFilter != null)
                            {
                                highPassFilter.enabled = false;
                            }
                            currentVoiceChatAudioSource.panStereo = 0f;
                            SoundManager.Instance.playerVoicePitchTargets[(int)((IntPtr)player.playerClientId)] = 1f;
                            SoundManager.Instance.SetPlayerPitch(1f, unchecked((int)player.playerClientId));
                            currentVoiceChatAudioSource.spatialBlend = 0f;
                            player.currentVoiceChatIngameSettings.set2D = true;
                            player.voicePlayerState.Volume = 1f;
                        }
                    }
                }
            }
        }
    }
}
