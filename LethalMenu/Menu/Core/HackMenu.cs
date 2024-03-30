using System.Collections.Generic;
using System.Linq;
using LethalMenu.Menu.Popup;
using LethalMenu.Menu.Tab;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Core;

internal class HackMenu : MenuFragment
{
    private const int SpaceFromTop = 60;
    public const int SpaceFromLeft = 10;

    private static HackMenu _instance;
    private readonly PopupMenu _firstSetupManagerWindow = new FirstSetupManagerWindow(4);

    private readonly List<MenuTab> _menuTabs = [];
    public readonly PopupMenu ItemManagerWindow = new ItemManagerWindow(3);
    public readonly PopupMenu LootManager = new LootManager(5);

    public readonly PopupMenu MoonManagerWindow = new MoonManagerWindow(1);
    public readonly PopupMenu UnlockableManagerWindow = new UnlockableManagerWindow(2);


    private Vector2 _scrollPosition = Vector2.zero;
    private int _selectedTab;
    public float ContentHeight;

    public float ContentWidth;
    public Rect WindowRect = new(50f, 50f, 700f, 450f);

    public HackMenu()
    {
        _instance = this;
        if (Settings.isDebugMode) _menuTabs.Add(new DebugTab());
        _menuTabs.Add(new SettingsTab());
        _menuTabs.Add(new GeneralTab());
        _menuTabs.Add(new SelfTab());
        _menuTabs.Add(new VisualsTab());
        _menuTabs.Add(new TrollTab());
        _menuTabs.Add(new PlayersTab());
        _menuTabs.Add(new EnemyTab());
        _menuTabs.Add(new ServerTab());

        Resize();

        _selectedTab = _menuTabs.IndexOf(_menuTabs.Find(x => x.Name == "General"));
    }

    public static HackMenu Instance
    {
        get { return _instance ??= new HackMenu(); }
    }


    public void Resize()
    {
        WindowRect.width = Settings.i_menuWidth;
        WindowRect.height = Settings.i_menuHeight;
        ContentWidth = WindowRect.width - SpaceFromLeft * 2;
        ContentHeight = WindowRect.height - SpaceFromTop;
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
        GUI.skin = ThemeUtil.Skin;
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

        GUI.skin.customStyles.First(x => x.name == "TabBtn").fontSize = Settings.i_menuFontSize;
        GUI.skin.customStyles.First(x => x.name == "SelectedTab").fontSize = Settings.i_menuFontSize;

        Resize();
    }

    public void Draw()
    {
        if (Settings.isFirstLaunch || Settings.isMenuOpen) Stylize();
        else return;

        if (Settings.isFirstLaunch)
        {
            _firstSetupManagerWindow.Draw();
        }
        else
        {
            GUI.color = new Color(1f, 1f, 1f, Settings.f_menuAlpha);
            WindowRect = GUILayout.Window(0, WindowRect, DrawContent, "Lethal Menu");
            UnlockableManagerWindow.Draw();
            ItemManagerWindow.Draw();
            MoonManagerWindow.Draw();
            LootManager.Draw();
            GUI.color = Color.white;
        }
    }

    private void DrawContent(int windowID)
    {
        GUI.color = new Color(1f, 1f, 1f, 0.1f);
        var watermark = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold };
        const string text = "Developed By IcyRelic, and Dustin";

        GUI.Label(
            new Rect(WindowRect.width - watermark.CalcSize(new GUIContent(text)).x - 10,
                WindowRect.height - watermark.CalcSize(new GUIContent(text)).y - 10,
                watermark.CalcSize(new GUIContent(text)).x, watermark.CalcSize(new GUIContent(text)).y), text,
            watermark);

        GUI.color = new Color(1f, 1f, 1f, Settings.f_menuAlpha);

        GUILayout.BeginVertical();
        GUILayout.BeginArea(new Rect(0, 25, WindowRect.width, 25), style: "Toolbar");

        GUILayout.BeginHorizontal();
        _menuTabs.ForEach(x => x.LocalizeName());
        _selectedTab = GUILayout.Toolbar(_selectedTab, _menuTabs.Select(x => x.Name).ToArray(), "TabBtn");
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
        GUILayout.Space(SpaceFromTop);

        GUILayout.BeginArea(new Rect(SpaceFromLeft, SpaceFromTop, WindowRect.width - SpaceFromLeft,
            ContentHeight - 15));

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        GUILayout.BeginHorizontal();
        _menuTabs[_selectedTab].Draw();
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();

        GUILayout.EndArea();

        GUI.color = Color.white;

        GUI.DragWindow();
    }
}