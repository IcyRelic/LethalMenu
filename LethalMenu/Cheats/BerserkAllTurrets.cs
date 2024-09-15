using System.Linq;
using GameNetcodeStuff;
using HarmonyLib; 

namespace LethalMenu.Cheats
{
    internal class BerserkTurrets : Cheat
    {
        [HarmonyPatch(typeof(Turret), ("Update"))]
        public static class TurretUpdatePatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref int ___turretMode)
            {
                if (Hack.BerserkAllTurrets.IsEnabled())
                {
                    ___turretMode = 3;
                }
            }
        }
    }
}
