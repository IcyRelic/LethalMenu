using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    internal class NoCameraShake : Cheat
    {
        [HarmonyPatch(typeof(HUDManager), ("ShakeCamera"))]
        public static class HUDManagerShakeCameraPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ScreenShakeType shakeType)
            {
                if (Hack.NoCameraShake.IsEnabled())
                {
                    return false;
                }
                return true;
            }
        }
    }
}
