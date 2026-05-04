using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class EggsNeverExplode : Cheat
    {
        // don't work  I think

        [HarmonyPatch(typeof(StunGrenadeItem), nameof(StunGrenadeItem.SetExplodeOnThrowClientRpc)), HarmonyPrefix]
        public static void SetExplodeOnThrowClientRpc(ref bool explode)
        {
            if (!Hack.EggsAlwaysExplode.IsEnabled() && Hack.EggsNeverExplode.IsEnabled() && LethalMenu.localPlayer != null && LethalMenu.localPlayer?.currentlyHeldObjectServer is StunGrenadeItem egg && egg != null && egg.explodeSFX.name == "EasterEggPop") explode = false;
        }
    }
}