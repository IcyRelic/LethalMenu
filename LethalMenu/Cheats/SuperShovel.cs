using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class SuperShovel : Cheat
    {
        [HarmonyPatch(typeof(Shovel), "HitShovel"), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> HitShovel(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand is FieldInfo fieldInfo && fieldInfo.Name == "shovelHitForce") yield return CodeInstruction.Call(typeof(SuperShovel), nameof(newHitShovel));
                else yield return instruction;
            }
        }

        private static int newHitShovel(Shovel shovel)
        {
            return Hack.SuperShovel.IsEnabled() ? int.MaxValue : shovel.shovelHitForce;
        }
    }
}