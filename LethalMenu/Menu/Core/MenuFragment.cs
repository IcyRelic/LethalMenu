using System;
using UnityEngine;
using UnityEngine.Networking;

namespace LethalMenu.Menu.Core;

internal class MenuFragment
{
    [Obsolete]
    protected static Texture2D GetImageDeprecated(string url)
    {
        var texture = new Texture2D(1, 1);
        var www = new WWW(url);
        while (!www.isDone)
        {
        }

        www.LoadImageIntoTexture(texture);
        return texture;
    }

    protected static Texture2D GetImage(string url)
    {
        var request = UnityWebRequestTexture.GetTexture(url);
        request.SendWebRequest();
        while (!request.isDone)
        {
        }

        var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        return texture;
    }
}