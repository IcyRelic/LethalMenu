using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace LethalMenu.Cheats
{
    internal class NoFog : Cheat
    {
        public override void Update()
        {
            GameObject gameObject = GameObject.Find("Systems");

            if (gameObject == null || TimeOfDay.Instance.currentLevelWeather != LevelWeatherType.Foggy) return;
            // fix later and add specifics maps with fog

            gameObject.transform.Find("Rendering").Find("VolumeMain")?.gameObject.SetActive(!Hack.NoFog.IsEnabled());
            gameObject.transform.Find("Rendering").Find("VolumeMain (1)")?.gameObject.SetActive(!Hack.NoFog.IsEnabled());
            RoundManager.Instance.indoorFog.gameObject.SetActive(!Hack.NoFog.IsEnabled());
            TimeOfDay.Instance.foggyWeather.gameObject.SetActive(!Hack.NoFog.IsEnabled());
        }
    }
}
