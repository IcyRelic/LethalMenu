using GameNetcodeStuff;
using LethalMenu.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine;
using LethalMenu.Util;

namespace LethalMenu.Cheats
{
    internal class EnemyControl : Cheat
    {

        private static Camera camera = null;
        private static Light light = null;
        private static EnemyAI enemy = null;

        public static void Control(EnemyAI enemy)
        {
            if(enemy.isEnemyDead) return;
            EnemyControl.enemy = enemy;
        }

        public static void StopControl()
        {
            if (!Hack.EnemyControl.IsEnabled() && enemy != null)
            {
                if (camera != null) GameObject.Destroy(camera.gameObject);
                if (light != null) GameObject.Destroy(light.gameObject);

                camera = null;
                light = null;

                enemy.moveTowardsDestination = true;
                enemy.updatePositionThreshold = 1f;

                enemy.GetComponentsInChildren<Collider>().ToList().ForEach(c => c.enabled = true);
                LethalMenu.localPlayer.GetComponent<CharacterController>().enabled = true;

                enemy = null;

                CameraManager.GetBaseCamera().enabled = true;
                CameraManager.ActiveCamera = CameraManager.GetBaseCamera();
            }

            

        }

        public override void Update()
        {
            StopControl();

            


            ControlEnemy();


        }


        


        private void ControlEnemy()
        {

            if (enemy == null) return;

            if (Hack.SpectatePlayer.IsEnabled() || Hack.FreeCam.IsEnabled())
            {
                Hack.SpectatePlayer.SetToggle(false);
                Hack.FreeCam.SetToggle(false);
                SpectatePlayer.StopSpectating();
            }

            if (!enemy.IsOwner) enemy.ChangeEnemyOwnerServerRpc(LethalMenu.localPlayer.actualClientId);


            enemy.updatePositionThreshold = 0f;


            //enemy.GetComponentsInChildren<Collider>().ToList().ForEach(c => c.enabled = false);

            if (camera == null)
            {
                camera = GameObjectUtil.CreateCamera("EnemyControlCam", CameraManager.GetBaseCamera().transform);
                camera.enabled = true;
                light = GameObjectUtil.CreateLight();
                light.transform.SetParent(camera.transform, false);
                CameraManager.GetBaseCamera().enabled = false;
                CameraManager.ActiveCamera = camera;

                //set the position of the camera
                camera.transform.position = enemy.transform.position - (enemy.transform.forward * 3f) + (enemy.transform.up * 1.5f);
            }

            light.intensity = Settings.f_nvIntensity;
            light.range = Settings.f_nvRange;

            LethalMenu.localPlayer.GetComponent<CharacterController>().enabled = false;

            Vector3 vector3 = new Vector3();
            if (Keyboard.current.wKey.isPressed) vector3 += enemy.transform.forward;
            if (Keyboard.current.sKey.isPressed) vector3 -= enemy.transform.forward;
            if (Keyboard.current.aKey.isPressed) vector3 -= enemy.transform.right;
            if (Keyboard.current.dKey.isPressed) vector3 += enemy.transform.right;
            if (Keyboard.current.spaceKey.isPressed) vector3 += enemy.transform.up;
            if (Keyboard.current.ctrlKey.isPressed) vector3 -= enemy.transform.up;

            Vector3 localPosition = enemy.transform.position;
            Vector3 vector3_2 = localPosition + vector3 * (Settings.f_movementSpeed * Time.deltaTime);
            Vector3 camPos = vector3_2 - (enemy.transform.forward * 2f) + (enemy.transform.up * 3f);

            camera.transform.SetPositionAndRotation(camPos, LethalMenu.localPlayer.gameplayCamera.transform.rotation);

            if (vector3.Equals(Vector3.zero)) return;

            
            enemy.transform.SetPositionAndRotation(vector3_2, LethalMenu.localPlayer.gameplayCamera.transform.rotation);
            enemy.moveTowardsDestination = false;
            enemy.TargetClosestPlayer();
            

            enemy.SetDestinationToPosition(vector3_2);
            enemy.SyncPositionToClients();
        }
    }
}