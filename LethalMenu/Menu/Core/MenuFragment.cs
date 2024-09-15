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
                if (www.result != UnityWebRequest.Result.Success || HUDManager.Instance == null) return;
                JObject json = JObject.Parse(www.downloadHandler.text);
                void DisplayMessage(string key)
                {
                    if (json[key] != null && json[key]["Show"].Value<bool>()) 
                        HUDManager.Instance.DisplayTip("LethalMenu", json[key]["Message"].Value<string>());
                }
                if (json["global"]["Show"].Value<bool>())
                {
                    DisplayMessage("global");
                    System.Threading.Tasks.Task.Delay(8000).ContinueWith(_ => DisplayMessage(Settings.version ?? "default"));
                }
                else DisplayMessage(Settings.version ?? "default");
            };
        }
    }
}