using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class LootAnyItemBeltBag : Cheat
    {
        [HarmonyPatch(typeof(BeltBagItem), "ItemInteractLeftRight"), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ItemInteractLeftRight(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo newItemCheckMethod = AccessTools.Method(typeof(LootAnyItemBeltBag), nameof(newItemCheck));
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;
                if (instruction.opcode == OpCodes.Stloc_1)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldloc_1);
                    yield return new CodeInstruction(OpCodes.Call, newItemCheckMethod);
                    yield return new CodeInstruction(OpCodes.Pop);
                }
            }
        }

        private static bool newItemCheck(BeltBagItem beltBagItem, GrabbableObject grabbableObject)
        {
            if (Hack.LootAnyItemBeltBag.IsEnabled())
            {
                if (grabbableObject is LungProp lung && lung.isLungDocked) lung.EquipItem();
                beltBagItem.TryAddObjectToBag(grabbableObject);
                return true;
            }
            return grabbableObject != null && !grabbableObject.itemProperties.isScrap && !grabbableObject.isHeld && !grabbableObject.isHeldByEnemy && grabbableObject.itemProperties.itemId != 123984 && grabbableObject.itemProperties.itemId != 819501;
        }
    }
}
