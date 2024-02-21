using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LethalMenu.Util
{
    public class SkinUtil
    {
        public static GUISkin Skin;

        public static void LoadSkin()
        {
            //load LethalMenu.skin
            string name = "LethalMenu.Resources.Skin.lmskin.skin";

            //check if rss exists
            if (Assembly.GetExecutingAssembly().GetManifestResourceInfo(name) == null)
            {
                Debug.LogError($"Skin not found => {name}");
                return;
            }

            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream skinStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);

            AssetBundle Bundle = AssetBundle.LoadFromStream(skinStream);

            Skin = Bundle.LoadAllAssets<GUISkin>().First();
            
            Debug.Log($"Loaded Skin => {Skin.name}");
            
        }   

    }
}
