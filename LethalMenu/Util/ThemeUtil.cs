using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace LethalMenu.Util;

public static class ThemeUtil
{
    public static GUISkin Skin;
    private static string _defaultThemeName = "Default";
    private static AssetBundle _assetBundle;

    public static string ThemeName
    {
        set
        {
            _defaultThemeName = value;
            LoadTheme(value);
        }
    }

    public static void LoadTheme(string themeName)
    {
        if (_assetBundle != null)
        {
            _assetBundle.Unload(true);
            _assetBundle = null;
            Skin = null;
        }

        var resourceName = $"LethalMenu.Resources.Theme.{themeName}.skin";

        try
        {
            _assetBundle = LoadAssetBundle(resourceName);
            if (_assetBundle == null)
            {
                Debug.LogError($"Failed to load theme AssetBundle for {themeName}");
                return;
            }

            Skin = _assetBundle.LoadAllAssets<GUISkin>().FirstOrDefault();
            if (Skin == null) Debug.LogError($"Failed to load GUISkin from AssetBundle for {themeName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load theme: {e.Message}\n{e.StackTrace}");
        }
    }

    private static AssetBundle LoadAssetBundle(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var themeStream = assembly.GetManifestResourceStream(resourceName);
        return AssetBundle.LoadFromStream(themeStream);
    }

    public static void ApplyTheme(string themeName)
    {
        ThemeName = themeName;
    }
}