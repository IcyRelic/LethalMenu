using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Util;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    public class GrabNutcrackerShotgun
    {
        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject"), HarmonyPrefix]
        public static void BeginGrabObject(PlayerControllerB __instance)
        {
            if (!Hack.GrabNutcrackerShotgun.IsEnabled()) return;
            GrabbableObject grabbableObject = __instance.Reflect().GetValue<GrabbableObject>("currentlyGrabbingObject");
            if (grabbableObject == null) return;
            ShotgunItem shotgun = grabbableObject as ShotgunItem;
            if (shotgun == null) return;
            EnemyAI enemy = shotgun.Reflect().GetValue<EnemyAI>("heldByEnemy");
            if (enemy == null) return;
            NutcrackerEnemyAI nutcracker = enemy as NutcrackerEnemyAI;
            if (nutcracker == null) return;
            nutcracker.ChangeEnemyOwnerServerRpc(LethalMenu.localPlayer.actualClientId);
            nutcracker.DropGunServerRpc(nutcracker.gunPoint.position);
        }
    }
}