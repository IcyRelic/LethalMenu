using TMPro;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class HPDisplay : Cheat
    {
        private static TextMeshProUGUI HPText = null;
        private static GameObject text = null; 

        public override void Update()
        {
            if (!Hack.HPDisplay.IsEnabled() || LethalMenu.localPlayer == null)
            {
                if (HPText != null)
                {
                    Object.Destroy(text);
                    text = null;
                    HPText = null;
                }
                return;
            }
            if (HPText == null)
            {
                GameObject TopLeftCorner = GameObject.Find("Systems/UI/Canvas/IngamePlayerHUD/TopLeftCorner");
                if (TopLeftCorner == null) return;
                text = new GameObject("HealthText");
                text.transform.SetParent(TopLeftCorner.transform, false);
                HPText = text.AddComponent<TextMeshProUGUI>();
                TextMeshProUGUI weightCounter = HUDManager.Instance.weightCounter;
                if (weightCounter.transform.parent == null) return;
                RectTransform weightCounterParent = weightCounter.transform.parent.GetComponent<RectTransform>();
                if (weightCounterParent == null) return;
                HPText.GetComponent<RectTransform>().localRotation = weightCounterParent.localRotation;
                HPText.font = weightCounter.font;
                HPText.fontSize = weightCounter.fontSize;
                HPText.alignment = TextAlignmentOptions.Center;
                HPText.enableAutoSizing = weightCounter.enableAutoSizing;
                HPText.fontSizeMin = weightCounter.fontSizeMin;
                HPText.fontSizeMax = weightCounter.fontSizeMax;
                HPText.fontSharedMaterial = new Material(weightCounter.fontMaterial);
                RectTransform rect = text.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0, 1); 
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
                rect.anchoredPosition = new Vector2(-53, -95);
                HPText.color = weightCounter.color;
            }
            if (HPText == null) return;
            HPText.text = $"HP \n {LethalMenu.localPlayer.health}";
        }
    }
}
