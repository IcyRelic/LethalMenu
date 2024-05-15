using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class NeverLoseScrap : Cheat
    {
        [HarmonyPatch(typeof(RoundManager), ("DespawnPropsAtEndOfRound"))]
        public static class DespawnPropsAtEndOfRoundPatch
        {
            public static bool Prefix(ref RoundManager __instance, bool despawnAllItems = false)
            {
                if (Hack.NeverLoseScrap.IsEnabled())
                {
                    if (!(__instance).IsServer)
                    {
                        return false;
                    }

                    GrabbableObject[] array = Object.FindObjectsOfType<GrabbableObject>();
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (despawnAllItems || (!array[i].isHeld && !array[i].isInShipRoom) || array[i].deactivated)
                        {
                            if (array[i].isHeld && (Object)(object)array[i].playerHeldBy != (Object)null)
                            {
                                array[i].playerHeldBy.DropAllHeldItems(true, false);
                            }
                            array[i].gameObject.GetComponent<NetworkObject>().Despawn(true);
                        }
                        else
                        {
                            array[i].scrapPersistedThroughRounds = true;
                        }

                        if (__instance.spawnedSyncedObjects.Contains(array[i].gameObject))
                        {
                            __instance.spawnedSyncedObjects.Remove(array[i].gameObject);
                        }
                    }

                    GameObject[] array2 = GameObject.FindGameObjectsWithTag("TemporaryEffect");
                    for (int j = 0; j < array2.Length; j++)
                    {
                        Object val = (Object)(object)array2[j];
                        Object.Destroy(val);
                    }
                }
                return false;
            }
        }
    }
}
