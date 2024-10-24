using Newtonsoft.Json.Linq;
using System;
using UnityEngine;
using UnityEngine.Networking;
using LethalMenu.Menu.Popup;
using System.Threading.Tasks;

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

        public static void FetchNotification()
        {
            UnityWebRequest www = UnityWebRequest.Get("https://icyrelic.com/release/lethalmenu/message.json");
            www.SendWebRequest().completed += (op) =>
            {
                if (www.result != UnityWebRequest.Result.Success) return;
                JObject json = JObject.Parse(www.downloadHandler.text);
                bool globalMessage = json["global"]["Show"].Value<bool>();
                DisplayMessage(json, globalMessage ? "global" : (Settings.version ?? "default"));
            };
        }

        public static async void InjectNotification(GameObject Load)
        {
            GameObject _Load = Load;
            while (!Hack.OpenMenu.IsEnabled()) await Task.Delay(2000);
            DisplayMessage(_Load != null ? "Lethal Menu is already injected!" : "Lethal Menu injected!");
            if (_Load == null) FetchNotification();
        }

        private static void DisplayMessage(JObject json, string msg) => DisplayMessage(json[msg]?["Show"].Value<bool>() == true ? json[msg]["Message"].Value<string>() : null);
        public static void DisplayMessage(string msg) => NotificationWindow.Notification("Lethal Menu", msg);
    }
}