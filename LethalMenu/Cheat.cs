using System;
using LethalMenu.Manager;
using LethalMenu.Util;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace LethalMenu;

public class Cheat : MonoBehaviour
{
    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    protected static bool WorldToScreen(Camera camera, Vector3 world, out Vector3 screen)
    {
        screen = camera.WorldToViewportPoint(world);
        screen.x *= Screen.width;
        screen.y *= Screen.height;
        screen.y = Screen.height - screen.y;
        return screen.z > 0.0;
    }

    protected static bool WorldToScreen(Vector3 world, out Vector3 screen)
    {
        screen = CameraManager.ActiveCamera.WorldToViewportPoint(world);
        screen.x *= Screen.width;
        screen.y *= Screen.height;
        screen.y = Screen.height - screen.y;
        return screen.z > 0.0;
    }

    protected void DrawLabel(Vector3 position, string text, Color color, float distance)
    {
        VisualUtil.DrawString(new Vector2(position.x, position.y), text + "\n" + distance + "m", color, true, true);
    }

    protected static float GetDistanceToPlayer(Vector3 position)
    {
        return (float)Math.Round(Vector3.Distance(CameraManager.ActiveCamera.transform.position, position));
    }

    public virtual void OnGui()
    {
    }
}