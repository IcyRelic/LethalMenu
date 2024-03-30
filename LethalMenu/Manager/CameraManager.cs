using UnityEngine;

namespace LethalMenu.Manager;

public static class CameraManager
{
    private static Camera _camera;

    public static Camera ActiveCamera
    {
        get
        {
            if (!(bool)StartOfRound.Instance) _camera = null;

            if (!_camera || UsingBaseCamera()) _camera = GetBaseCamera();

            return _camera;
        }
        set => _camera = value;
    }

    public static Camera GetBaseCamera()
    {
        var player = GameNetworkManager.Instance.localPlayerController;

        return player.isPlayerDead ? StartOfRound.Instance.spectateCamera : player.gameplayCamera;
    }

    private static bool UsingBaseCamera()
    {
        var player = GameNetworkManager.Instance.localPlayerController;

        return _camera.GetInstanceID() == player.gameplayCamera.GetInstanceID()
               || _camera.GetInstanceID() == StartOfRound.Instance.spectateCamera.GetInstanceID();
    }
}