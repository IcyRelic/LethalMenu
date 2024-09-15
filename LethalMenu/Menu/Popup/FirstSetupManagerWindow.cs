using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Language;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace LethalMenu.Menu.Popup
{
    internal class FirstSetupManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;

        public static ButtonControl bind = Hack.OpenMenu.GetKeyBind();
        public static string kb = Hack.OpenMenu.HasKeyBind() ? bind.GetType() == typeof(KeyControl) ? ((KeyControl)bind).keyCode.ToString() : bind.displayName : "None";
        private int selectedLanguage = -1;
        private string[] languages;
        private bool disableBtns = false;

        public FirstSetupManagerWindow(int id) : base("FirstSetup.Title", new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100f, 300f, 250f), id) 
        {
            languages = Localization.GetLanguages();
            isOpen = true; 
        }
        public override void DrawContent(int windowID)
        {
            if (disableBtns) GUI.enabled = false;
            windowRect.x = Screen.width / 2 - 150;
            windowRect.y = Screen.height / 2 - 100f;

            UI.Label("FirstSetup.Welcome");
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
            UI.Label("FirstSetup.SelectKeybind");
         
            UI.Label("FirstSetup.Keybind", $"{kb}");
            GUILayout.Space(20f);

            string btnText = Hack.OpenMenu.IsWaiting() ? "General.Waiting" : "FirstSetup.ClickToChange";

            UI.Actions(new UIButton(btnText, () =>
            {
                disableBtns = true;
                KBUtil.BeginChangeKeyBind(Hack.OpenMenu, () => { disableBtns = false; });
            }));

            GUILayout.EndVertical();
            GUILayout.Space(20f);
            GUILayout.BeginVertical();
            UI.Label("FirstSetup.SelectLanguage");
            GUILayout.Space(10f);

            Rect rect = new Rect(8, 255, 300f, 100f);
            GUI.Box(rect, GUIContent.none);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(300f), GUILayout.Height(100f));
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(300f), GUILayout.Height(100f));

            if (selectedLanguage == -1) selectedLanguage = Array.FindIndex(languages, x => x == Localization.Language.Name);

            for (int i = 0; i < languages.Length; i++)
            {
                if (selectedLanguage == i) GUI.contentColor = Settings.c_playerESP.GetColor();

                if (GUILayout.Button(languages[i], GUI.skin.label))
                {
                    selectedLanguage = i;
                    Localization.SetLanguage(languages[i]);
                }

                GUI.contentColor = Settings.c_menuText.GetColor();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.Space(20f);

            UI.Actions(new UIButton("FirstSetup.Complete", () =>
            {
                Settings.isFirstLaunch = false;
                Settings.Config.SaveConfig();
            }));
            if (disableBtns) GUI.enabled = true;
            GUI.DragWindow();
        }
    }
}
