using GameNetcodeStuff;
using LethalMenu.Components;
using LethalMenu.Manager;
using LethalMenu.Util;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalMenu.Cheats
{
    internal class Freecam : Cheat
    {
        //FreeCam
        public static Camera camera = null;
        private static Light light = null;
        public static MouseInput mouse = null;
        public static KBInput movement = null;
        public static bool isStatic = false;


        public override void Update()
        {
            try
            {
                Stop();

                if (!Hack.FreeCam.IsEnabled()) return;
                isStatic = Hack.EnemyControl.IsEnabled();
                LethalMenu.localPlayer.enabled = false;
                LethalMenu.localPlayer.isFreeCamera = true;
                CreateIfNull();
                light.intensity = Settings.f_nvIntensity;
                light.range = Settings.f_nvRange;
                GameUtil.RenderPlayerModels();
                if(!isStatic) camera.transform.SetPositionAndRotation(movement.transform.position, mouse.transform.rotation);
            }
            catch (Exception e)
            {
                LethalMenu.debugMessage = e.Message + "\n" + e.StackTrace;
            }
        }

        private void CreateIfNull()
        {
            if (camera == null)
            {
                camera = GameObjectUtil.CreateCamera("FreeCam", CameraManager.GetBaseCamera().transform);
                camera.enabled = true;
                mouse = camera.gameObject.AddComponent<MouseInput>();
                movement = camera.gameObject.AddComponent<KBInput>();
                light = GameObjectUtil.CreateLight();
                light.transform.SetParent(camera.transform, false);
                CameraManager.GetBaseCamera().enabled = false;
                CameraManager.ActiveCamera = camera;
            }
        }

        public static void Reset()
        {
            if (camera != null) Destroy(camera.gameObject);
            if (light != null) Destroy(light.gameObject);

            camera = null;
            light = null;

            Hack.FreeCam.SetToggle(false);
            Stop();
        }

        public static void Stop()
        {
            if (!(bool) StartOfRound.Instance || Hack.FreeCam.IsEnabled() || camera == null) return;
            CameraManager.ActiveCamera = SpectatePlayer.spectatingPlayer == -1 ? CameraManager.GetBaseCamera() : SpectatePlayer.camera;
            CameraManager.GetBaseCamera().enabled = true;

            camera.enabled = false;
            Destroy(camera.gameObject);
            camera = null;
            mouse = null;
            movement = null;


            Destroy(light.gameObject);
            light = null;

            LethalMenu.localPlayer.enabled = true;
            LethalMenu.localPlayer.isFreeCamera = false;
            GameUtil.RenderPlayerModels();
        }
    }
}
