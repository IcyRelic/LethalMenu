using System.Collections;
using UnityEngine;

namespace LethalMenu.Menu.Core
{
    internal class MenuFragment
    {
        [System.Obsolete]
        public Texture2D GetImage(string url)
        {
            Texture2D texture = new Texture2D(1, 1);
            WWW www = new WWW(url);
            while (!www.isDone) { }
            www.LoadImageIntoTexture(texture);
            return texture;
        }
        
    }
}
