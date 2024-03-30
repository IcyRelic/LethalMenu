using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats;

internal class AlwaysShowClock : Cheat
{
    public override void Update()
    {
        if (Hack.AlwaysShowClock.IsEnabled()) HUDManager.Instance.SetClockVisible(true);
    }

    public override void OnGui()
    {
        if (!Hack.SimpleClock.IsEnabled()) return;

        VisualUtil.DrawString(new Vector2(Screen.width / 2, 25f), GetTime(), Color.white, true, true, true, 40);
        VisualUtil.DrawString(new Vector2(Screen.width / 2, 55f), TimeOfDay.Instance.dayMode.ToString(),
            Color.white, true, true, true, 16);
    }

    private string GetTime()
    {
        var num1 = (int)(TimeOfDay.Instance.normalizedTimeOfDay *
                         (60.0 * TimeOfDay.Instance.numberOfHours)) + 360;
        var num2 = (int)Mathf.Floor(num1 / 60);


        if (num2 >= 24) return "12:00 AM";

        var amPM = num2 >= 12 ? "PM" : "AM";
        if (num2 > 12)
            num2 %= 12;
        var num3 = num1 % 60;

        return $"{num2:00}:{num3:00} ".TrimStart('0') + amPM;
    }
}