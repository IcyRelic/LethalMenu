using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class AntiGhostGirl : Cheat
    {
        public override void Update()
        {
            if (!Hack.AntiGhostGirl.IsEnabled()) return;
            DressGirlAI dressgirl = Object.FindAnyObjectByType<DressGirlAI>();
            if (dressgirl == null || dressgirl.hauntingPlayer != LethalMenu.localPlayer) return;
            dressgirl.hauntingPlayer = null;
        }
    }
}