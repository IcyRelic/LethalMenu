using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class UnlimitedAmmo
    {

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.ShootGun))]
        public static void ShotgunShootGun(ShotgunItem __instance)
        {
            if(Hack.UnlimitedAmmo.IsEnabled())
                __instance.shellsLoaded++;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.ItemActivate))]
        public static void ActiveShotGunPrefix(ShotgunItem __instance)
        {

            if (Hack.UnlimitedAmmo.IsEnabled() && __instance.shellsLoaded < 1)
                __instance.shellsLoaded++;
        }
    }
}
