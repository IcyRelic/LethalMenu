using LethalMenu.Menu.Core;
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
            if (!LethalMenu.localPlayer.IsHost)
            {
                GUILayout.Label("<color=#DD0B0B>This feature requires host!</color>");
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Scrap Value: ");
            GUILayout.FlexibleSpace();

            s_scrapValue = GUILayout.TextField(s_scrapValue, GUILayout.Width(Settings.GUISize.GetTextboxWidth()));
            s_scrapValue = Regex.Replace(s_scrapValue, @"[^0-9]", "");

            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            if ((bool)StartOfRound.Instance)
            {
                var items = StartOfRound.Instance.allItemsList.itemsList;
                int itemsPerRow = 3;
                int rows = Mathf.CeilToInt(items.Count / (float)itemsPerRow);

                for (int i = 0; i < rows; i++)
                {
                    GUILayout.BeginHorizontal();
                    for (int j = 0; j < itemsPerRow; j++)
                    {
                        int index = i * itemsPerRow + j;
                        if (index >= items.Count) break;
                        var item = items[index];

                        if (GUILayout.Button(item.name, GUILayout.Width(175))) SpawnItem(item);
                    }
                    GUILayout.EndHorizontal();
                }

            }


            GUILayout.EndScrollView();
        }

        private void SpawnItem(Item item)
        {
            Vector3 position = GameNetworkManager.Instance.localPlayerController.playerEye.transform.position;
            GameObject gameObject = Object.Instantiate(item.spawnPrefab, position, Quaternion.identity, StartOfRound.Instance.propsContainer);


            int value = int.TryParse(s_scrapValue, out value) ? value : 1000;


            gameObject.GetComponent<GrabbableObject>().SetScrapValue(value);
            gameObject.GetComponent<GrabbableObject>().fallTime = 0.0f;
            gameObject.GetComponent<NetworkObject>().Spawn();
        }
    }
}
