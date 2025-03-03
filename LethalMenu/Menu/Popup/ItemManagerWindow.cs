using LethalMenu.Handler;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LethalMenu.Menu.Popup
{
    internal class ItemManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;
        private string s_scrapValue = "1000";
        private string s_amount = "1";
        private string s_search = "";

        public ItemManagerWindow(int id) : base("ItemManager.Title", new Rect(50f, 50f, 575f, 300f), id) { }

        public override void DrawContent(int windowID)
        {
            if (LethalMenu.localPlayer == null || StartOfRound.Instance == null)
            {
                UI.Label("General.NullError", Settings.c_error);
                GUI.DragWindow();
                return;
            }

            if (!LethalMenu.localPlayer.IsHost())
            {
                UI.Label("General.HostRequired", Settings.c_error);
                GUI.DragWindow();
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.BeginHorizontal();
            UI.Textbox("General.Search", ref s_search, "", 0, false);
            GUILayout.FlexibleSpace();
            UI.Textbox("ItemManager.ScrapValue", ref s_scrapValue, @"[^0-9]", 0, false);
            UI.Textbox("ItemManager.Amount", ref s_amount, @"[^0-9]", 0, false);
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            UI.ButtonGrid(StartOfRound.Instance.allItemsList.itemsList.Where(i => i != null && i.spawnPrefab != null).ToList(), (i) => i.name, s_search, (i) => SpawnItem(i), 3);

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        private void SpawnItem(Item item)
        {
            if (HUDManager.Instance == null) return;
            int value = int.TryParse(s_scrapValue, out value) ? value : Random.Range(15, 100);
            int amount = int.TryParse(s_amount, out amount) ? amount : 1;
            for (int i = 0; i < amount; i++)
            {
                GameObject gameObject = Object.Instantiate(item.spawnPrefab, LethalMenu.localPlayer.playerEye.transform.position, Quaternion.identity, StartOfRound.Instance.propsContainer);
                gameObject.GetComponent<GrabbableObject>().SetScrapValue(value);
                gameObject.GetComponent<GrabbableObject>().fallTime = 0.0f;
                gameObject.GetComponent<NetworkObject>().Spawn();
            }
            HUDManager.Instance.DisplayTip("Lethal Menu", $"Spawned {amount} {item.itemName}{(amount == 1 ? "" : "s")} ({value})!");
        }
    }
}