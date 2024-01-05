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
        private static Camera spectateCamera = null;

        //MiniCam
        public static int camPlayer = -1;
        private static Camera miniCam = null;
        private static RawImage miniCamDisplay = null;
        private static Vector2 defaultTooltipPos = Vector2.zero;
        private static RectTransform tooltips = null;

        //FreeCam
        private static Camera freeCam = null;
        private static Light freeCamLight = null;

        public override void Update()
        {
            try
            {
                StopSpectating();
                Spectate();
                DisplayMiniCam();
                FreeCam();

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

                float x = miniCamDisplay.rectTransform.localPosition.x + (miniCamDisplay.rectTransform.sizeDelta.x/2);
                VisualUtil.DrawString(new Vector2(Screen.width - (x / 2), 15f), player.playerUsername, Settings.c_playerESP, true, true, false, 16);
            }
        }

        public static void Reset()
        {
            CameraManager.ActiveCamera = null;
            spectatingPlayer = -1;
            camPlayer = -1;
            

            if(freeCam != null) Destroy(freeCam.gameObject);
            if (freeCamLight != null) Destroy(freeCamLight.gameObject);
            if(spectateCamera != null) Destroy(spectateCamera.gameObject);
            if(miniCam != null) Destroy(miniCam.gameObject);
            if(miniCamDisplay != null) Destroy(miniCamDisplay.gameObject);

            freeCam = null;
            freeCamLight = null;
            spectateCamera = null;
            miniCam = null;
            miniCamDisplay = null;


            Hack.SpectatePlayer.SetToggle(false);
            Hack.MiniCam.SetToggle(false);
            Hack.FreeCam.SetToggle(false);
        }

        

        public static void StopSpectating()
        {
            //Stop Spectating Player
            if (!(bool)StartOfRound.Instance || (!Hack.SpectatePlayer.IsEnabled() && spectateCamera != null))
            {
                PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
                PlayerControllerB player = localPlayer.playersManager.allPlayerScripts[spectatingPlayer];

                spectateCamera.enabled = false;
                CameraManager.GetBaseCamera().enabled = true;
                CameraManager.ActiveCamera = !Hack.FreeCam.IsEnabled() ? CameraManager.GetBaseCamera() : freeCam;
                spectatingPlayer = -1;
                Destroy(spectateCamera.gameObject);
                spectateCamera = null;
                RenderPlayerModels();
            }

            //FreeCam
            if (!(bool)StartOfRound.Instance || (!Hack.FreeCam.IsEnabled() && freeCam != null))
            {
                PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
                freeCam.enabled = false;
                CameraManager.GetBaseCamera().enabled = true;
                CameraManager.ActiveCamera = spectatingPlayer == -1 ? CameraManager.GetBaseCamera() : spectateCamera;

                localPlayer.GetComponent<CharacterController>().enabled = true;

                Destroy(freeCam.gameObject);
                Destroy(freeCamLight.gameObject);
                freeCamLight = null;
                freeCam = null;
                RenderPlayerModels();
            }

            if (!(bool)StartOfRound.Instance || (!Hack.MiniCam.IsEnabled() && miniCam != null))
            {
                PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
                PlayerControllerB player = localPlayer.playersManager.allPlayerScripts[camPlayer];

                tooltips.anchoredPosition = defaultTooltipPos;
                tooltips = null;
                camPlayer = -1;
                miniCamDisplay.transform.SetParent(null, false);
                defaultTooltipPos = Vector2.zero;
                Destroy(miniCam.gameObject);
                Destroy(miniCamDisplay.gameObject);
                miniCam = null;
                miniCamDisplay = null;
                RenderPlayerModels();

            }

        }

        private void FreeCam()
        {
            if(!Hack.FreeCam.IsEnabled()) return;

            if(spectatingPlayer != -1)
            {
                Hack.SpectatePlayer.SetToggle(false);
                StopSpectating();
            }

            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;

            


            if (freeCam == null)
            {
                freeCam = GameObjectUtil.CreateCamera("FreeCam", CameraManager.GetBaseCamera().transform);
                freeCam.enabled = true;
                freeCamLight = GameObjectUtil.CreateLight();
                


                freeCamLight.transform.SetParent(freeCam.transform, false);
                CameraManager.GetBaseCamera().enabled = false;
                

                CameraManager.ActiveCamera = freeCam;
            }

            
            freeCamLight.intensity = Settings.f_nvIntensity;
            freeCamLight.range = Settings.f_nvRange;

            localPlayer.GetComponent<CharacterController>().enabled = false;

            RenderPlayerModels();

            Vector3 vector3 = new Vector3();
            if (Keyboard.current.wKey.isPressed) vector3 += freeCam.transform.forward;
            if (Keyboard.current.sKey.isPressed) vector3 -= freeCam.transform.forward;
            if (Keyboard.current.aKey.isPressed) vector3 -= freeCam.transform.right;
            if (Keyboard.current.dKey.isPressed) vector3 += freeCam.transform.right;
            if (Keyboard.current.spaceKey.isPressed) vector3 += freeCam.transform.up;
            if (Keyboard.current.ctrlKey.isPressed) vector3 -= freeCam.transform.up;

            Vector3 localPosition = freeCam.transform.position;
            if (localPosition.Equals(Vector3.zero)) return;

            Vector3 vector3_2 = localPosition + vector3 * (Settings.f_noclipSpeed * Time.deltaTime);

            freeCam.transform.position = vector3_2;
            freeCam.transform.SetPositionAndRotation(vector3_2, localPlayer.gameplayCamera.transform.rotation);
        }

        private void Spectate()
        {
            if (!Hack.SpectatePlayer.IsEnabled()) return;

            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
            PlayerControllerB player = localPlayer.playersManager.allPlayerScripts[spectatingPlayer];

            if (spectateCamera == null)
            {
                spectateCamera = GameObjectUtil.CreateCamera("SpectateCamera", CameraManager.GetBaseCamera().transform, true);
                spectateCamera.enabled = true;
                CameraManager.GetBaseCamera().enabled = false;

                CameraManager.ActiveCamera = spectateCamera;
            }
            bool nv = Hack.NightVision.IsEnabled() || player.isInsideFactory;
            float intensity = Hack.NightVision.IsEnabled() ? Settings.f_nvIntensity : Settings.f_defaultNightVisionIntensity;
            float range = Hack.NightVision.IsEnabled() ? Settings.f_nvRange : Settings.f_defaultNightVisionRange;


            player.nightVision.enabled = nv;
            player.nightVision.intensity = intensity;
            player.nightVision.range = range;
            RenderPlayerModels();

            spectateCamera.transform.SetLocalPositionAndRotation(player.gameplayCamera.transform.localPosition, player.gameplayCamera.transform.localRotation);
            spectateCamera.transform.SetPositionAndRotation(player.gameplayCamera.transform.position, player.gameplayCamera.transform.rotation);
        }

        private void DisplayMiniCam()
        {
            if (!Hack.MiniCam.IsEnabled()) return;

            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;
            PlayerControllerB player = localPlayer.playersManager.allPlayerScripts[camPlayer];

            if (miniCam == null)
            {
                miniCam = GameObjectUtil.CreateCamera("SpectateMiniCam", player.gameplayCamera.transform, false);
                miniCamDisplay = GameObjectUtil.CreateMiniCamDisplay(miniCam.targetTexture);
            }
            
            miniCam.gameObject.transform.SetLocalPositionAndRotation(player.gameObject.transform.localPosition, player.gameObject.transform.localRotation);
            miniCam.transform.SetPositionAndRotation(player.gameplayCamera.transform.position, player.gameplayCamera.transform.rotation);

            bool nv = Hack.NightVision.IsEnabled() || player.isInsideFactory;
            float intensity = Hack.NightVision.IsEnabled() ? Settings.f_nvIntensity : Settings.f_defaultNightVisionIntensity;
            float range = Hack.NightVision.IsEnabled() ? Settings.f_nvRange : Settings.f_defaultNightVisionRange;


            player.nightVision.enabled = nv;
            player.nightVision.intensity = intensity;
            player.nightVision.range = range;

            RenderPlayerModels();

            tooltips = HUDManager.Instance.Tooltips.canvasGroup.gameObject.GetComponent<RectTransform>();

            if (defaultTooltipPos == Vector2.zero) defaultTooltipPos = tooltips.anchoredPosition;

            tooltips.anchoredPosition -= new Vector2(0, 200);
        }

        private static void RenderPlayerModels()
        {
            PlayerControllerB localPlayer = GameNetworkManager.Instance.localPlayerController;

            if (Hack.SpectatePlayer.IsEnabled() || Hack.FreeCam.IsEnabled())
            {
                localPlayer.DisablePlayerModel(localPlayer.gameObject, true);
                localPlayer.thisPlayerModelArms.enabled = false;
            }
            else
            {
                localPlayer.DisablePlayerModel(localPlayer.gameObject);
                localPlayer.thisPlayerModelArms.enabled = true;
            }

            foreach (PlayerControllerB player in localPlayer.playersManager.allPlayerScripts)
            {
                if (localPlayer.playerClientId == player.playerClientId) continue;

                if ((isSpectatingPlayer(player) || isCamPlayer(player)) && Settings.b_disableSpectatorModels)
                {
                    player.DisablePlayerModel(player.gameObject);
                    player.thisPlayerModelArms.enabled = true;
                }
                else
                {
                    player.DisablePlayerModel(player.gameObject, true, true);
                    player.thisPlayerModelArms.enabled = false;
                }
            }
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
