using LethalMenu.Handler;
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System.Linq;
using UnityEngine;
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
            GUILayout.Label("Get Object");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                GrabbableObject grabbableObject = Object.FindAnyObjectByType<GrabbableObject>();
                Transform parentTransform = grabbableObject.transform.parent;

                if (parentTransform != null)
                {
                    GameObject parentGameObject = parentTransform.gameObject;
                    Debug.LogError("Parent GameObject: " + parentGameObject.name);
                }
                else
                {
                    Debug.LogError("This GameObject has no parent.");
                }




            }



            GUILayout.EndHorizontal();



            GUILayout.BeginHorizontal();
            GUILayout.Label("Spawnable Map Objects");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                StartOfRound.Instance.currentLevel.spawnableMapObjects.ToList().ForEach(o =>
                {
                    LethalMenu.debugMessage += o.prefabToSpawn.name + " => " + o.prefabToSpawn.GetType() + " => " + o.prefabToSpawn.tag + "\n";
                    foreach (var item in o.prefabToSpawn.GetComponentsInChildren<MeshRenderer>())
                    {
                        LethalMenu.debugMessage += item.name + " => " + item.GetType() + "\n";
                    }

                    
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

            GUILayout.BeginHorizontal();
            GUILayout.Label("Force Cham Every Renderer");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                Object.FindObjectsOfType<Renderer>().ToList().ForEach(r => r.GetChamHandler().ApplyCham());
            }



            GUILayout.EndHorizontal();


            GUILayout.EndScrollView();
        }

    }
}
