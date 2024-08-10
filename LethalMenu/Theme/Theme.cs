using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LethalMenu.Themes
{
    public class Theme
    {
        public static GUISkin Skin { get; private set; }
        public static AssetBundle AssetBundle;
        public const string name = "Default";

        public static void SetTheme(string t) => LoadTheme(ThemeExists(t) ? t : name);
        private static bool ThemeExists(string t) => Assembly.GetExecutingAssembly().GetManifestResourceStream($"LethalMenu.Resources.Theme.{t}.skin") != null;
        private static AssetBundle LoadAssetBundle(string r) => AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream(r));
        public static string[] GetThemes() => Assembly.GetExecutingAssembly().GetManifestResourceNames().Where(r => r.StartsWith("LethalMenu.Resources.Theme.") && r.EndsWith(".skin")).Select(r => r["LethalMenu.Resources.Theme.".Length..^".skin".Length]).OrderBy(name => name).ToArray();
        private static void LoadTheme(string t)
        {
            AssetBundle?.Unload(true);
            AssetBundle = null;
            Skin = null;
            AssetBundle = LoadAssetBundle($"LethalMenu.Resources.Theme.{t}.skin");
            if (AssetBundle == null) return;
            Skin = AssetBundle.LoadAllAssets<GUISkin>().FirstOrDefault();
            if (Skin == null) return;
        }
    }
}
