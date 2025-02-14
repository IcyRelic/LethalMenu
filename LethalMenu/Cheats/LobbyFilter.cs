using UnityEngine;
using UnityEngine.UI;
using TMPro;
using HarmonyLib;
using System.Collections;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class LobbyListPatch : Cheat
    {
        [HarmonyPatch(typeof(LobbySlot), "Awake")]
        [HarmonyPostfix]
        public static void Awake(LobbySlot __instance)
        {
            __instance.StartCoroutine(CheckLobby(__instance));
        }

        private static IEnumerator CheckLobby(LobbySlot slot)
        {
            yield return new WaitForEndOfFrame();
            if (!string.IsNullOrEmpty(slot.thisLobby.GetData("name")) && AntiKick.HostKickedPlayerList.Contains(slot.thisLobby.Owner.Id) && Settings.b_DisplayHostKickedLobbies) ApplyChanges(slot, "Host Kicked", Color.red);
            if (!string.IsNullOrEmpty(slot.thisLobby.GetData("name")) && slot.LobbyName.text.Contains('\x200C') && Settings.b_DisplayLMUsers) ApplyChanges(slot, "Lethal Menu User", Color.magenta);
        }

        private static void ApplyChanges(LobbySlot slot, string text, Color color)
        {
            if (!slot.transform.name.Contains("Challenge")) slot.GetComponent<Image>().color = color;
            GameObject @object = new GameObject(text);
            @object.transform.SetParent(slot.GetComponent<Image>().transform, false);
            TextMeshProUGUI tmp = @object.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.font = slot.playerCount.font;
            tmp.fontSize = 15;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = slot.playerCount.color;
            tmp.enableWordWrapping = false;
            RectTransform rect = @object.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(400, 50); 
            rect.anchorMin = new Vector2(0.5f, 0);
            rect.anchorMax = new Vector2(0.5f, 0);
            rect.pivot = new Vector2(0.5f, 0);
            rect.anchoredPosition = new Vector2(0, -10);
        }
    }
}
