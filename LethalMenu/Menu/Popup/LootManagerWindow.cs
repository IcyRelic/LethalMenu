using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System.Linq;
using UnityEngine;

namespace LethalMenu.Menu.Popup
{
    internal class LootManagerWindow : PopupMenu
    {
        private string s_search = "";
        private Vector2 scrollPos = Vector2.zero;

        public LootManagerWindow(int id) : base("LootManager.Title", new Rect(50f, 50f, 577f, 300f), id) { }

        public override void DrawContent(int windowID)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.BeginHorizontal();
            UI.Textbox("General.Search", ref s_search);
            UI.Toggle("LootManager.ShowShipItems", ref Settings.b_ShowShipItems, "General.Enable", "General.Disable");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            UI.ButtonGrid(LethalMenu.items.Where(i => i != null && !i.isHeld && !i.isPocketed).GroupBy(i => i.itemProperties.itemName).Select(g => g.First()).ToList(), (i) => $"{i.itemProperties.itemName} {LethalMenu.items.Count(ii => ii.itemProperties.itemName == i.itemProperties.itemName)}x", s_search, TeleportItem, 3);

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        private void TeleportItem(GrabbableObject grabbableObject)
        {
            if (CameraManager.ActiveCamera == null || HUDManager.Instance == null || StartOfRound.Instance == null) return;
            Vector3 position = grabbableObject.GetItemFloorPosition(CameraManager.ActiveCamera.transform.position);
            grabbableObject.targetFloorPosition = !StartOfRound.Instance.shipBounds.bounds.Contains(position) ? StartOfRound.Instance.propsContainer.InverseTransformPoint(position) : StartOfRound.Instance.elevatorTransform.InverseTransformPoint(position);
            HUDManager.Instance.DisplayTip("Lethal Menu", $"Teleported {grabbableObject.itemProperties.itemName} ( {grabbableObject.scrapValue} )!");
        }
    }
}