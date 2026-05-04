using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class BerserkTurrets : Cheat
    {
        [HarmonyPatch(typeof(Turret), ("Update")), HarmonyPostfix]
        public static void Update(Turret __instance)
        {
            if (Hack.BerserkAllTurrets.IsEnabled()) __instance.turretMode = TurretMode.Berserk;
        }
    }
}
