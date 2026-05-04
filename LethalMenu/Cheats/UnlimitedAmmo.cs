using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class UnlimitedAmmo
    {
        [HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.ShootGun)), HarmonyPostfix]
        public static void ShotgunShootGun(ShotgunItem __instance)
        {
            if(Hack.UnlimitedAmmo.IsEnabled()) __instance.shellsLoaded++;
        }

        [HarmonyPatch(typeof(ShotgunItem), nameof(ShotgunItem.ItemActivate)), HarmonyPostfix]
        public static void ItemActivateShotGunPrefix(ShotgunItem __instance)
        {
            if (Hack.UnlimitedAmmo.IsEnabled() && __instance.shellsLoaded < 1) __instance.shellsLoaded++;
        }
    }
}
