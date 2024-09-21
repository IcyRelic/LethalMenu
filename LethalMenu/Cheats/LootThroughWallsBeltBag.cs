using HarmonyLib;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class LootThroughWallsBeltBag
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BeltBagItem), "ItemInteractLeftRight")]
        public static bool ItemInteractLeftRight(BeltBagItem __instance, bool right)
        {
            if (__instance.playerHeldBy == null || __instance.tryingAddToBag || __instance.objectsInBag.Count >= 15 || right) return true;
            float grabDistance = Hack.LootThroughWallsBeltBag.IsEnabled() ? float.MaxValue : 4f;
            LayerMask mask = Hack.LootThroughWallsBeltBag.IsEnabled() ? LayerMask.GetMask("Props") : (LayerMask)1073742144;
            if (Physics.Raycast(__instance.playerHeldBy.gameplayCamera.transform.position, __instance.playerHeldBy.gameplayCamera.transform.forward, out var hit, grabDistance, mask.value, QueryTriggerInteraction.Ignore))
            {
                GrabbableObject item = hit.collider.gameObject.GetComponent<GrabbableObject>();
                if (item != null && !item.isHeld && !item.isHeldByEnemy)
                {
                    if (Hack.LootAnyItemBeltBag.IsEnabled() || (!item.itemProperties.isScrap && item.itemProperties.itemId != 123984 && item.itemProperties.itemId != 819501))
                    {
                        if (Hack.LootAnyItemBeltBag.IsEnabled() && item is LungProp lung && lung.isLungDocked) lung.EquipItem();
                        __instance.TryAddObjectToBag(item);
                    }
                }
            }
            return false;
        }
    }
}
