using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Language;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace LethalMenu.Menu.Popup
{
    internal class FirstSetupManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;

        private int selectedLanguage = -1;
        private string[] languages;
        private GUIStyle style;
        private bool disableBtns = false;

        public FirstSetupManagerWindow(int id) : base("FirstSetupWindow.Title", new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100f, 300f, 250f), id) 
        {
            languages = Localization.GetLanguages();
            isOpen = true; 
        }
        public override void DrawContent(int windowID)
        {
            if (disableBtns) GUI.enabled = false;
            SetupContent();
            if (disableBtns) GUI.enabled = true;
        }

        private void SetupContent()
        {
            windowRect.x = Screen.width / 2 - 150;
            windowRect.y = Screen.height / 2 - 100f;

            style = new GUIStyle(GUI.skin.button);

            style.padding = new RectOffset(15, 15, 10, 10);
            style.margin = new RectOffset(5, 5, 5, 5);
            style.normal.textColor = Settings.c_menuText.GetColor();
            style.hover.textColor = Settings.c_menuText.GetColor();
            style.active.textColor = Settings.c_menuText.GetColor();
            style.fontSize = 18;
            style.fontStyle = FontStyle.Bold;

            UI.Label("FirstSetup.Welcome");
            GUILayout.Space(20f);

            Keybind();
            GUILayout.Space(20f);
            LangList();
            GUILayout.Space(10f);

            UI.Actions(new UIButton("FirstSetup.Complete", () =>
            {
                Settings.isFirstLaunch = false;
                Settings.Config.SaveConfig();
            }, style));
        }

        private void LangList()
        {
            GUILayout.BeginVertical();
            UI.Label("FirstSetup.SelectLanguage");
            GUILayout.Space(5f);
  
            Rect rect = new Rect(8, 255, 300f, 100f);
            GUI.Box(rect, GUIContent.none);
            
            GUILayout.BeginVertical(GUILayout.Width(300f), GUILayout.Height(100f));
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            if (selectedLanguage == -1) selectedLanguage = Array.FindIndex(languages, x => x == Localization.Language.Name);

            for (int i = 0; i < languages.Length; i++)
            {
                if(selectedLanguage == i) GUI.contentColor = Settings.c_playerESP.GetColor();

                if (GUILayout.Button(languages[i], GUI.skin.label))
                {
                    selectedLanguage = i;
                    Localization.SetLanguage(languages[i]);
                }

                GUI.contentColor = Settings.c_menuText.GetColor();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndVertical();
        }

        private void Keybind()
        {
            GUILayout.BeginVertical();
            UI.Label("FirstSetup.SelectKeybind");
            ButtonControl bind = Hack.OpenMenu.GetKeyBind();
            string kb = Hack.OpenMenu.HasKeyBind() ? bind.GetType() == typeof(KeyControl) ? ((KeyControl)bind).keyCode.ToString() : bind.displayName : "None";

            UI.Label("Current: " + kb);
            GUILayout.Space(20f);

            string btnText = Hack.OpenMenu.IsWaiting() ? "General.Waiting" : "FirstSetup.ClickToChange";

            

            UI.Actions(new UIButton(btnText, () =>
            {
                disableBtns = true;
                KBUtil.BeginChangeKeyBind(Hack.OpenMenu, () => { disableBtns = false; });
            }, style));
            GUILayout.EndVertical();
        }
    }
}
