using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class Reach : Cheat
    {
        [HarmonyPatch(typeof(BeltBagItem), "ItemInteractLeftRight"), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ItemInteractLeftRight(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldc_R4 && instruction.operand is float value && value == 4f) yield return CodeInstruction.Call(typeof(Reach), nameof(newBeltBagMaxDistance));
                else yield return instruction;
            }
        }

        private static float newBeltBagMaxDistance()
        {
            return Hack.Reach.IsEnabled() ? float.MaxValue : 4f;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
        [HarmonyPatch(typeof(PlayerControllerB), "SetHoverTipAndCurrentInteractTrigger")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> BeginGrabObject(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand is FieldInfo fieldInfo && fieldInfo.Name == "grabDistance") yield return CodeInstruction.Call(typeof(Reach), nameof(newGrabDistance));
                else yield return instruction;
            }
        }

        private static float newGrabDistance(PlayerControllerB playerControllerB)
        {
            return Hack.Reach.IsEnabled() ? float.MaxValue : playerControllerB.grabDistance;
        }
    }
}
