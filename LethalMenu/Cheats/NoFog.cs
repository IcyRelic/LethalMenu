using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class NoFog : Cheat
    {
        private static Dictionary<LocalVolumetricFog, bool> OriginalFogs = new Dictionary<LocalVolumetricFog, bool>();
        private static Dictionary<Volume, bool> OriginalVolumes = new Dictionary<Volume, bool>();

        [HarmonyPatch(typeof(RoundManager), "FinishGeneratingNewLevelClientRpc"), HarmonyPostfix]
        public static void FinishGeneratingNewLevelClientRpc(RoundManager __instance)
        {
            if (Settings.b_NoFog) ToggleNoFog();
        }

        public static void ToggleNoFog()
        {
            LethalMenu.fogs.Where(f => f != null).ToList().ForEach(f => ToggleFog(f));
            LethalMenu.volumes.Where(v => v != null && v.gameObject != null && (v.gameObject.name == "VolumeMain" || v.gameObject.name == "StormVolume")).ToList().ForEach(v => ToggleFog(null, v));
        }

        private static void ToggleFog(LocalVolumetricFog fog = null, Volume volume = null)
        {
            if (Settings.b_NoFog)
            {
                if (fog != null && !OriginalFogs.ContainsKey(fog))
                {
                    OriginalFogs.Add(fog, fog.enabled);
                    if (fog.enabled) fog.enabled = false;
                }
                else if (volume != null && !OriginalVolumes.ContainsKey(volume))
                {
                    OriginalVolumes.Add(volume, volume.enabled);
                    if (volume.enabled) volume.enabled = false;
                }
            }
            else if (fog != null && OriginalFogs.ContainsKey(fog))
            {
                fog.enabled = OriginalFogs[fog];
                OriginalFogs.Remove(fog);
            }
            else if (volume != null && OriginalVolumes.ContainsKey(volume))
            {
                volume.enabled = OriginalVolumes[volume];
                OriginalVolumes.Remove(volume);
            }
        }
    }
}
