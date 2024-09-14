using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace LethalMenu.Menu.Core
{
    internal class MenuFragment
    {
        public void GetImage(string url, Action<Texture2D> Action)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            AsyncOperation operation = www.SendWebRequest();
            operation.completed += (op) =>
            {
                if (www.result != UnityWebRequest.Result.Success) return;
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                Action?.Invoke(texture);
            };
        }

        public static void SetEnabled(bool enabled)
        {
            bool MSent = false;
            if (enabled && !MSent)
            {
                MSent = true;
                CheckForMessage();
            }
            else if (!enabled && MSent) MSent = false;
        }

        public static void CheckForMessage()
        {
            UnityWebRequest www = UnityWebRequest.Get("https://icyrelic.com/release/lethalmenu/message.json");
            www.SendWebRequest().completed += (op) =>
            {
                if (www.result != UnityWebRequest.Result.Success) return;
                JObject json = JObject.Parse(www.downloadHandler.text);
                if (!json["Show"].Value<bool>() || HUDManager.Instance == null) return;
                HUDManager.Instance.DisplayTip("LethalMenu", json["Message"].Value<string>());
            };
        }
    }
}