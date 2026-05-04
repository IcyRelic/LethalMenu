using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class LootThroughWallsBeltBag
    {
        [HarmonyPatch(typeof(BeltBagItem), "ItemInteractLeftRight"), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ItemInteractLeftRight(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_I4 && instruction.operand is int value && value == 1073742144) yield return CodeInstruction.Call(typeof(LootThroughWallsBeltBag), nameof(newLayerMask));
                else yield return instruction;
            }
        }

        private static int newLayerMask()
        {
            return Hack.LootThroughWallsBeltBag.IsEnabled() ? LayerMask.GetMask("Props") : 1073742144;
        }
    }
}
