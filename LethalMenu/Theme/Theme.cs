using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LethalMenu.Themes
{
    public class Theme
    {
        public static string Name = "Default";
        public static GUISkin? Skin;
        public static AssetBundle? AssetBundle;

        public static string[] GetThemes()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(r => r.StartsWith("LethalMenu.Resources.Theme.") && r.EndsWith(".skin")).Select(r => r["LethalMenu.Resources.Theme.".Length..^".skin".Length]).OrderBy(n => n).ToArray();
        }
    
        public static void SetTheme(string themeName)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"LethalMenu.Resources.Theme.{themeName}.skin");
            if (stream == null)
            {
                Debug.LogError($"[ERROR] Theme {themeName} doesn't exist");
                themeName = "Default";
            }
            if (Name == themeName && Skin != null && AssetBundle != null)
            {
                Debug.LogWarning($"[WARNING] Theme {themeName} already loaded");
                return;
            }
            AssetBundle?.Unload(true);
            AssetBundle = null;
            Skin = null;
            AssetBundle = AssetBundle.LoadFromStream(stream);
            if (AssetBundle == null) return;
            Skin = AssetBundle.LoadAsset<GUISkin>("assets/lethalmenu.guiskin");
            Name = themeName;
            Debug.Log($"Loaded Theme {themeName}");
        }
    }
}
