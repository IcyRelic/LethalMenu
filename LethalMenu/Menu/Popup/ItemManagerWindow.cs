using GameNetcodeStuff;
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
            PlayerControllerB? localPlayer = LethalMenu.localPlayer;
            if (HUDManager.Instance == null || localPlayer == null) return;
            int value = int.TryParse(s_scrapValue, out value) ? value : Random.Range(15, 100);
            int amount = int.TryParse(s_amount, out amount) ? amount : 1;
            for (int i = 0; i < amount; i++)
            {
                Vector3 playerEyePosition = localPlayer.playerEye.transform.position;
                GameObject gameObject = Object.Instantiate(item.spawnPrefab, playerEyePosition, Quaternion.identity, StartOfRound.Instance.propsContainer);
                GrabbableObject grabbableObject = gameObject.GetComponent<GrabbableObject>();
                grabbableObject.SetScrapValue(value);
                gameObject.GetComponent<NetworkObject>().Spawn();
                if (localPlayer.isInElevator) grabbableObject.transform.SetParent(StartOfRound.Instance.elevatorTransform, true);
                else grabbableObject.transform.SetParent(StartOfRound.Instance.propsContainer, true);
                localPlayer.SetItemInElevator(localPlayer.isInHangarShipRoom, localPlayer.isInElevator, grabbableObject);
                Vector3 localGrabbableObjectPosition = grabbableObject.transform.parent.InverseTransformPoint(playerEyePosition);
                grabbableObject.startFallingPosition = localGrabbableObjectPosition;
                grabbableObject.targetFloorPosition = localGrabbableObjectPosition;
                grabbableObject.EnablePhysics(true);
            }
            HUDManager.Instance.DisplayTip("Lethal Menu", $"Spawned {amount} {item.itemName}{(amount == 1 ? "" : "s")} ({value})!");
        }
    }
}