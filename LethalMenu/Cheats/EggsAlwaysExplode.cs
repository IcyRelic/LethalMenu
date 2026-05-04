using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class EggsAlwaysExplode : Cheat
    {
        // don't work

        [HarmonyPatch(typeof(StunGrenadeItem), nameof(StunGrenadeItem.EquipItem)), HarmonyPostfix]
        public static void EquipItem()
        {
            if (Hack.EggsAlwaysExplode.IsEnabled() && !Hack.EggsNeverExplode.IsEnabled() && LethalMenu.localPlayer != null && LethalMenu.localPlayer?.currentlyHeldObjectServer is StunGrenadeItem egg && egg != null && egg.explodeSFX.name == "EasterEggPop") egg.SetExplodeOnThrowServerRpc();
        }

        [HarmonyPatch(typeof(StunGrenadeItem), nameof(StunGrenadeItem.SetExplodeOnThrowClientRpc)), HarmonyPrefix]
        public static void SetExplodeOnThrowClientRpc(ref bool explode)
        {
            if (Hack.EggsAlwaysExplode.IsEnabled() && !Hack.EggsNeverExplode.IsEnabled() && LethalMenu.localPlayer != null && LethalMenu.localPlayer?.currentlyHeldObjectServer is StunGrenadeItem egg && egg != null && egg.explodeSFX.name == "EasterEggPop") explode = true;
        }
    }
}
