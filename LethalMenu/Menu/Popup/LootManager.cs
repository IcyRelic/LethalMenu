using System.Collections.Generic;
using System.Linq;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Popup;

internal class LootManager(int id) : PopupMenu("Loot Manager", new Rect(50f, 50f, 577f, 300f), id)
{
    private readonly Dictionary<string, int> _items = new();
    private Vector2 _scrollPosition = Vector2.zero;
    private string _sSearch = "";

    protected override void DrawContent(int windowID)
    {
        if (!(bool)StartOfRound.Instance) return;

        _items.Clear();
        foreach (var item in LethalMenu.Items.Where(item => !item.isInShipRoom && !item.isHeld && !item.isPocketed))
            _items[item.name] = _items.ContainsKey(item.name) ? _items[item.name] + 1 : 1;

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        GUILayout.BeginHorizontal();
        UI.Textbox("General.Search", ref _sSearch);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        UI.ButtonGrid(_items.Keys.ToList(), itemName => itemName.Replace("(Clone)", "") + " x" + _items[itemName],
            _sSearch, itemName =>
            {
                var itemToTeleport = LethalMenu.Items
                    .Where(i => !i.isHeld && !i.isPocketed && !i.isInShipRoom && i.name == itemName)
                    .OrderBy(_ => Random.value)
                    .FirstOrDefault();

                if (!itemToTeleport) return;
                var point = LethalMenu.LocalPlayer.gameplayCamera.transform.position +
                            LethalMenu.LocalPlayer.gameplayCamera.transform.forward * 1f;

                itemToTeleport.gameObject.transform.position = point;
                itemToTeleport.startFallingPosition = point;
                itemToTeleport.targetFloorPosition = point;

                if (!_items.ContainsKey(itemName)) return;

                _items[itemName]--;
                if (_items[itemName] == 0)
                    _items.Remove(itemName);
            }, 3);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }
}