using Dissonance.Integrations.Unity_NFGO;
using LethalMenu.Cheats;
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Vector2 = UnityEngine.Vector2;



namespace LethalMenu.Menu.Tab
{
    internal class DebugTab : MenuTab
    {
        public DebugTab() : base("Debug") { }

        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;
        private string s_amount = "";
        private bool b_outside = false;


        private int selectedItem = 65;

        public override void Draw()
        {
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
            MenuContent();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));

            GUILayout.EndVertical();
        }

        private void MenuContent()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            if (GUILayout.Button("Clear Debug Message"))
            {
                LethalMenu.debugMessage = "";
                LethalMenu.debugMessage2 = "";
            }
            GUILayout.TextArea(LethalMenu.debugMessage, GUILayout.Height(50));
            GUILayout.TextArea(LethalMenu.debugMessage2, GUILayout.Height(50));


            GUILayout.Label("Debug Menu");



            GUILayout.BeginHorizontal();
            GUILayout.Label("Get Turret");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                Turret turret = Object.FindAnyObjectByType<Turret>();

                turret.GetComponentsInParent<Component>().ToList().ForEach(c =>
                {
                    LethalMenu.debugMessage += c.GetType().Name + "\n";
                });

                turret.GetComponentsInChildren<Component>().ToList().ForEach(c =>
                {
                    LethalMenu.debugMessage2 += c.GetType().Name + "\n";
                });
            }



            GUILayout.EndHorizontal();

   





            GUILayout.BeginHorizontal();
            GUILayout.Label("Raycast Colliders");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                foreach (RaycastHit hit in CameraManager.ActiveCamera.transform.SphereCastForward())
                {
                    Collider collider = hit.collider;

                    LethalMenu.debugMessage += "Hit: " + collider.name + " =>" + collider.gameObject.name + "\n";
                }
            }



            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Garage");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                LethalMenu.interactTriggers.ForEach(t =>
                {
                    if (t == null || t.name != "Cube" || t.transform.parent.name != "Cutscenes") return;

                    t.randomChancePercentage = 100;
                    t.Interact(LethalMenu.localPlayer.transform);

                });
            }



            GUILayout.EndHorizontal();


            GUILayout.EndScrollView();
        }

    }
}
