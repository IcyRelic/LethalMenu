using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LethalMenu.Util
{
    public class ThemeUtil
    {
        public static GUISkin Skin;
        private static string DefaultThemeName = "Default";
        private static AssetBundle AssetBundle;

        public static void SetTheme(string name)
        {
            if (ThemeExists(name))
            {
                ApplyTheme(name);
            }
            else
            {
                ApplyTheme("Default");
            }
        }

        private static bool ThemeExists(string themename)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream themestream = assembly.GetManifestResourceStream($"LethalMenu.Resources.Theme.{themename}.skin"))
            {
                return themestream != null;
            }
        }


        public static void LoadTheme(string themeName)
        {
            if (AssetBundle != null)
            {
                AssetBundle.Unload(true);
                AssetBundle = null;
                Skin = null;
            }

            string resourceName = $"LethalMenu.Resources.Theme.{themeName}.skin";

            try
            {
                AssetBundle = LoadAssetBundle(resourceName);
                if (AssetBundle == null)
                {
                    Debug.LogError($"Failed to load theme AssetBundle for {themeName}");
                    return;
                }

                Skin = AssetBundle.LoadAllAssets<GUISkin>().FirstOrDefault();
                if (Skin == null)
                {
                    Debug.LogError($"Failed to load GUISkin from AssetBundle for {themeName}");
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load theme: {e.Message}\n{e.StackTrace}");
            }
        }

        private static AssetBundle LoadAssetBundle(string resourceName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (Stream themeStream = assembly.GetManifestResourceStream(resourceName))
            {
                return AssetBundle.LoadFromStream(themeStream);
            }
        }

        public static string ThemeName
        {
            get => DefaultThemeName;
            set
            {
                DefaultThemeName = value;
                LoadTheme(value);
            }
        }

        public static void ApplyTheme(string themeName)
        {
            ThemeName = themeName;
        }
    }
}
