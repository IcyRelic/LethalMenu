using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LethalMenu.Menu.Core
{
    internal class PopupMenu : MenuFragment
    {
        public Rect windowRect;
        public bool isOpen = false;
        public string name;
        public int id;

        protected List<MenuTab> menuTabs = new List<MenuTab>();
        protected int selectedTab = 0;

        public PopupMenu(string name, Rect size, int id)
        {
            this.name = name;
            this.windowRect = size;
            this.id = id;
        }

        public void Draw()
        {
            if (!isOpen) return;
            windowRect = GUILayout.Window(id, windowRect, new GUI.WindowFunction(DrawContent), name);
        }

        public virtual void DrawContent(int windowID) { }

    }
}
