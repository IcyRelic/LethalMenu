﻿using System.Collections.Generic;
using UnityEngine;
using LethalMenu.Menu.Popup;
using LethalMenu.Menu.Tab;
using LethalMenu.Themes;
using System.Linq;

namespace LethalMenu.Menu.Core
{
    internal class HackMenu : MenuFragment
    {
        private Vector2 scrollPos = Vector2.zero;
        public Rect windowRect = new Rect(50f, 50f, 700f, 450f);

        public PopupMenu MoonManagerWindow = new MoonManagerWindow(1);
        public PopupMenu UnlockableManagerWindow = new UnlockableManagerWindow(2);
        public PopupMenu ItemManagerWindow = new ItemManagerWindow(3);
        public PopupMenu FirstSetupManagerWindow = new FirstSetupManagerWindow(4);
        public PopupMenu LootManagerWindow = new LootManagerWindow(5);
        public PopupMenu WeatherManagerWindow = new WeatherManagerWindow(6);
        public PopupMenu SuitManagerWindow = new SuitManagerWindow(7);

        public static List<MenuTab> menuTabs = new List<MenuTab>();
        private int selectedTab = 0;

        public float contentWidth;
        public float contentHeight;
        public int spaceFromTop = 60;
        public int spaceFromLeft = 10;

        private static HackMenu instance;

        public static HackMenu Instance
        {
            get
            {
                if (instance == null)
                    instance = new HackMenu();
                return instance;
            }
        }

        public HackMenu()
        {
            instance = this;
            menuTabs.Add(new SettingsTab());
            menuTabs.Add(new GeneralTab());
            menuTabs.Add(new SelfTab());
            menuTabs.Add(new VisualsTab());
            menuTabs.Add(new TrollTab());
            menuTabs.Add(new PlayersTab());
            menuTabs.Add(new EnemyTab());
            menuTabs.Add(new ServerTab());
            if (Settings.DebugMode) menuTabs.Add(new DebugTab());


            Resize();

            selectedTab = menuTabs.IndexOf(menuTabs.Find(x => x is GeneralTab));
        }

        public void ToggleDebugTab(bool enabled)
        {
            if (enabled && !menuTabs.Any(t => t is DebugTab)) menuTabs.Add(new DebugTab());
            else menuTabs.RemoveAll(t => t is DebugTab);
        }


        public void Resize()
        {
            windowRect.width = Settings.i_menuWidth;
            windowRect.height = Settings.i_menuHeight;
            contentWidth = windowRect.width - (spaceFromLeft * 2);
            contentHeight = windowRect.height - spaceFromTop;
        }

        public void ResetMenuSize()
        {
            Settings.i_menuFontSize = 14;
            Settings.i_menuWidth = 810;
            Settings.i_menuHeight = 410;
            Settings.i_sliderWidth = 100;
            Settings.i_textboxWidth = 85;
            Settings.Config.SaveConfig();
        }

        public void Stylize()
        {
            GUI.skin = Theme.Skin;
            GUI.color = Color.white;

            GUI.skin.label.fontSize = Settings.i_menuFontSize;
            GUI.skin.button.fontSize = Settings.i_menuFontSize;
            GUI.skin.toggle.fontSize = Settings.i_menuFontSize;
            //GUI.skin.window.fontSize = Settings.i_menuFontSize;
            GUI.skin.box.fontSize = Settings.i_menuFontSize;
            GUI.skin.textField.fontSize = Settings.i_menuFontSize;
            GUI.skin.horizontalSlider.fontSize = Settings.i_menuFontSize;
            GUI.skin.horizontalSliderThumb.fontSize = Settings.i_menuFontSize;
            GUI.skin.verticalSlider.fontSize = Settings.i_menuFontSize;
            GUI.skin.verticalSliderThumb.fontSize = Settings.i_menuFontSize;

            GUI.skin.customStyles.Where(x => x.name == "TabBtn").First().fontSize = Settings.i_menuFontSize;
            GUI.skin.customStyles.Where(x => x.name == "SelectedTab").First().fontSize = Settings.i_menuFontSize;

            Resize();
        }

        public void Draw()
        {
            if (Settings.isFirstLaunch || Settings.isMenuOpen) Stylize(); else return;
            if (Settings.isFirstLaunch) FirstSetupManagerWindow.Draw();
            else
            {
                GUI.color = new Color(1f, 1f, 1f, Settings.f_menuAlpha);
                windowRect = GUILayout.Window(0, windowRect, new GUI.WindowFunction(DrawContent), "Lethal Menu");
                UnlockableManagerWindow.Draw();
                ItemManagerWindow.Draw();
                MoonManagerWindow.Draw();
                LootManagerWindow.Draw();
                WeatherManagerWindow.Draw();
                SuitManagerWindow.Draw();
                GUI.color = Color.white;
            }
        }

        private void DrawContent(int windowID)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.1f);
            GUIStyle watermark = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
            string text = "Developed By IcyRelic, and Dustin";

            GUI.Label(new Rect(windowRect.width - watermark.CalcSize(new GUIContent(text)).x - 10, windowRect.height - watermark.CalcSize(new GUIContent(text)).y - 10, watermark.CalcSize(new GUIContent(text)).x, watermark.CalcSize(new GUIContent(text)).y), text, watermark);

            GUI.color = new Color(1f, 1f, 1f, Settings.f_menuAlpha);

            GUILayout.BeginVertical();
            GUILayout.BeginArea(new Rect(0, 25, windowRect.width, 25), style: "Toolbar");

            GUILayout.BeginHorizontal();
            menuTabs.ForEach(x => x.LocalizeName());
            selectedTab = GUILayout.Toolbar(selectedTab, menuTabs.Select(x => x.name).ToArray(), style: "TabBtn");
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            GUILayout.Space(spaceFromTop);

            GUILayout.BeginArea(new Rect(spaceFromLeft, spaceFromTop, windowRect.width - spaceFromLeft, contentHeight - 15));

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.BeginHorizontal();
            menuTabs[selectedTab].Draw();
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();

            GUILayout.EndArea();

            GUI.color = Color.white;

            GUI.DragWindow();
        }
    }
}
