using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats
{
    public enum CrosshairType
    {
        X = 0,
        Plus = 1
    }
    internal class Crosshair : Cheat
    {
        

        public override void OnGui()
        {
            if (!Hack.Crosshair.IsEnabled()) return;

            Vector2[] pointsX = new Vector2[4]
                {
                    new Vector2((float)Screen.width / 2f - Settings.f_crosshairScale, (float)Screen.height / 2f - Settings.f_crosshairScale),
                    new Vector2((float)Screen.width / 2f + Settings.f_crosshairScale, (float)Screen.height / 2f + Settings.f_crosshairScale),
                    new Vector2((float)Screen.width / 2f + Settings.f_crosshairScale, (float)Screen.height / 2f - Settings.f_crosshairScale),
                    new Vector2((float)Screen.width / 2f - Settings.f_crosshairScale, (float)Screen.height / 2f + Settings.f_crosshairScale)
                };

            Vector2[] pointsPlus = new Vector2[4]
            {
                    new Vector2((float)Screen.width / 2f - Settings.f_crosshairScale, (float)Screen.height / 2f),
                    new Vector2((float)Screen.width / 2f + Settings.f_crosshairScale, (float)Screen.height / 2f),
                    new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f - Settings.f_crosshairScale),
                    new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f + Settings.f_crosshairScale)
                };

            Vector2[] points = Settings.b_crosshairPlus ? pointsPlus : pointsX;

            VisualUtil.DrawLine(points[0], points[1], Settings.c_crosshair, Settings.f_crosshairThickness);
            VisualUtil.DrawLine(points[2], points[3], Settings.c_crosshair, Settings.f_crosshairThickness);
            

        }
    }
}
