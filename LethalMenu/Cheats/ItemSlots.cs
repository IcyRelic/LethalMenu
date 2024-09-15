using HarmonyLib;
using GameNetcodeStuff;
using UnityEngine.UI;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class ItemSlots : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "Awake")]
        public static class AwakePatch
        {
            [HarmonyPostfix]
            public static void Postfix(PlayerControllerB __instance)
            {
                if (Hack.ItemSlots.IsEnabled() && Settings.f_slots != 4) __instance.ItemSlots = new GrabbableObject[(int)Settings.f_slots];
            }


            [HarmonyPatch(typeof(HUDManager), "Awake")]
            public static class HudManagerAwakePatch
            {
                [HarmonyPostfix]
                public static void Postfix(ref Image[] ___itemSlotIcons, ref Image[] ___itemSlotIconFrames)
                {
                    if (Hack.ItemSlots.IsEnabled() && Settings.f_slots != 4)
                    {
                        GameObject gameObject = Instantiate(___itemSlotIconFrames[0].gameObject);
                        Transform parent = ___itemSlotIconFrames[0].transform.parent;
                        RectTransform Rect = parent.GetComponent<RectTransform>();
                        Rect.localScale = new Vector3(Mathf.Clamp(1f - ((int)Settings.f_slots - 4) * 0.05f, 0.5f, 1f), Mathf.Clamp(1f - ((int)Settings.f_slots - 4), 0.5f, 1f), 1f);
                        Rect.anchorMin = new Vector2(0.35f, 0f);
                        Rect.anchorMax = new Vector2(0.65f, 0.3f);
                        Rect.pivot = new Vector2(0.5f, 0f);
                        Rect.anchoredPosition = new Vector2(0f, 0f);
                        GridLayoutGroup LayoutGroup = parent.gameObject.AddComponent<GridLayoutGroup>();
                        LayoutGroup.spacing = new Vector2(15f, 15f);
                        LayoutGroup.cellSize = new Vector2(50f, 50f);
                        LayoutGroup.childAlignment = TextAnchor.LowerCenter;
                        foreach (Component components in parent) Destroy(components.gameObject);
                        parent.DetachChildren();
                        for (int index = 0; index < (int)Settings.f_slots; ++index) Instantiate(gameObject, parent).name = string.Format("Slot{0}", index);
                        Destroy(gameObject);
                        ___itemSlotIcons = new Image[(int)Settings.f_slots];
                        ___itemSlotIconFrames = new Image[(int)Settings.f_slots];
                        int indexs = 0;
                        foreach (Transform transform in parent)
                        {
                            ___itemSlotIcons[indexs] = transform.GetChild(0).GetComponent<Image>();
                            ___itemSlotIconFrames[indexs] = transform.GetComponent<Image>();
                            ++indexs;
                        }
                    }
                }
            }
        }
    }
}