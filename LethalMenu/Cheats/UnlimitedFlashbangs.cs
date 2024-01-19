using System.Reflection;
using HarmonyLib;


namespace LethalMenu.Cheats
{
    internal class UnlimitedFlashbangs : Cheat
    {
        [HarmonyPatch(typeof(StunGrenadeItem), "ItemActivate")]
        public class StunGrenadeItem_ItemActivate_Patch
        {
            public static bool Prefix(StunGrenadeItem __instance)
            {
                if (!Hack.UnlimitedFlashbangs.IsEnabled())
                {
                    __instance.itemUsedUp = false;
                    __instance.pinPulled = false;
                    __instance.hasExploded = false;
                    __instance.DestroyGrenade = false;
                    typeof(StunGrenadeItem).GetField("pullPinCoroutine", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(__instance, null);
                }

                return true;
            }
        }
    }
}
