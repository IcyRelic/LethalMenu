using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class NoCameraShake : Cheat
    {
        [HarmonyPatch(typeof(HUDManager), ("ShakeCamera")), HarmonyPrefix]
        public static bool ShakeCamera(ScreenShakeType shakeType)
        {
            return !Hack.NoCameraShake.IsEnabled();
        }
    }
}
