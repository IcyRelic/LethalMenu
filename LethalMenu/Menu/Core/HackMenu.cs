using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LethalMenu.Menu.Popup;
using LethalMenu.Menu.Tab;

namespace LethalMenu.Menu.Core
{
    internal class HackMenu : MenuFragment
    {
        public Rect windowRect = new Rect(50f, 50f, 700f, 450f);

        public PopupMenu moonManagerWindow = new MoonManagerWindow(1);
        public PopupMenu unlockableManagerWindow = new UnlockableManagerWindow(2);
        public PopupMenu itemManagerWindow = new ItemManagerWindow(3);
        public PopupMenu firstSetupManagerWindow = new FirstSetupManagerWindow(4);

        private List<MenuTab> menuTabs = new List<MenuTab>();
        private int selectedTab = 0;

        public float tabPercent = 0.20f;
        public int btnPadding = 15;
        public float contentWidth;
        public float contentHeight;
        public float tabWidth;
        public float tabHeight;
        public int spaceFromTop = 45;
        public int spaceFromLeft = 10;
        

        private Vector2 scrollPos = Vector2.zero;

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
            menuTabs.Add(new DebugTab());
            menuTabs.Add(new SettingsTab());
            menuTabs.Add(new GeneralTab());
            menuTabs.Add(new SelfTab());
            menuTabs.Add(new VisualsTab());
            menuTabs.Add(new TrollTab());
            menuTabs.Add(new PlayersTab());
            menuTabs.Add(new EnemyTab());
            menuTabs.Add(new ServerTab());


            Resize();

            selectedTab = menuTabs.IndexOf(menuTabs.Find(x => x.name == "General"));
            
        }

        
        public void Resize()
        {
            windowRect.width = Settings.i_menuWidth;
            windowRect.height = Settings.i_menuHeight;
            tabPercent = Settings.f_tabWidth;
            btnPadding = Settings.i_tabPadding;

            tabWidth = windowRect.width * tabPercent;
            tabHeight = windowRect.height - spaceFromTop;
            contentWidth = windowRect.width * (1f - tabPercent) - spaceFromLeft;
            contentHeight = windowRect.height - spaceFromTop;
        }

        public void ResetMenuSize()
        {
            Settings.i_menuFontSize = 14;
            Settings.i_menuWidth = 810;
            Settings.i_menuHeight = 410;
            Settings.i_tabPadding = 10;
            Settings.i_sliderWidth = 100;
            Settings.i_textboxWidth = 85;
            Settings.f_tabWidth = 0.15f;
            Settings.Config.SaveConfig();
        }

        public void Stylize()
        {
            GUI.backgroundColor = Settings.c_background.GetColor();

            GUI.color = Color.white;

            GUI.skin.label.fontSize = Settings.i_menuFontSize;
            GUI.skin.button.fontSize = Settings.i_menuFontSize;
            GUI.skin.toggle.fontSize = Settings.i_menuFontSize;
            GUI.skin.window.fontSize = Settings.i_menuFontSize;
            GUI.skin.box.fontSize = Settings.i_menuFontSize;
            GUI.skin.textField.fontSize = Settings.i_menuFontSize;
            GUI.skin.horizontalSlider.fontSize = Settings.i_menuFontSize;
            GUI.skin.horizontalSliderThumb.fontSize = Settings.i_menuFontSize;
            GUI.skin.verticalSlider.fontSize = Settings.i_menuFontSize;
            GUI.skin.verticalSliderThumb.fontSize = Settings.i_menuFontSize;
            GUI.skin.window.margin = new RectOffset(10, 10, 0, 0);

            Resize();
        }

        public void Draw()
        {
            if (Settings.isFirstLaunch || Settings.isMenuOpen) Stylize();
            else return;

            if(Settings.isFirstLaunch) firstSetupManagerWindow.Draw();
            else
            {
                windowRect = GUILayout.Window(0, windowRect, new GUI.WindowFunction(DrawContent), "Lethal Menu");
                unlockableManagerWindow.Draw();
                itemManagerWindow.Draw();
                moonManagerWindow.Draw();
            }            
        }

        private void DrawContent(int windowID)
        {

            GUI.color = new Color(1f, 1f, 1f, 0.1f);
            GUIStyle watermark = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
            string text = "Developed By IcyRelic , and Dustin";

            //draw the watermark in the bottom right of the screen
            GUI.Label(new Rect(windowRect.width - watermark.CalcSize(new GUIContent(text)).x - 10, windowRect.height - watermark.CalcSize(new GUIContent(text)).y - 10, watermark.CalcSize(new GUIContent(text)).x, watermark.CalcSize(new GUIContent(text)).y), text, watermark);
            GUI.color = Color.white;




            GUILayout.BeginHorizontal();
            GUILayout.BeginArea(new Rect(spaceFromLeft, spaceFromTop, tabWidth - spaceFromLeft, tabHeight));


            GUILayout.BeginVertical();

            GUIStyle style = new GUIStyle(GUI.skin.button);

            style.padding = new RectOffset(btnPadding, btnPadding, btnPadding, btnPadding);
            style.margin = new RectOffset(btnPadding, btnPadding, 5, 5);
            style.normal.textColor = Settings.c_menuText.GetColor();
            style.hover.textColor = Settings.c_menuText.GetColor();
            style.active.textColor = Settings.c_menuText.GetColor();
            style.fontSize = Settings.i_menuFontSize;


            for (int i = 0; i < menuTabs.Count; i++)
            {
                if (menuTabs[i].name == "Debug" && !Settings.isDebugMode) continue;
                menuTabs[i].LocalizeName();
                if (GUILayout.Button(menuTabs[i].name, style)) selectedTab = i;
            }


            GUILayout.EndVertical();
            GUILayout.EndArea();

            


            GUILayout.BeginArea(new Rect(tabWidth + spaceFromLeft * 2, spaceFromTop, contentWidth, contentHeight));

            GUILayout.BeginHorizontal();
            menuTabs[selectedTab].Draw();
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
            GUI.DragWindow(new Rect(0.0f, 0.0f, 10000f, 45f));


        }




    }
}
