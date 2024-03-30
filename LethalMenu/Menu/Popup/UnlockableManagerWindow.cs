using System;
using System.Linq;
using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Popup;

internal class UnlockableManagerWindow : PopupMenu
{
    private Vector2 _scrollPosition = Vector2.zero;
    private string _sSearch = "";

    public UnlockableManagerWindow(int id) : base("UnlockableManager.Title", new Rect(50f, 50f, 562f, 225f), id)
    {
    }

    protected override void DrawContent(int windowID)
    {
        if (!(bool)StartOfRound.Instance) return;

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        var unlockables = Enum.GetValues(typeof(Unlockable)).Cast<Unlockable>().ToList();

        GUILayout.BeginHorizontal();
        UI.Textbox("General.Search", ref _sSearch);
        GUILayout.FlexibleSpace();
        UI.Button("UnlockableManager.UnlockAll", () => unlockables.ForEach(x => Hack.UnlockUnlockable.Execute(x)));
        GUILayout.EndHorizontal();


        UI.ButtonGrid(unlockables, u => u.GetItem().unlockableName, _sSearch, u => Hack.UnlockUnlockable.Execute(u), 3);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }
}