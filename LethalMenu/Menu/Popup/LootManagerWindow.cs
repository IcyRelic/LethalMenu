using LethalMenu.Menu.Core;
using LethalMenu.Util;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LethalMenu.Menu.Popup
{
    internal class LootManagerWindow : PopupMenu
    {
        private string s_search = "";
        private readonly Dictionary<string, int> items = new Dictionary<string, int>();
        private Vector2 scrollPos = Vector2.zero;

        public LootManagerWindow(int id) : base("LootManager.Title", new Rect(50f, 50f, 577f, 300f), id) { }

        public override void DrawContent(int windowID)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginHorizontal();
            UI.Textbox("General.Search", ref s_search);
            UI.Toggle("LootManager.ShowShipItems", ref Settings.b_ShowShipItems, "General.Disable", "General.Enable");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            foreach (var item in LethalMenu.items.Where(item => (Settings.b_ShowShipItems || !item.isInShipRoom) && !item.isHeld && !item.isPocketed)) items[item.name] = items.ContainsKey(item.name) ? items[item.name] + 1 : 1;

            UI.ButtonGrid(items.Keys.ToList(), (string itemname) => itemname.Replace("(Clone)", "") + " x" + items[itemname], s_search, TeleportItem, 3);

            items.Clear();

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        private void TeleportItem(string itemname)
        {
            GrabbableObject i = LethalMenu.items.FirstOrDefault(item => item.name == itemname && (Settings.b_ShowShipItems || !item.isInShipRoom) && !item.isHeld && !item.isPocketed);
            if (i != null)
            {
                string itemName = itemname.Replace("(Clone)", ""); ;
                HUDManager.Instance.DisplayTip("Lethal Menu", $"Teleported Item: {itemName}! Item Worth: {i.scrapValue}!");
                Vector3 point = new Ray(LethalMenu.localPlayer.gameplayCamera.transform.position, LethalMenu.localPlayer.gameplayCamera.transform.forward).GetPoint(1f);
                i.gameObject.transform.position = point;
                i.startFallingPosition = point;
                if (!i.isInShipRoom)
                {
                    i.targetFloorPosition = point;
                }
                else if (i.isInShipRoom)
                {
                    i.targetFloorPosition = point + new Vector3(0, 0, 7);
                }
                else if (i.name == "LungApparatus(Clone)")
                {
                    i.targetFloorPosition = point + new Vector3(14, 29, -13);
                }
                if (items.ContainsKey(itemname))
                {
                    items[itemname] = items[itemname] - 1;
                    if (items[itemname] == 0)
                    {
                        items.Remove(itemname);
                    }
                }
            }
        }
    }
}