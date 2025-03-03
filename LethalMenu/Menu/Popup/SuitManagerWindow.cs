using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LethalMenu.Menu.Popup
{
    internal class SuitManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;
        private string s_search = "";

        private List<Unlockable> SuitUnlockables = new List<Unlockable>
        {
            Unlockable.OrangeSuit,
            Unlockable.GreenSuit,
            Unlockable.HazardSuit,
            Unlockable.PajamaSuit,
            Unlockable.PurpleSuit,
            Unlockable.BeeSuit,
            Unlockable.BunnySuit
        };

        public SuitManagerWindow(int id) : base("SuitManager.Title", new Rect(50f, 50f, 562f, 225f), id) { }

        public override void DrawContent(int windowID)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.BeginHorizontal();
            UI.Textbox("General.Search", ref s_search, "", 0, false);
            UI.Button("SuitManager.UnlockAll", () => UnlockAllUnlockableSuits());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            UI.ButtonGrid(SuitUnlockables.Where(u => u.GetItem().unlockableName.Contains(s_search, StringComparison.OrdinalIgnoreCase)).ToList(), (x) => x.GetItem().unlockableName, s_search, (x) => BuyUnlockableSuit(x), 3);

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        private void BuyUnlockableSuit(Unlockable suit, bool enabled = true)
        {
            if (StartOfRound.Instance == null || HUDManager.Instance == null || LethalMenu.terminal == null || LethalMenu.localPlayer == null || suit.GetItem().hasBeenUnlockedByPlayer) return;
            suit.Buy(LethalMenu.terminal.groupCredits);
            suit.SetLocked(true);
            if (enabled) HUDManager.Instance.DisplayTip("Lethal Menu", $"Unlocked {suit.GetItem().unlockableName}!");
        }

        private void UnlockAllUnlockableSuits()
        {
            if (HUDManager.Instance == null) return;
            SuitUnlockables.ForEach(s => BuyUnlockableSuit(s, false));
            HUDManager.Instance.DisplayTip("Lethal Menu", $"Unlocked all suits!");
        }
    }
}
