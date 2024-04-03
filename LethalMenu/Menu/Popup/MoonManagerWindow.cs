using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;

namespace LethalMenu.Menu.Popup
{
    internal class MoonManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;

        public MoonManagerWindow(int id) : base("Moon Manager", new Rect(50f, 50f, 350f, 250f), id)
        {

        }

        public override void DrawContent(int windowID)
        {
            //if (!(bool)StartOfRound.Instance) return;
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            if((bool) StartOfRound.Instance)
            {
                UI.Header("MoonManager.CurrentMoon");
                UI.Label("MoonManager.Moon", StartOfRound.Instance.currentLevel.PlanetName);
                UI.Label("MoonManager.Weather", StartOfRound.Instance.currentLevel.currentWeather.ToString());
                UI.Label("MoonManager.Risk", StartOfRound.Instance.currentLevel.riskLevel);

                UI.Header("MoonManager.ChangeMoon", true);

                int[] levelOrder = { 3, 0, 1, 2, 7, 4, 5, 6, 8 };

                Dictionary<int, int> order = levelOrder
                    .Select((id, index) => new { id, index })
                    .ToDictionary(item => item.id, item => item.index);



                               foreach (SelectableLevel x in StartOfRound.Instance.levels.OrderBy(x => order.ContainsKey(x.levelID) ? order[x.levelID] : int.MaxValue))
                {
                    if (x.levelID == StartOfRound.Instance.currentLevel.levelID) continue;

                    string weather = x.currentWeather == LevelWeatherType.None ? "" : $" ({x.currentWeather})";

                    UI.Button($"{x.PlanetName}{weather}", () => RoundHandler.ChangeMoon(x.levelID), "MoonManager.Visit");
                }
            }

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }


    }
}
