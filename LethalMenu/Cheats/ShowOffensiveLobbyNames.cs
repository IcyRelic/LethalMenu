using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class ShowOffensiveLobbyName : Cheat
    {
        [HarmonyPatch(typeof(SteamLobbyManager), "loadLobbyListAndFilter"), HarmonyPrefix]
        public static void loadLobbyListAndFilter(SteamLobbyManager __instance)
        {
            if (Hack.ShowOffensiveLobbyNames.IsEnabled()) __instance.censorOffensiveLobbyNames = false;
        }
    }
}
