using HarmonyLib;
using LethalMenu.Util;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class UnlimitedTZP : Cheat
    {
        public static TetraChemicalItem TZP;

        public override void Update()
        {
            if (!Hack.UnlimitedTZP.IsEnabled()) return;
            TZP.Reflect().SetValue("fuel", 1f);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(TetraChemicalItem), nameof(TetraChemicalItem.EquipItem))]
        public static void EquipItem(TetraChemicalItem __instance)
        {
            TZP = __instance;
        }
    }
}
