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
        var timeOfDay = (int)(TimeOfDay.Instance.normalizedTimeOfDay *
                              (60.0 * TimeOfDay.Instance.numberOfHours)) + 360;
        var floor = (int)Mathf.Floor(timeOfDay / 60);


        if (floor >= 24) return "12:00 AM";

        var amPm = floor >= 12 ? "PM" : "AM";
        if (floor > 12)
            floor %= 12;
        var i = timeOfDay % 60;

        return $"{floor:00}:{i:00} ".TrimStart('0') + amPm;
    }
}