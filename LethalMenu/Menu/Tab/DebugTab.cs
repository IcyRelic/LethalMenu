using DunGen;
using LethalMenu.Handler;
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector2 = UnityEngine.Vector2;



namespace LethalMenu.Menu.Tab
{
    internal class DebugTab : MenuTab
    {
        public DebugTab() : base("Debug") { }

        private Vector2 scrollPos = Vector2.zero;

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
            GUILayout.Label("Goto Not Spawned");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                LethalMenu.localPlayer.TeleportPlayer(StartOfRound.Instance.notSpawnedPosition.position);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Spawn Turret");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                SpawnableMapObject spawnable = StartOfRound.Instance.currentLevel.spawnableMapObjects.FirstOrDefault(o => o.prefabToSpawn.name == "TurretContainer");

                //get a position in front of the LethalMenu.localPlayer
                Vector3 pos = LethalMenu.localPlayer.transform.position + LethalMenu.localPlayer.transform.forward * 2f;

                GameObject gameObject = Object.Instantiate<GameObject>(spawnable.prefabToSpawn, pos, Quaternion.identity, RoundManager.Instance.mapPropsContainer.transform);
                gameObject.transform.eulerAngles = !spawnable.spawnFacingAwayFromWall ? new Vector3(gameObject.transform.eulerAngles.x, (float)RoundManager.Instance.AnomalyRandom.Next(0, 360), gameObject.transform.eulerAngles.z) : new Vector3(0.0f, RoundManager.Instance.YRotationThatFacesTheFarthestFromPosition(pos + Vector3.up * 0.2f), 0.0f);
                gameObject.GetComponent<NetworkObject>().Spawn(true);

            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Spawn Landmine");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                SpawnableMapObject spawnable = StartOfRound.Instance.currentLevel.spawnableMapObjects.FirstOrDefault(o => o.prefabToSpawn.name == "Landmine");

                //get a position in front of the LethalMenu.localPlayer
                Vector3 pos = LethalMenu.localPlayer.transform.position + LethalMenu.localPlayer.transform.forward * 2f;

                GameObject gameObject = Object.Instantiate<GameObject>(spawnable.prefabToSpawn, pos, Quaternion.identity, RoundManager.Instance.mapPropsContainer.transform);
                gameObject.transform.eulerAngles = !spawnable.spawnFacingAwayFromWall ? new Vector3(gameObject.transform.eulerAngles.x, (float)RoundManager.Instance.AnomalyRandom.Next(0, 360), gameObject.transform.eulerAngles.z) : new Vector3(0.0f, RoundManager.Instance.YRotationThatFacesTheFarthestFromPosition(pos + Vector3.up * 0.2f), 0.0f);
                gameObject.GetComponent<NetworkObject>().Spawn(true);

            }
            GUILayout.EndHorizontal();


            GUILayout.BeginHorizontal();
            GUILayout.Label("Spawnable Map Objects");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Execute"))
            {
                GameUtil.GetSpawnableMapObjects().ForEach(o =>
                {
                    LethalMenu.debugMessage += o.prefabToSpawn.name + " => " + o.prefabToSpawn.GetType() + " => " + o.prefabToSpawn.tag + "\n";       
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
