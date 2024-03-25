using GameNetcodeStuff;
using LethalMenu.Manager;
using LethalMenu.Util;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace LethalMenu.Cheats
{
    internal class SpectatePlayer : Cheat
    {


        //Spectate
        public static int spectatingPlayer = -1;
        public static Camera camera = null;

        //MiniCam
        public static int camPlayer = -1;
        private static Camera minicam = null;
        private static RawImage minicamDisplay = null;
        private static Vector2 defaultTooltipPos = Vector2.zero;
        private static RectTransform tooltips = null;

        

        public override void Update()
        {
            try
            {
                StopSpectating();
                Spectate();
                DisplayMiniCam();
            }
            catch (Exception e)
            {
                LethalMenu.debugMessage = e.Message + "\n" + e.StackTrace;
            }
        }

        public override void OnGui()
        {
            if(Hack.SpectatePlayer.IsEnabled())
            {
                PlayerControllerB player = GetSpectatedPlayer();
                if (player == null) return;

                VisualUtil.DrawString(new Vector2(Screen.width / 2, 160f), "(Spectating: "+ player.playerUsername + ")", Settings.c_playerESP, true, true, false, 32);
            }

            if(Hack.MiniCam.IsEnabled())
            {
                PlayerControllerB player = GetCamPlayer();
                if (player == null) return;

                float x = minicamDisplay.rectTransform.localPosition.x + (minicamDisplay.rectTransform.sizeDelta.x/2);
                VisualUtil.DrawString(new Vector2(Screen.width - (x / 2), 15f), player.playerUsername, Settings.c_playerESP, true, true, false, 16);
            }
        }

        public static void Reset()
        {
            CameraManager.ActiveCamera = null;
            spectatingPlayer = -1;
            camPlayer = -1;
            
            if(camera != null) Destroy(camera.gameObject);
            if(minicam != null) Destroy(minicam.gameObject);
            if(minicamDisplay != null) Destroy(minicamDisplay.gameObject);

            camera = null;
            minicam = null;
            minicamDisplay = null;

            Hack.SpectatePlayer.SetToggle(false);
            Hack.MiniCam.SetToggle(false);
        }

        

        public static void StopSpectating()
        {
            //Stop Spectating Player
            if (!(bool)StartOfRound.Instance || (!Hack.SpectatePlayer.IsEnabled() && camera != null))
            {
                PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
                PlayerControllerB player = localPlayer.playersManager.allPlayerScripts[spectatingPlayer];

                camera.enabled = false;
                CameraManager.GetBaseCamera().enabled = true;
                CameraManager.ActiveCamera = !Hack.FreeCam.IsEnabled() ? CameraManager.GetBaseCamera() : Freecam.camera;
                spectatingPlayer = -1;
                Destroy(camera.gameObject);
                camera = null;
                GameUtil.RenderPlayerModels();
            }       

            if (!(bool)StartOfRound.Instance || (!Hack.MiniCam.IsEnabled() && minicam != null))
            {
                PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
                PlayerControllerB player = localPlayer.playersManager.allPlayerScripts[camPlayer];

                tooltips.anchoredPosition = defaultTooltipPos;
                tooltips = null;
                camPlayer = -1;
                minicamDisplay.transform.SetParent(null, false);
                defaultTooltipPos = Vector2.zero;
                Destroy(minicam.gameObject);
                Destroy(minicamDisplay.gameObject);
                minicam = null;
                minicamDisplay = null;
                GameUtil.RenderPlayerModels();

            }

        }

        private void Spectate()
        {
            if (!Hack.SpectatePlayer.IsEnabled()) return;

            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
            PlayerControllerB player = localPlayer.playersManager.allPlayerScripts[spectatingPlayer];

            if (camera == null)
            {
                camera = GameObjectUtil.CreateCamera("SpectateCamera", CameraManager.GetBaseCamera().transform, true);
                camera.enabled = true;
                CameraManager.GetBaseCamera().enabled = false;

                CameraManager.ActiveCamera = camera;
            }
            bool nv = Hack.NightVision.IsEnabled() || player.isInsideFactory;
            float intensity = Hack.NightVision.IsEnabled() ? Settings.f_nvIntensity : Settings.f_defaultNightVisionIntensity;
            float range = Hack.NightVision.IsEnabled() ? Settings.f_nvRange : Settings.f_defaultNightVisionRange;


            player.nightVision.enabled = nv;
            player.nightVision.intensity = intensity;
            player.nightVision.range = range;
            GameUtil.RenderPlayerModels();

            camera.transform.SetLocalPositionAndRotation(player.gameplayCamera.transform.localPosition, player.gameplayCamera.transform.localRotation);
            camera.transform.SetPositionAndRotation(player.gameplayCamera.transform.position, player.gameplayCamera.transform.rotation);
        }

        private void DisplayMiniCam()
        {
            if (!Hack.MiniCam.IsEnabled()) return;

            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
            PlayerControllerB player = localPlayer.playersManager.allPlayerScripts[camPlayer];

            if (minicam == null)
            {
                minicam = GameObjectUtil.CreateCamera("SpectateMiniCam", player.gameplayCamera.transform, false);
                minicamDisplay = GameObjectUtil.CreateMiniCamDisplay(minicam.targetTexture);
            }
            
            minicam.gameObject.transform.SetLocalPositionAndRotation(player.gameObject.transform.localPosition, player.gameObject.transform.localRotation);
            minicam.transform.SetPositionAndRotation(player.gameplayCamera.transform.position, player.gameplayCamera.transform.rotation);

            bool nv = Hack.NightVision.IsEnabled() || player.isInsideFactory;
            float intensity = Hack.NightVision.IsEnabled() ? Settings.f_nvIntensity : Settings.f_defaultNightVisionIntensity;
            float range = Hack.NightVision.IsEnabled() ? Settings.f_nvRange : Settings.f_defaultNightVisionRange;


            player.nightVision.enabled = nv;
            player.nightVision.intensity = intensity;
            player.nightVision.range = range;

            GameUtil.RenderPlayerModels();

            tooltips = HUDManager.Instance.Tooltips.canvasGroup.gameObject.GetComponent<RectTransform>();

            if (defaultTooltipPos == Vector2.zero) defaultTooltipPos = tooltips.anchoredPosition;

            tooltips.anchoredPosition -= new Vector2(0, 200);
        }

        public static bool isSpectatingPlayer(PlayerControllerB player)
        {
            if(spectatingPlayer == -1) return false;

            string name = StartOfRound.Instance.allPlayerScripts[spectatingPlayer].playerUsername;

            return spectatingPlayer == (int) player.playerClientId && name == player.playerUsername;
        }

        public static PlayerControllerB GetSpectatedPlayer()
        {
            if (spectatingPlayer == -1) return null;

            return StartOfRound.Instance.allPlayerScripts[spectatingPlayer];
        }

        public static bool isCamPlayer(PlayerControllerB player)
        {
            if (camPlayer == -1) return false;

            string name = StartOfRound.Instance.allPlayerScripts[camPlayer].playerUsername;

            return camPlayer == (int)player.playerClientId && name == player.playerUsername;
        }   

        public static PlayerControllerB GetCamPlayer()
        {
            if (camPlayer == -1) return null;

            return StartOfRound.Instance.allPlayerScripts[camPlayer];
        }


    }
}
