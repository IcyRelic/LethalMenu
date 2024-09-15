using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace LethalMenu.Cheats
{
    internal class ShowOffensiveLobbyName : Cheat
    {
        [HarmonyPatch(typeof(SteamLobbyManager), "loadLobbyListAndFilter")] 
        public static class SteamLobbyManagerloadLobbyListAndFilterPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(SteamLobbyManager __instance)
            {
                if (Hack.ShowOffensiveLobbyNames.IsEnabled())
                {
                    __instance.censorOffensiveLobbyNames = false;
                }
                return true;
            }
        }
    }
}
