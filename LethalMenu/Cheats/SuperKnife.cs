using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class SuperKnife : Cheat
    {
        [HarmonyPatch(typeof(KnifeItem), "HitKnife"), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> HitKnife(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand is FieldInfo fieldInfo && fieldInfo.Name == "knifeHitForce") yield return CodeInstruction.Call(typeof(SuperKnife), nameof(newKnifeHitForce));
                else yield return instruction;
            }
        }

        private static int newKnifeHitForce(KnifeItem knifeItem)
        {
            return Hack.SuperKnife.IsEnabled() ? int.MaxValue : knifeItem.knifeHitForce;
        }
    }
}
