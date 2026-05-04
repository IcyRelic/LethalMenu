using GameNetcodeStuff;
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using Steamworks.Ugc;
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

            UI.ButtonGrid(LethalMenu.items.Where(i => i != null && !i.isHeld && !i.isPocketed && (Settings.b_ShowShipItems || !i.isInShipRoom)).GroupBy(i => i.itemProperties.itemName).Select(g => g.First()).ToList(), (i) => $"{i.itemProperties.itemName} {LethalMenu.items.Count(ii => ii.itemProperties.itemName == i.itemProperties.itemName)}x", s_search, TeleportItem, 3);

            GUILayout.EndScrollView();
            GUI.DragWindow();
        }

        private void TeleportItem(GrabbableObject grabbableObject)
        {
            PlayerControllerB? localPlayer = LethalMenu.localPlayer;
            if (HUDManager.Instance == null || localPlayer == null) return;
            if (grabbableObject is LungProp lung && lung.isLungDocked) lung.EquipItem();
            if (localPlayer.isInElevator) grabbableObject.transform.SetParent(StartOfRound.Instance.elevatorTransform, true);
            else grabbableObject.transform.SetParent(StartOfRound.Instance.propsContainer, true);
            localPlayer.SetItemInElevator(localPlayer.isInHangarShipRoom, localPlayer.isInElevator, grabbableObject);
            Vector3 localGrabbableObjectPosition = grabbableObject.transform.parent.InverseTransformPoint(localPlayer.playerEye.transform.position);
            grabbableObject.startFallingPosition = localGrabbableObjectPosition;
            grabbableObject.targetFloorPosition = localGrabbableObjectPosition;
            grabbableObject.EnablePhysics(true);
            grabbableObject.FallToGround();
            HUDManager.Instance.DisplayTip("Lethal Menu", $"Teleported {grabbableObject.itemProperties.itemName} ( {grabbableObject.scrapValue} )!");
        }
    }
}