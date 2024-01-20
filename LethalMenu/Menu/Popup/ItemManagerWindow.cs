using LethalMenu.Cheats;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LethalMenu.Menu.Popup
{
    internal class ItemManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;

        private string s_scrapValue = "1000";
        private string s_search = "";

        public ItemManagerWindow(int id) : base("Item Manager", new Rect(50f, 50f, 577f, 300f), id)
        {

        }

        public override void DrawContent(int windowID)
        {
            ItemContent();

            GUI.DragWindow(new Rect(0.0f, 0.0f, 10000f, 45f));
        }

        private void ItemContent()
        {
            if(!(bool)StartOfRound.Instance) return;

            if (!LethalMenu.localPlayer.IsHost)
            {
                UI.Label("General.HostRequired", Settings.c_error);
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.BeginHorizontal();
            UI.Textbox("General.Search", ref s_search);
            GUILayout.FlexibleSpace();
            UI.Textbox("ItemManager.ScrapValue", ref s_scrapValue, @"[^0-9]");
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            UI.ButtonGrid(StartOfRound.Instance.allItemsList.itemsList, (i) => i.name, s_search, (i) => SpawnItem(i), 3);

            GUILayout.EndScrollView();
        }

        private void SpawnItem(Item item)
        {
            Vector3 position = GameNetworkManager.Instance.localPlayerController.playerEye.transform.position;
            GameObject gameObject = Object.Instantiate(item.spawnPrefab, position, Quaternion.identity, StartOfRound.Instance.propsContainer);
            int value = int.TryParse(s_scrapValue, out value) ? value : Random.Range(15, 100);

            gameObject.GetComponent<GrabbableObject>().SetScrapValue(value);
            gameObject.GetComponent<GrabbableObject>().fallTime = 0.0f;
            gameObject.GetComponent<NetworkObject>().Spawn();
        }
    }
}
