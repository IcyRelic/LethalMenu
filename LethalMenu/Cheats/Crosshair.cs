using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats;

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

        var pointsX = new Vector2[4]
        {
            new(Screen.width / 2f - Settings.f_crosshairScale,
                Screen.height / 2f - Settings.f_crosshairScale),
            new(Screen.width / 2f + Settings.f_crosshairScale,
                Screen.height / 2f + Settings.f_crosshairScale),
            new(Screen.width / 2f + Settings.f_crosshairScale,
                Screen.height / 2f - Settings.f_crosshairScale),
            new(Screen.width / 2f - Settings.f_crosshairScale,
                Screen.height / 2f + Settings.f_crosshairScale)
        };

        var pointsPlus = new Vector2[4]
        {
            new(Screen.width / 2f - Settings.f_crosshairScale, Screen.height / 2f),
            new(Screen.width / 2f + Settings.f_crosshairScale, Screen.height / 2f),
            new(Screen.width / 2f, Screen.height / 2f - Settings.f_crosshairScale),
            new(Screen.width / 2f, Screen.height / 2f + Settings.f_crosshairScale)
        };

        var points = Settings.ct_crosshairType == CrosshairType.Plus ? pointsPlus : pointsX;

        VisualUtil.DrawLine(points[0], points[1], Settings.c_crosshair, Settings.f_crosshairThickness);
        VisualUtil.DrawLine(points[2], points[3], Settings.c_crosshair, Settings.f_crosshairThickness);
    }
}