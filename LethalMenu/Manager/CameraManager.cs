using GameNetcodeStuff;
using LethalMenu.Types;
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
            if (LethalMenu.localPlayer == null || LethalMenu.localPlayer.gameplayCamera == null) return Camera.main;
            return LethalMenu.localPlayer.isPlayerDead ? StartOfRound.Instance.spectateCamera : LethalMenu.localPlayer.gameplayCamera ?? Camera.main;
        }

        public static bool UsingBaseCamera()
        {
            return _camera.GetInstanceID() == LethalMenu.localPlayer?.gameplayCamera.GetInstanceID() || _camera.GetInstanceID() == StartOfRound.Instance.spectateCamera.GetInstanceID();
        }     
    }
}
