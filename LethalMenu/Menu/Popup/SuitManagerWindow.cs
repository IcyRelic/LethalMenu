using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using Steamworks.Ugc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LethalMenu.Menu.Popup
{
    internal class SuitManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;
        private string s_search = "";

        public SuitManagerWindow(int id) : base("SuitManager.Title", new Rect(50f, 50f, 562f, 225f), id) { }

        public override void DrawContent(int windowID)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            List<Unlockable> suitunlockables = new List<Unlockable>
            {
                Unlockable.OrangeSuit,
                Unlockable.GreenSuit,
                Unlockable.HazardSuit,
                Unlockable.PajamaSuit,
                Unlockable.PurpleSuit,
                Unlockable.BeeSuit,
                Unlockable.BunnySuit
            };

            GUILayout.BeginHorizontal();
            UI.Textbox("General.Search", ref s_search);
            UI.Toggle("SuitManager.WearBuy", ref Settings.b_WearBuy, "General.Disable", "General.Enable");
            UI.Button("SuitManager.UnlockAll", () => suitunlockables.ForEach(x => Hack.UnlockUnlockableSuit.Execute(x, Settings.b_WearBuy, true, true)));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            List<Unlockable> suits = suitunlockables.Where(u => u.GetItem().unlockableName.Contains(s_search, StringComparison.OrdinalIgnoreCase)).ToList();

            UI.ButtonGrid(suits, (x) => x.GetItem().unlockableName, s_search, (x) => Hack.UnlockUnlockableSuit.Execute(x, Settings.b_WearBuy, true, true), 3);

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }
    }
}
