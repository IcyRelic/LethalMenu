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
    }
}