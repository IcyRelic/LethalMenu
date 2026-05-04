using HarmonyLib;
using LethalMenu.Util;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class BuildAnywhere : Cheat
    {
        [HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild"), HarmonyPrefix]
        public static bool PlayerMeetsConditionsToBuild(ref bool __result)
        {
            if (Hack.BuildAnywhere.IsEnabled())
            {
                __result = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(ShipBuildModeManager), "EnterBuildMode")]
        [HarmonyPatch(typeof(ShipBuildModeManager), "ConfirmBuildMode_performed")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> EnterBuildModeConfirmBuildMode_performed(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand is FieldInfo fieldInfo && fieldInfo.Name == "CanConfirmPosition") yield return CodeInstruction.Call(typeof(BuildAnywhere), nameof(newCanConfirmPosition));
                else yield return instruction;
            }
        }

        private static bool newCanConfirmPosition(ShipBuildModeManager shipBuildModeManager)
        {
            return Hack.BuildAnywhere.IsEnabled() ? true : shipBuildModeManager.Reflect().GetValue<bool>("CanConfirmPosition");
        }

        [HarmonyPatch(typeof(ShipBuildModeManager), "Update"), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> Update(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand is FieldInfo fieldInfo && fieldInfo.Name == "AllowPlacementOnWalls") yield return CodeInstruction.Call(typeof(BuildAnywhere), nameof(newAllowPlacementOnWalls));
                else yield return instruction;
            }
        }

        private static bool newAllowPlacementOnWalls(PlaceableShipObject placeableShipObject)
        {
            return Hack.BuildAnywhere.IsEnabled() ? true : placeableShipObject.AllowPlacementOnWalls;
        }

        // fix red
    }
}
