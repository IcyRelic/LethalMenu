using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LethalMenu.Util
{
    public class SkinUtil
    {
        public static GUISkin Skin;
        private static string DefaultSkinName = "Default";
        private static AssetBundle AssetBundle;

        public static void LoadSkin(string skinName)
        {
            if (AssetBundle != null)
            {
                AssetBundle.Unload(true);
                AssetBundle = null;
                Skin = null;
            }

            string resourceName = $"LethalMenu.Resources.Skin.{skinName}.skin";

            try
            {
                AssetBundle = LoadAssetBundle(resourceName);
                if (AssetBundle == null)
                {
                    Debug.LogError($"Failed to load skin AssetBundle for {skinName}");
                    return;
                }

                Skin = AssetBundle.LoadAllAssets<GUISkin>().FirstOrDefault();
                if (Skin == null)
                {
                    Debug.LogError($"Failed to load Skin from AssetBundle for {skinName}");
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load skin: {e.Message}\n{e.StackTrace}");
            }
        }

        private static AssetBundle LoadAssetBundle(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream skinStream = assembly.GetManifestResourceStream(resourceName))
            {
                return AssetBundle.LoadFromStream(skinStream);
            }
        }

        public static string SkinName
        {
            get => DefaultSkinName;
            set
            {
                DefaultSkinName = value;
                LoadSkin(value);
            }
        }

        public static void ApplySkin(string skinName)
        {
            SkinName = skinName;
        }
    }
}
