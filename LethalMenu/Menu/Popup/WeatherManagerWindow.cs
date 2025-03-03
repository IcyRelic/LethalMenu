using LethalMenu.Handler;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LethalMenu.Menu.Popup
{
    internal class WeatherManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;
        private List<LevelWeatherType> Weathers = new List<LevelWeatherType>();

        public WeatherManagerWindow(int id) : base("WeatherManager.Title", new Rect(50f, 50f, 350f, 250f), id) { }

        public override void DrawContent(int windowID)
        {
            if (LethalMenu.localPlayer == null || StartOfRound.Instance == null)
            {
                UI.Label("General.NullError", Settings.c_error);
                GUI.DragWindow();
                return;
            }

            if (!LethalMenu.localPlayer.IsHost())
            {
                UI.Label("General.HostRequired", Settings.c_error);
                GUI.DragWindow();
                return;      
            }

            if (Weathers.Count == 0) Weathers = Enum.GetValues(typeof(LevelWeatherType)).Cast<LevelWeatherType>().ToList();

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            UI.Header("MoonManager.CurrentMoon");
            UI.Label("MoonManager.Moon", StartOfRound.Instance.currentLevel.PlanetName);
            UI.Label("MoonManager.Weather", StartOfRound.Instance.currentLevel.currentWeather.ToString());

            Weathers.Where(w => w != StartOfRound.Instance.currentLevel.currentWeather).ToList().ForEach(w =>
            {
                UI.Button(w.ToString(), () => {
                    StartOfRound.Instance.currentLevel.currentWeather = w;
                    HUDManager.Instance.DisplayTip($"Lethal Menu", $"{StartOfRound.Instance.currentLevel.PlanetName} weather set to {StartOfRound.Instance.currentLevel.currentWeather}!");
                }, "General.Set");
            });

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
    }
}
