using UnityEngine;
using UnityEngine.UI;

namespace LethalMenu.Util;

internal class GameObjectUtil
{
    public static Camera CreateCamera(string name, Transform pos, bool copyPlayerTexture = true)
    {
        var localPlayer = GameNetworkManager.Instance.localPlayerController;
        var camera = new GameObject(name).AddComponent<Camera>();

        camera.transform.position = pos.position;
        camera.transform.rotation = pos.rotation;
        camera.targetTexture = copyPlayerTexture
            ? localPlayer.gameplayCamera.targetTexture
            : new RenderTexture(1920, 1080, 24);
        camera.cullingMask = localPlayer.gameplayCamera.cullingMask;
        camera.farClipPlane = localPlayer.gameplayCamera.farClipPlane;
        camera.nearClipPlane = localPlayer.gameplayCamera.nearClipPlane;

        return camera;
    }

    public static RawImage CreateMiniCamDisplay(Texture targetTexture)
    {
        var display = new GameObject("SpectateMiniCamDisplay").AddComponent<RawImage>();
        display.rectTransform.anchorMin = new Vector2(1, 1);
        display.rectTransform.anchorMax = new Vector2(1, 1);
        display.rectTransform.pivot = new Vector2(1f, 1f);
        display.rectTransform.sizeDelta = new Vector2(192, 108);
        display.rectTransform.anchoredPosition = new Vector2(1, 1);
        display.texture = targetTexture;
        display.transform.SetParent(HUDManager.Instance.playerScreenTexture.transform, false);

        return display;
    }

    public static Light CreateLight()
    {
        //create and return a copy of LethalMenu.localPlayer.nightVision
        var light = Object.Instantiate(LethalMenu.localPlayer.nightVision);

        light.enabled = true;
        light.intensity = Settings.f_nvIntensity;
        light.range = Settings.f_nvRange;

        return light;
    }
}