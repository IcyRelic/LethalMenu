using System;
using System.Linq;
using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Popup;

internal class UnlockableManagerWindow : PopupMenu
{
    private string s_search = "";
    private Vector2 scrollPos = Vector2.zero;

    public UnlockableManagerWindow(int id) : base("UnlockableManager.Title", new Rect(50f, 50f, 562f, 225f), id)
    {
    }

    protected override void DrawContent(int windowID)
    {
        if (!(bool)StartOfRound.Instance) return;

        scrollPos = GUILayout.BeginScrollView(scrollPos);

        var unlockables = Enum.GetValues(typeof(Unlockable)).Cast<Unlockable>().ToList();

        GUILayout.BeginHorizontal();
        UI.Textbox("General.Search", ref s_search);
        GUILayout.FlexibleSpace();
        UI.Button("UnlockableManager.UnlockAll", () => unlockables.ForEach(x => Hack.UnlockUnlockable.Execute(x)));
        GUILayout.EndHorizontal();


        UI.ButtonGrid(unlockables, u => u.GetItem().unlockableName, s_search, u => Hack.UnlockUnlockable.Execute(u), 3);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }
}