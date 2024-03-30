using System;
using LethalMenu.Components;
using LethalMenu.Manager;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats;

internal class Freecam : Cheat
{
    //FreeCam
    public static Camera Camera;
    private static Light _light;
    private static MouseInput _mouse;
    private static KbInput _movement;
    private static bool _isStatic;

    public static void Reset()
    {
        if (Camera != null) Destroy(Camera.gameObject);
        if (_light != null) Destroy(_light.gameObject);

        Camera = null;
        _light = null;

        Hack.FreeCam.SetToggle(false);
        Stop();
    }


    public override void Update()
    {
        try
        {
            Stop();

            if (!Hack.FreeCam.IsEnabled()) return;
            _isStatic = Hack.EnemyControl.IsEnabled();
            LethalMenu.LocalPlayer.enabled = false;
            LethalMenu.LocalPlayer.isFreeCamera = true;
            CreateIfNull();
            _light.intensity = Settings.f_nvIntensity;
            _light.range = Settings.f_nvRange;
            GameUtil.RenderPlayerModels();
            if (!_isStatic)
                Camera.transform.SetPositionAndRotation(_movement.transform.position, _mouse.transform.rotation);
        }
        catch (Exception e)
        {
            LethalMenu.DebugMessage = e.Message + "\n" + e.StackTrace;
        }
    }

    private static void CreateIfNull()
    {
        if (Camera) return;

        Camera = GameObjectUtil.CreateCamera("FreeCam", CameraManager.GetBaseCamera().transform);
        Camera.enabled = true;
        _mouse = Camera.gameObject.AddComponent<MouseInput>();
        _movement = Camera.gameObject.AddComponent<KbInput>();
        _light = GameObjectUtil.CreateLight();
        _light.transform.SetParent(Camera.transform, false);
        CameraManager.GetBaseCamera().enabled = false;
        CameraManager.ActiveCamera = Camera;
    }

    private static void Stop()
    {
        if (!(bool)StartOfRound.Instance || Hack.FreeCam.IsEnabled() || !Camera) return;
        CameraManager.ActiveCamera = SpectatePlayer.spectatingPlayer == -1
            ? CameraManager.GetBaseCamera()
            : SpectatePlayer.camera;
        CameraManager.GetBaseCamera().enabled = true;

        Camera.enabled = false;
        Destroy(Camera.gameObject);
        Camera = null;
        _mouse = null;
        _movement = null;


        Destroy(_light.gameObject);
        _light = null;

        LethalMenu.LocalPlayer.enabled = true;
        LethalMenu.LocalPlayer.isFreeCamera = false;
        GameUtil.RenderPlayerModels();
    }
}