using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LethalMenu.Manager
{
    public class CameraManager
    {
        private static Camera _camera = null;
        public static Camera ActiveCamera
        {
            get
            {
                if (!(bool)StartOfRound.Instance) _camera = null;

                PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;
                
                if(_camera == null || UsingBaseCamera()) _camera = GetBaseCamera();

                

                return _camera;
            }
            set
            {
                _camera = value;
            }
        }
        
        public static Camera GetBaseCamera()
        {
            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;

            return player.isPlayerDead ? StartOfRound.Instance.spectateCamera : player.gameplayCamera;
        }

        public static bool UsingBaseCamera()
        {
            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;

            return _camera.GetInstanceID() == player.gameplayCamera.GetInstanceID() 
                || _camera.GetInstanceID() == StartOfRound.Instance.spectateCamera.GetInstanceID();
        }
        
    }
}
