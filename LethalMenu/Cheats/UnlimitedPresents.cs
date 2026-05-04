using HarmonyLib;
using LethalMenu.Util;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class UnlimitedPresents : Cheat
    {
        [HarmonyPatch(typeof(GiftBoxItem), "ItemActivate"), HarmonyPrefix]
        public static void ItemActivate(GiftBoxItem __instance)
        {
            if (Hack.UnlimitedPresents.IsEnabled()) __instance.Reflect().SetValue("hasUsedGift", false);    
        }

        [HarmonyPatch(typeof(GrabbableObject), nameof(GrabbableObject.DestroyObjectInHand)), HarmonyPrefix]
        public static bool DestroyObjectInHand()
        {
            if (Hack.UnlimitedPresents.IsEnabled() && LethalMenu.localPlayer?.currentlyHeldObjectServer is GiftBoxItem) return false;
            return true;
        }

        [HarmonyPatch(typeof(GiftBoxItem), "OpenGiftBoxNoPresentClientRpc")]
        [HarmonyPatch(typeof(GiftBoxItem), "OpenGiftBoxClientRpc")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ReplacePoofParticle(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo fixNullPoofParticleMethod = AccessTools.Method(typeof(UnlimitedPresents), nameof(fixNullPoofParticle));
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Callvirt && instruction.operand is MethodInfo methodInfo && methodInfo.Name == "Play")
                {
                    yield return new CodeInstruction(OpCodes.Call, fixNullPoofParticleMethod);
                    continue;
                }
                yield return instruction;
            }
        }

        public static void fixNullPoofParticle(GiftBoxItem giftBoxItem)
        {
            if (giftBoxItem.PoofParticle != null) giftBoxItem.PoofParticle.Play();
        }
    }
}
