using LethalMenu.Language;
using System.Collections.Generic;
using UnityEngine;

namespace LethalMenu.Menu.Core
{
    internal class PopupMenu : MenuFragment
    {
        public Rect windowRect;
        public bool isOpen = false;
        public string name;
        private string localization;
        public int id;

        protected List<MenuTab> menuTabs = new List<MenuTab>();
        protected int selectedTab = 0;

        public PopupMenu(string name, Rect size, int id)
        {
            this.localization = name;
            this.windowRect = size;
            this.id = id;
            LocalizeName();
        }

        public void LocalizeName() => name = Localization.Localize(localization);
        public void Draw()
        {
            if (!isOpen) return;
            windowRect = GUILayout.Window(id, windowRect, new GUI.WindowFunction(DrawContent), name);
        }

        public virtual void DrawContent(int windowID) { }

    }
}
