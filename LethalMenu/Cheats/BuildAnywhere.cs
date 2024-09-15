using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class BuildAnywhere : Cheat
    {
        [HarmonyPatch(typeof(ShipBuildModeManager), "PlayerMeetsConditionsToBuild")]
        public static class PlayerMeetsConditionsToBuildPatch
        {
            [HarmonyPrefix]
            public static bool Prefix(ShipBuildModeManager __instance, ref bool __result)
            {
                if (Hack.BuildAnywhere.IsEnabled())
                {
                    __result = true;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ShipBuildModeManager), ("Update"))]
        public static class ShipBuildModeManagerPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref bool ___CanConfirmPosition, ref PlaceableShipObject ___placingObject, ref bool ___InBuildMode)
            {
                if (Hack.BuildAnywhere.IsEnabled() && ___InBuildMode)
                {
                    ___CanConfirmPosition = true;
                    ___placingObject.AllowPlacementOnWalls = true;
                    ___placingObject.AllowPlacementOnCounters = true;
                }
            }
        }

        [HarmonyPatch(typeof(ShipBuildModeManager), "PlaceShipObject")]
        public static class PlaceShipObjectPatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref Vector3 placementPosition, ref Vector3 placementRotation, ref PlaceableShipObject placeableObject)
            {
                if (Hack.BuildAnywhere.IsEnabled())
                {
                    placeableObject.transform.position = placementPosition;
                    placeableObject.transform.rotation = Quaternion.Euler(placementRotation);
                    StartOfRound.Instance.suckingFurnitureOutOfShip = false;
                    StartOfRound.Instance.unlockablesList.unlockables[placeableObject.unlockableID].placedPosition = placementPosition;
                    StartOfRound.Instance.unlockablesList.unlockables[placeableObject.unlockableID].placedRotation = placementRotation;
                    StartOfRound.Instance.unlockablesList.unlockables[placeableObject.unlockableID].hasBeenMoved = true;
                    if (placeableObject.parentObjectSecondary != null)
                    {
                        Quaternion quaternion = Quaternion.Euler(placementRotation) * Quaternion.Inverse(placeableObject.mainMesh.transform.rotation);
                        placeableObject.parentObjectSecondary.transform.rotation = quaternion * placeableObject.parentObjectSecondary.transform.rotation;
                        placeableObject.parentObjectSecondary.position = placementPosition + (placeableObject.parentObjectSecondary.transform.position - placeableObject.mainMesh.transform.position) + (placeableObject.mainMesh.transform.position - placeableObject.placeObjectCollider.transform.position);
                    }
                }
            }
        }
    }
}
