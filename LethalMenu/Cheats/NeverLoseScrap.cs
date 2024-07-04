using HarmonyLib;
using Steamworks.Ugc;
using Unity.Netcode;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class NeverLoseScrap : Cheat
    {
        [HarmonyPatch(typeof(RoundManager), ("DespawnPropsAtEndOfRound"))]
        public static class DespawnPropsAtEndOfRoundPatch
        {
            public static bool Prefix(bool despawnAllItems = false)
            {
                {
                    foreach (var item in LethalMenu.items)
                    {
                        if (Hack.NeverLoseScrap.IsEnabled() && item.isInShipRoom)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    }
}
