using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LethalMenu.Menu.Popup
{
    internal class WeatherManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;

        public WeatherManagerWindow(int id) : base("WeatherManager.Title", new Rect(50f, 50f, 350f, 250f), id) { }

        public override void DrawContent(int windowID)
        {
            if (!LethalMenu.localPlayer.IsHost)
            {
                UI.Label("General.HostRequired", Settings.c_error);
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            if ((bool)StartOfRound.Instance)
            {
                UI.Header("MoonManager.CurrentMoon");
                UI.Label("MoonManager.Moon", StartOfRound.Instance.currentLevel.PlanetName);
                UI.Label("MoonManager.Weather", StartOfRound.Instance.currentLevel.currentWeather.ToString());
                List<LevelWeatherType> weathertypes = new((LevelWeatherType[])Enum.GetValues(typeof(LevelWeatherType))); 
                weathertypes.ForEach(weather =>
                {
                    UI.Button(weather.ToString(), () => {
                        StartOfRound.Instance.currentLevel.currentWeather = weather;
                        HUDManager.Instance.DisplayTip($"Lethal Menu", $"{StartOfRound.Instance.currentLevel.PlanetName} weather set to {StartOfRound.Instance.currentLevel.currentWeather}!");
                    }, "General.Set");
                });
            }
            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
    }
}
