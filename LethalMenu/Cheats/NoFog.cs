using Dissonance;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

namespace LethalMenu.Cheats
{
    internal class NoFog : Cheat
    {
        public override void Update()
        {
            GameObject gameObject = GameObject.Find("Systems");

            if (gameObject == null) return;

            gameObject.transform.Find("Rendering").Find("VolumeMain")?.gameObject.SetActive(!Hack.NoFog.IsEnabled());

            if (LethalMenu.volume?.sharedProfile?.TryGet(out Fog fog) == true) fog.active = !Hack.NoFog.IsEnabled();
        }
    }
}
