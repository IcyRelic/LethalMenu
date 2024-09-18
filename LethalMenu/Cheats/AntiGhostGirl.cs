using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class AntiGhostGirl : Cheat
    {
        public override void Update()
        {
            if (!Hack.AntiGhostGirl.IsEnabled() || Object.FindAnyObjectByType<DressGirlAI>().hauntingPlayer != LethalMenu.localPlayer) return;
            Object.FindAnyObjectByType<DressGirlAI>().hauntingPlayer = null;
        }
    }
}