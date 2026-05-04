using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    public class LootBeforeGameStarts : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "SetHoverTipAndCurrentInteractTrigger")]
        [HarmonyPatch(typeof(PlayerControllerB), "BeginGrabObject")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> SetHoverTipAndCurrentInteractTriggerBeginGrabObject(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand is FieldInfo fieldInfo && fieldInfo.Name == "canBeGrabbedBeforeGameStart") yield return CodeInstruction.Call(typeof(LootBeforeGameStarts), nameof(newCanBeGrabbedBeforeGameStart));
                else yield return instruction;
            }
        }

        private static bool newCanBeGrabbedBeforeGameStart(Item item)
        {
            return Hack.LootBeforeGameStarts.IsEnabled() ? true : item.canBeGrabbedBeforeGameStart;
        }
    }
}