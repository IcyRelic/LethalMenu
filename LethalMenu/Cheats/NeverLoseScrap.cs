using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class NeverLoseScrap : Cheat
    {
        [HarmonyPatch(typeof(RoundManager), "DespawnPropsAtEndOfRound")]
        public static class DespawnPropsAtEndOfRoundPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(bool despawnAllItems = false)
            {
                if (Hack.NeverLoseScrap.IsEnabled())
                {
                    GrabbableObject[] array = FindObjectsOfType<GrabbableObject>();
                    Transform transform = null;
                    if (FindObjectOfType<VehicleController>() != null)
                    {
                        transform = FindObjectOfType<VehicleController>().transform;
                    }
                    for (int i = 0; i < array.Length; i++)
                    {
                        if ((despawnAllItems || (!array[i].isHeld && array[i].isInShipRoom) || array[i].deactivated || (StartOfRound.Instance.allPlayersDead && array[i].itemProperties.isScrap)) && (!(array[i].transform.parent == transform) || !StartOfRound.Instance.isObjectAttachedToMagnet))
                        {
                            if (array[i].isHeld && array[i].playerHeldBy != null)
                            {
                                array[i].playerHeldBy.DropAllHeldItemsAndSync();
                            }

                            NetworkObject component = array[i].gameObject.GetComponent<NetworkObject>();
                            if (component != null && component.IsSpawned && !array[i].isInShipRoom)
                            {
                                Debug.Log("Despawning prop");
                                array[i].gameObject.GetComponent<NetworkObject>().Despawn();
                            }
                            else
                            {
                                if (!array[i].isInShipRoom)
                                {
                                    Debug.Log("Error/warning: prop '" + array[i].gameObject.name + "' was not spawned or did not have a NetworkObject component! Skipped despawning and destroyed it instead.");                     
                                    Destroy(array[i].gameObject);
                                }
                            }
                        }
                        else
                        {
                            array[i].scrapPersistedThroughRounds = true;
                        }
                    }
                    return false;
                }
                return true;
            }
        }
    }
}
