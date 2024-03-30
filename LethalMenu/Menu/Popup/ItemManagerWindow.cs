using LethalMenu.Menu.Core;
using LethalMenu.Util;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LethalMenu.Menu.Popup;

internal class ItemManagerWindow(int id) : PopupMenu("Item Manager", new Rect(50f, 50f, 577f, 300f), id)
{
    private Vector2 _scrollPosition = Vector2.zero;
    private string _sScrapValue = "1000";
    private string _sSearch = "";

    protected override void DrawContent(int windowID)
    {
        ItemContent();
    }

    private void ItemContent()
    {
        if (!(bool)StartOfRound.Instance) return;

        if (!LethalMenu.LocalPlayer.IsHost)
        {
            UI.Label("General.HostRequired", Settings.c_error);
            return;
        }

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        GUILayout.BeginHorizontal();
        UI.Textbox("General.Search", ref _sSearch);
        GUILayout.FlexibleSpace();
        UI.Textbox("ItemManager.ScrapValue", ref _sScrapValue, @"[^0-9]");
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        UI.ButtonGrid(StartOfRound.Instance.allItemsList.itemsList, i => i.name, _sSearch, SpawnItem, 3);

        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    private void SpawnItem(Item item)
    {
        var position = GameNetworkManager.Instance.localPlayerController.playerEye.transform.position;
        var gameObject = Object.Instantiate(item.spawnPrefab, position, Quaternion.identity,
            StartOfRound.Instance.propsContainer);
        int value = int.TryParse(_sScrapValue, out value) ? value : Random.Range(15, 100);

        gameObject.GetComponent<GrabbableObject>().SetScrapValue(value);
        gameObject.GetComponent<GrabbableObject>().fallTime = 0.0f;
        gameObject.GetComponent<NetworkObject>().Spawn();
    }
}