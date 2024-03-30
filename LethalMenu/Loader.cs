using System;
using System.Reflection;
using UnityEngine;

namespace LethalMenu;

public class Loader : MonoBehaviour
{
    private static GameObject _load;
    public static bool HarmonyLoaded;
    private static int _read;

    public static void Init()
    {
        LoadHarmony();
        _load = new GameObject();
        _load.AddComponent<LethalMenu>();
        DontDestroyOnLoad(_load);
    }

    private static void LoadHarmony()
    {
        const string name = "LethalMenu.Resources.0Harmony.dll";
        var assembly = Assembly.GetExecutingAssembly();
        var stream = assembly.GetManifestResourceStream(name);
        if (stream != null)
        {
            var rawAssembly = new byte[stream.Length];
            _read = stream.Read(rawAssembly, 0, (int)stream.Length);

            AppDomain.CurrentDomain.Load(rawAssembly);
        }

        HarmonyLoaded = true;
    }

    public static void Unload()
    {
        Destroy(_load);
    }
}