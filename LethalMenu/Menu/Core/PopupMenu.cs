using System.Collections.Generic;
using LethalMenu.Language;
using UnityEngine;

namespace LethalMenu.Menu.Core;

internal class PopupMenu : MenuFragment
{
    private readonly int _id;
    private readonly string _localization;
    private string _name;
    public bool IsOpen = false;

    protected List<MenuTab> MenuTabs = [];
    protected Rect WindowRect;

    protected PopupMenu(string name, Rect size, int id)
    {
        _localization = name;
        WindowRect = size;
        _id = id;
        LocalizeName();
    }

    private void LocalizeName()
    {
        _name = Localization.Localize(_localization);
    }

    public void Draw()
    {
        if (!IsOpen) return;
        WindowRect = GUILayout.Window(_id, WindowRect, DrawContent, _name);
    }

    protected virtual void DrawContent(int windowID)
    {
    }
}