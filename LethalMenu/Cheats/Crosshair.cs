using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats
{
    public enum CrosshairType
    {
        X = 0,
        Plus = 1,
        Dot = 2,
        CircleCross = 3
    }

    internal class Crosshair : Cheat
    {
        public override void OnGui()
        {
            if (!Hack.Crosshair.IsEnabled()) return;
            Vector2[] pointsX =
            [
                new((float)Screen.width / 2f - Settings.f_crosshairScale, (float)Screen.height / 2f - Settings.f_crosshairScale),
                new((float)Screen.width / 2f + Settings.f_crosshairScale, (float)Screen.height / 2f + Settings.f_crosshairScale),
                new((float)Screen.width / 2f + Settings.f_crosshairScale, (float)Screen.height / 2f - Settings.f_crosshairScale),
                new((float)Screen.width / 2f - Settings.f_crosshairScale, (float)Screen.height / 2f + Settings.f_crosshairScale)
            ];
            Vector2[] pointsPlus =
            [
                new((float)Screen.width / 2f - Settings.f_crosshairScale, (float)Screen.height / 2f),
                new((float)Screen.width / 2f + Settings.f_crosshairScale, (float)Screen.height / 2f),
                new((float)Screen.width / 2f, (float)Screen.height / 2f - Settings.f_crosshairScale),
                new((float)Screen.width / 2f, (float)Screen.height / 2f + Settings.f_crosshairScale)
            ];
            Vector2[] pointsDot =
            [
                new((float)Screen.width / 2f, (float)Screen.height / 2f)
            ];
            switch (Settings.ct_crosshairType)
            {
                case CrosshairType.X:
                    VisualUtil.DrawLine(pointsX[0], pointsX[1], Settings.c_crosshair, Settings.f_crosshairThickness);
                    VisualUtil.DrawLine(pointsX[2], pointsX[3], Settings.c_crosshair, Settings.f_crosshairThickness);
                    return;
                case CrosshairType.Plus:
                    VisualUtil.DrawLine(pointsPlus[0], pointsPlus[1], Settings.c_crosshair, Settings.f_crosshairThickness);
                    VisualUtil.DrawLine(pointsPlus[2], pointsPlus[3], Settings.c_crosshair, Settings.f_crosshairThickness);
                    return;
                case CrosshairType.Dot:
                    VisualUtil.DrawDot(pointsDot[0], Settings.c_crosshair, Settings.f_crosshairThickness);
                    return;
            }
        }
    }
}
