using System;
using LethalMenu.Language;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace LethalMenu.Menu.Popup;

internal class FirstSetupManagerWindow : PopupMenu
{
    private readonly string[] _languages;
    private bool _disableBtns;
    private Vector2 _scrollPos = Vector2.zero;

    private int _selectedLanguage = -1;
    private GUIStyle _style;

    public FirstSetupManagerWindow(int id) : base("FirstSetup.Title",
        new Rect(Screen.width / 2 - 150, Screen.height / 2 - 100f, 300f, 250f), id)
    {
        _languages = Localization.GetLanguages();
        IsOpen = true;
    }

    protected override void DrawContent(int windowID)
    {
        if (_disableBtns) GUI.enabled = false;
        SetupContent();
        if (_disableBtns) GUI.enabled = true;
        //GUI.DragWindow(new Rect(0.0f, 0.0f, 10000f, 45f));
    }

    private void SetupContent()
    {
        WindowRect.x = Screen.width / 2 - 150;
        WindowRect.y = Screen.height / 2 - 100f;

        _style = new GUIStyle(GUI.skin.button)
        {
            padding = new RectOffset(15, 15, 10, 10),
            margin = new RectOffset(5, 5, 5, 5),
            normal =
            {
                textColor = Settings.c_menuText.GetColor()
            },
            hover =
            {
                textColor = Settings.c_menuText.GetColor()
            },
            active =
            {
                textColor = Settings.c_menuText.GetColor()
            },
            fontSize = 18,
            fontStyle = FontStyle.Bold
        };

        UI.Label("FirstSetup.Welcome");
        GUILayout.Space(20f);

        Keybind();
        GUILayout.Space(20f);
        LangList();
        GUILayout.Space(10f);

        UI.Actions(new UIButton("FirstSetup.Complete", () =>
        {
            Settings.IsFirstLaunch = false;
            Settings.Config.SaveConfig();
        }, _style));
    }

    private void LangList()
    {
        GUILayout.BeginVertical();
        UI.Label("FirstSetup.SelectLanguage");
        GUILayout.Space(5f);

        var rect = new Rect(8, 255, 300f, 100f);
        GUI.Box(rect, GUIContent.none);

        GUILayout.BeginVertical(GUILayout.Width(300f), GUILayout.Height(100f));
        _scrollPos = GUILayout.BeginScrollView(_scrollPos);

        if (_selectedLanguage == -1)
            _selectedLanguage = Array.FindIndex(_languages, x => x == Localization.Language.Name);

        for (var i = 0; i < _languages.Length; i++)
        {
            if (_selectedLanguage == i) GUI.contentColor = Settings.c_playerESP.GetColor();

            if (GUILayout.Button(_languages[i], GUI.skin.label))
            {
                _selectedLanguage = i;
                Localization.SetLanguage(_languages[i]);
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
        var bind = Hack.OpenMenu.GetKeyBind();
        var kb = Hack.OpenMenu.HasKeyBind()
            ? bind.GetType() == typeof(KeyControl) ? ((KeyControl)bind).keyCode.ToString() : bind.displayName
            : "None";

        UI.Label("Current: " + kb);
        GUILayout.Space(20f);

        var btnText = Hack.OpenMenu.IsWaiting() ? "General.Waiting" : "FirstSetup.ClickToChange";


        UI.Actions(new UIButton(btnText, () =>
        {
            _disableBtns = true;
            KbUtil.BeginChangeKeyBind(Hack.OpenMenu, () => { _disableBtns = false; });
        }, _style));
        GUILayout.EndVertical();
    }
}