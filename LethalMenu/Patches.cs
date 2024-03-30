using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using LethalMenu.Cheats;

namespace LethalMenu;

[HarmonyPatch]
internal class Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Disconnect))]
    public static void Disconnect(GameNetworkManager __instance)
    {
        SpectatePlayer.Reset();
        Freecam.Reset();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.AttackPlayersServerRpc))]
    public static void CompanyAttackPrefix(ref bool ___attacking, ref bool ___inGrabbingObjectsAnimation,
        ref bool __state)
    {
        __state = ___inGrabbingObjectsAnimation;
        ___attacking = false;
        ___inGrabbingObjectsAnimation = false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.AttackPlayersServerRpc))]
    public static void CompanyAttackPostfix(ref bool ___inGrabbingObjectsAnimation, ref bool __state)
    {
        ___inGrabbingObjectsAnimation = __state;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(typeof(DepositItemsDesk), nameof(DepositItemsDesk.PlaceItemOnCounter))]
    public static IEnumerable<CodeInstruction> CompanyInfiniteSell(IEnumerable<CodeInstruction> instructions)
    {
        return instructions.Select(instruction =>
            instruction.opcode == OpCodes.Ldc_I4_S && instruction.operand.ToString().Equals("12")
                ? new CodeInstruction(OpCodes.Ldc_I4, int.MaxValue)
                : instruction);
    }
}