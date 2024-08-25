using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using System;
using System.Linq;
using UnityEngine;

namespace LethalMenu.Menu.Popup
{
    internal class UnlockableManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;
        private string s_search = "";

        public UnlockableManagerWindow(int id) : base("UnlockableManager.Title", new Rect(50f, 50f, 562f, 225f), id) { }

        public override void DrawContent(int windowID)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            var unlockables = Enum.GetValues(typeof(Unlockable)).Cast<Unlockable>().Where(u => u != Unlockable.Terminal || u != Unlockable.OrangeSuit).ToList();

            GUILayout.BeginHorizontal();
            UI.Textbox("General.Search", ref s_search);
            GUILayout.FlexibleSpace();
            UI.Button("UnlockableManager.UnlockAll", () => unlockables.ForEach(x => Hack.UnlockUnlockable.Execute(x, true, false)));
            GUILayout.EndHorizontal();

            UI.ButtonGrid(unlockables, (u) => u.GetItem().unlockableName, s_search, (u) => Hack.UnlockUnlockable.Execute(u, false, true), 3);

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
    }
}
