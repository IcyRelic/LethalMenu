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
            windowRect.width = Settings.GUISize.GetMenuWidth();
            windowRect.height = Settings.GUISize.GetMenuHeight();
            tabPercent = Settings.GUISize.GetMenuTabWidth();
            btnPadding = Settings.GUISize.GetMenuTabPadding();

            tabWidth = windowRect.width * tabPercent;
            tabHeight = windowRect.height - spaceFromTop;
            contentWidth = windowRect.width * (1f - tabPercent) - spaceFromLeft;
            contentHeight = windowRect.height - spaceFromTop;
        }

        public void Draw()
        {
            if (!Settings.isMenuOpen) return;
            GUI.backgroundColor = Settings.c_background.GetColor();

            GUI.color = Color.white;

            GUI.skin.label.fontSize = Settings.GUISize.GetFontSize();
            GUI.skin.button.fontSize = Settings.GUISize.GetFontSize();
            GUI.skin.toggle.fontSize = Settings.GUISize.GetFontSize();
            GUI.skin.window.fontSize = Settings.GUISize.GetFontSize();
            GUI.skin.box.fontSize = Settings.GUISize.GetFontSize();
            GUI.skin.textField.fontSize = Settings.GUISize.GetFontSize();
            GUI.skin.horizontalSlider.fontSize = Settings.GUISize.GetFontSize();
            GUI.skin.horizontalSliderThumb.fontSize = Settings.GUISize.GetFontSize();
            GUI.skin.verticalSlider.fontSize = Settings.GUISize.GetFontSize();
            GUI.skin.verticalSliderThumb.fontSize = Settings.GUISize.GetFontSize();
            GUI.skin.window.margin = new RectOffset(10, 10, 0, 0);

            windowRect = GUILayout.Window(0, windowRect, new GUI.WindowFunction(DrawContent), "Lethal Menu");
            unlockableManagerWindow.Draw();
            itemManagerWindow.Draw();
            moonManagerWindow.Draw();
        }

        private void DrawContent(int windowID)
        {
            //set avatar as a transparent watermark behind the tabContent and make it square
    

            GUI.color = new Color(1f, 1f, 1f, 0.1f);
            GUIStyle watermark = new GUIStyle(GUI.skin.label) { fontSize = 30, fontStyle = FontStyle.Bold };
            string text = "Developed By IcyRelic";

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
            style.fontSize = Settings.GUISize.GetFontSize();


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
