using System;
using UnityEngine;

namespace LethalMenu.Menu.Core;

internal class MenuFragment
{
    [Obsolete]
    protected static Texture2D GetImage(string url)
    {
        var texture = new Texture2D(1, 1);
        var www = new WWW(url);
        while (!www.isDone)
        {
        }

        www.LoadImageIntoTexture(texture);
        return texture;
    }
}