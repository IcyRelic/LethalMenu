using LethalMenu.Handler;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;


namespace LethalMenu
{
    public class Loader : MonoBehaviour
    {
        private static GameObject Load;

        public static void Init()
        {
            if (Load != null) return;      
            LoadHarmony();
            ChamHandler.ChamsSetEnabled(true);
            Loader.Load = new GameObject();
            Load.AddComponent<LethalMenu>();
            Object.DontDestroyOnLoad(Loader.Load);
        }

        public static void LoadHarmony()
        {
            String name = "LethalMenu.Resources.0Harmony.dll";
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream stream = assembly.GetManifestResourceStream(name);
            byte[] rawAssembly = new byte[stream.Length];
            stream.Read(rawAssembly, 0, (int)stream.Length);
            AppDomain.CurrentDomain.Load(rawAssembly);
        }

        public static void Unload()
        {
            HackExtensions.ToggleFlags.Keys.ToList().ForEach(h => HackExtensions.ToggleFlags[h] = false);
            ChamHandler.ChamsSetEnabled(false);
            if ((bool)!LethalMenu.localPlayer?.playerActions.Movement.enabled) LethalMenu.localPlayer.playerActions.Enable();
            if (Cursor.visible && !LethalMenu.quickMenuManager.isMenuOpen) Cursor.visible = false;
            LethalMenu.harmony.UnpatchAll("LethalMenu");
            Object.Destroy(Load);
        }
    }
}
