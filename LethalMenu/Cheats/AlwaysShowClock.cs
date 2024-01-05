using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class AlwaysShowClock : Cheat
    {
        public override void Update()
        {
            if(Hack.AlwaysShowClock.IsEnabled()) HUDManager.Instance.SetClockVisible(true);
        }

        public override void OnGui()
        {
            if (Hack.SimpleClock.IsEnabled())
            {
                VisualUtil.DrawString(new Vector2(Screen.width / 2, 25f), GetTime(), Color.white, true, true, true, 40);
                VisualUtil.DrawString(new Vector2(Screen.width / 2, 55f), TimeOfDay.Instance.dayMode.ToString(), Color.white, true, true, true, 16);
            }
        }

        private string GetTime()
        {
            int num1 = (int)((double)TimeOfDay.Instance.normalizedTimeOfDay * (60.0 * (double)TimeOfDay.Instance.numberOfHours)) + 360;
            int num2 = (int)Mathf.Floor((float)(num1 / 60));


            if (num2 >= 24) return "12:00 AM";

            string amPM = num2 >= 12 ? "PM" : "AM";
            if (num2 > 12)
                num2 %= 12;
            int num3 = num1 % 60;

            return string.Format("{0:00}:{1:00} ", (object)num2, (object)num3).TrimStart('0') + amPM;
        }

        
    }
}
