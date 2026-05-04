using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Util;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class ThroughWalls : Cheat
    {
        [HarmonyPatch(typeof(BeltBagItem), "ItemInteractLeftRight"), HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ItemInteractLeftRight(IEnumerable<CodeInstruction> instructions)
        {
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Ldfld && instruction.operand is FieldInfo fieldInfo && fieldInfo.Name == "interactableObjectsMask") yield return CodeInstruction.Call(typeof(ThroughWalls), nameof(newInteractableObjectsMask));
                else yield return instruction;
            }
        }

        private static int newInteractableObjectsMask(PlayerControllerB playerControllerB)
        {
            if (Hack.LootThroughWalls.IsEnabled() && Hack.InteractThroughWalls.IsEnabled()) return LayerMask.GetMask("Props", "InteractableObject");
            if (Hack.LootThroughWalls.IsEnabled()) return LayerMask.GetMask("Props");
            if (Hack.InteractThroughWalls.IsEnabled()) return LayerMask.GetMask("InteractableObject");
            return playerControllerB.Reflect().GetValue<int>("interactableObjectsMask");
        }
    }
}

