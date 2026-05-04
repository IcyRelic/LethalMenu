using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class SuperSpeed : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "Update"), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Update(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand is FieldInfo fieldInfo && fieldInfo.Name == "movementSpeed") yield return CodeInstruction.Call(typeof(SuperSpeed), nameof(newMovementSpeed));
                else yield return instruction;
            }
        }

        private static float newMovementSpeed(PlayerControllerB playerControllerB)
        {
            return Hack.SuperSpeed.IsEnabled() ? Settings.f_movementSpeed : playerControllerB.movementSpeed;
        }
    }
}
