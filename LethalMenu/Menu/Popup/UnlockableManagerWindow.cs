using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LethalMenu.Menu.Popup
{
    internal class UnlockableManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;
        private string s_search = "";
        private List<Unlockable> unlockables = new List<Unlockable>();

        public UnlockableManagerWindow(int id) : base("UnlockableManager.Title", new Rect(50f, 50f, 562f, 225f), id) { }

        public override void DrawContent(int windowID)
        {
            if (LethalMenu.localPlayer == null || StartOfRound.Instance == null)
            {
                UI.Label("General.NullError", Settings.c_error);
                GUI.DragWindow();
                return;
            }

            if (unlockables.Count == 0) unlockables = Enum.GetValues(typeof(Unlockable)).Cast<Unlockable>().Where(u => u != Unlockable.OrangeSuit).ToList();

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.BeginHorizontal();
            UI.Textbox("General.Search", ref s_search);
            GUILayout.FlexibleSpace();
            UI.Button("UnlockableManager.UnlockAll", () => BuyAllUnlockables());
            GUILayout.EndHorizontal();

            UI.ButtonGrid(unlockables, (u) => u.GetItem().unlockableName, s_search, (u) => BuyUnlockable(u), 3);

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        private void BuyAllUnlockables()
        {
            if (HUDManager.Instance == null) return;
            Enum.GetValues(typeof(Unlockable)).Cast<Unlockable>().ToList().ForEach(u => BuyUnlockable(u, false));
            HUDManager.Instance.DisplayTip("Lethal Menu", "Unlocked all unlockables!");
        }

        public void BuyUnlockable(Unlockable unlockable, bool enabled = true)
        {
            if (StartOfRound.Instance == null || HUDManager.Instance == null || LethalMenu.terminal == null) return;
            unlockable.Buy(LethalMenu.terminal.groupCredits);
            if (enabled) HUDManager.Instance.DisplayTip("Lethal Menu", $"Unlocked {unlockable.GetItem().unlockableName}!");
        }
    }
}
