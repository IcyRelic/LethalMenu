using System.Collections.Generic;
using System.Linq;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Popup;

internal class LootManager : PopupMenu
{
    private readonly Dictionary<string, int> items = new();
    private string s_search = "";
    private Vector2 scrollPos = Vector2.zero;

    public LootManager(int id) : base("Loot Manager", new Rect(50f, 50f, 577f, 300f), id)
    {
    }

    protected override void DrawContent(int windowID)
    {
        if (!(bool)StartOfRound.Instance) return;

        items.Clear();
        foreach (var item in LethalMenu.items.Where(item => !item.isInShipRoom && !item.isHeld && !item.isPocketed))
            items[item.name] = items.ContainsKey(item.name) ? items[item.name] + 1 : 1;

        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginHorizontal();
        UI.Textbox("General.Search", ref s_search);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        UI.ButtonGrid(items.Keys.ToList(), itemName => itemName.Replace("(Clone)", "") + " x" + items[itemName],
            s_search, itemName =>
            {
                var itemToTeleport = LethalMenu.items
                    .Where(i => !i.isHeld && !i.isPocketed && !i.isInShipRoom && i.name == itemName)
                    .OrderBy(_ => Random.value)
                    .FirstOrDefault();

                if (itemToTeleport != null)
                {
                    var point = LethalMenu.localPlayer.gameplayCamera.transform.position +
                                LethalMenu.localPlayer.gameplayCamera.transform.forward * 1f;

                    itemToTeleport.gameObject.transform.position = point;
                    itemToTeleport.startFallingPosition = point;
                    itemToTeleport.targetFloorPosition = point;

                    if (items.ContainsKey(itemName))
                    {
                        items[itemName]--;
                        if (items[itemName] == 0)
                            items.Remove(itemName);
                    }
                }
            }, 3);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }
}