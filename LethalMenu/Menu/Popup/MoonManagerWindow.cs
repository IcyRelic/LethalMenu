using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LethalMenu.Menu.Popup
{
    internal class MoonManagerWindow : PopupMenu
    {
        private List<SelectableLevel> selectableLevels = new List<SelectableLevel>();
        private Vector2 scrollPos = Vector2.zero;

        public MoonManagerWindow(int id) : base("MoonManager.Title", new Rect(50f, 50f, 350f, 250f), id) { }

        public override void DrawContent(int windowID)
        {
            if (StartOfRound.Instance == null || StartOfRound.Instance.currentLevel == null || StartOfRound.Instance.levels == null)
            {
                UI.Label("General.NullError", Settings.c_error);
                GUI.DragWindow();
                return;
            }

            if (selectableLevels.Count == 0) selectableLevels = StartOfRound.Instance.levels.Where(s => s != null && SceneUtility.GetBuildIndexByScenePath(s.sceneName) != -1).ToList();

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            UI.Header("MoonManager.CurrentMoon");
            UI.Label("MoonManager.Moon", StartOfRound.Instance.currentLevel.PlanetName);                      
            UI.Label("MoonManager.Weather", StartOfRound.Instance.currentLevel.currentWeather.ToString());   
            UI.Label("MoonManager.Risk", StartOfRound.Instance.currentLevel.riskLevel);

            UI.Header("MoonManager.ChangeMoon", true);

            selectableLevels.Where(x => x != null && x.levelID != StartOfRound.Instance.currentLevel.levelID).ToList().ForEach(x =>
            {
                UI.Button($"{x.PlanetName}{(x.currentWeather == LevelWeatherType.None ? "" : $" ({x.currentWeather})")}", () =>
                {
                    RoundHandler.ChangeMoon(x.levelID);
                    HUDManager.Instance.DisplayTip($"Lethal Menu", $"Changed moon to {x.PlanetName}!");
                }, "MoonManager.Visit");
            });

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
    }
}
