using System;
using UnityEngine;
using UnityEngine.UI;
using HarmonyLib;
using GameNetcodeStuff;

namespace LethalMenu.Cheats
{
    internal class ExtraItemSlots : Cheat
    {
        [HarmonyPatch(typeof(PlayerControllerB), "Awake")]
        public static class PlayerControllerBAwakePatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref GrabbableObject[] ___ItemSlots)
            {
                if (Hack.ExtraItemSlots.IsEnabled() && Settings.i_slots != 4)
                {
                    ___ItemSlots = new GrabbableObject[Settings.i_slots];
                }
            }
        }

        [HarmonyPatch(typeof(HUDManager), "Awake")]
        public static class HudManagerAwakePatch
        {
            [HarmonyPostfix]
            public static void Postfix(ref Image[] ___itemSlotIcons, ref Image[] ___itemSlotIconFrames)
            {
                if (Hack.ExtraItemSlots.IsEnabled())
                {
                    GameObject gameObject = Instantiate(___itemSlotIconFrames[0].gameObject);
                    Transform parent = ___itemSlotIconFrames[0].transform.parent;
                    RectTransform Rect = parent.GetComponent<RectTransform>();
                    Rect.localScale = new Vector3(Mathf.Clamp(1f - (Settings.i_slots - 4) * 0.05f, 0.5f, 1f), Mathf.Clamp(1f - (Settings.i_slots - 4), 0.5f, 1f), 1f);
                    Rect.anchorMin = new Vector2(0.35f, 0f);
                    Rect.anchorMax = new Vector2(0.65f, 0.3f);
                    Rect.pivot = new Vector2(0.5f, 0f);
                    Rect.anchoredPosition = new Vector2(0f, 0f);
                    GridLayoutGroup LayoutGroup = parent.gameObject.AddComponent<GridLayoutGroup>();
                    LayoutGroup.spacing = new Vector2(15f, 15f);
                    LayoutGroup.cellSize = new Vector2(50f, 50f);
                    LayoutGroup.childAlignment = TextAnchor.LowerCenter;
                    foreach (Component components in parent)
                    {
                        Destroy(components.gameObject);
                    }
                    parent.DetachChildren();
                    for (int index = 0; index < Settings.i_slots; ++index)
                    {
                        Instantiate(gameObject, parent).name = string.Format("Slot{0}", index);
                    }
                    Destroy(gameObject);
                    ___itemSlotIcons = new Image[Settings.i_slots];
                    ___itemSlotIconFrames = new Image[Settings.i_slots];
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

