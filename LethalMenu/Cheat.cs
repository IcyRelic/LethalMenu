using LethalMenu.Manager;
using LethalMenu.Util;
using System;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;

namespace LethalMenu
{
    public class Cheat : MonoBehaviour
    {
        protected static bool WorldToScreen(Camera camera, Vector3 world, out Vector3 screen)
        {
            screen = camera.WorldToViewportPoint(world);
            screen.x *= (float)Screen.width;
            screen.y *= (float)Screen.height;
            screen.y = (float)Screen.height - screen.y;
            return (double)screen.z > 0.0;
        }

        protected static bool WorldToScreen(Vector3 world, out Vector3 screen)
        {           
            screen = CameraManager.ActiveCamera.WorldToViewportPoint(world);
            screen.x *= (float)Screen.width;
            screen.y *= (float)Screen.height;
            screen.y = (float)Screen.height - screen.y;
            return (double)screen.z > 0.0;
        }

        protected void DrawLabel(Vector3 pos, string text, Color color, float distance)
        {


           VisualUtil.DrawString(new Vector2(pos.x, pos.y), text + "\n" + distance.ToString() + "m", color, true, true);

            /**GUIStyle style = Settings.GetLabelStyle(color);
            style.alignment = TextAnchor.MiddleCenter;

            GUI.Label(new Rect(pos.x, pos.y, 155f, 65f), text + "\n" + distance.ToString() + "m", style);**/
        }

        protected float GetDistanceToPlayer(Vector3 position)
        {
            return (float)Math.Round((double)Vector3.Distance(CameraManager.ActiveCamera.transform.position, position));
        }

        public virtual void OnGui() { }
        public virtual void Update() { }
        public virtual void FixedUpdate() { }
    }

}
