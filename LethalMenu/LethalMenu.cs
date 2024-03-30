using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using LethalMenu.Language;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu;

public class LethalMenu : MonoBehaviour
{
    public static readonly List<GrabbableObject> Items = [];
    public static readonly List<Landmine> Landmines = [];
    public static readonly List<Turret> Turrets = [];
    public static readonly List<EntranceTeleport> Doors = [];
    public static readonly List<PlayerControllerB> Players = [];
    public static readonly List<EnemyAI> Enemies = [];
    public static readonly List<SteamValveHazard> SteamValves = [];
    public static readonly List<TerminalAccessibleObject> BigDoors = [];
    public static readonly List<TerminalAccessibleObject> AllTerminalObjects = [];
    public static readonly List<DoorLock> DoorLocks = [];
    public static readonly List<ShipTeleporter> Teleporters = [];
    public static readonly List<InteractTrigger> InteractTriggers = [];
    public static HangarShipDoor ShipDoor;
    public static BreakerBox Breaker;
    public static PlayerControllerB LocalPlayer;
    public static int SelectedPlayer = -1;


    public static string DebugMessage = "";
    public static string DebugMessage2 = "";
    private static LethalMenu _instance;
    private List<Cheat> _cheats;

    private Harmony _harmony;
    private HackMenu _menu;

    public static LethalMenu Instance
    {
        get
        {
            if (!_instance)
                _instance = new LethalMenu();
            return _instance;
        }
    }


    public void Start()
    {
        _instance = this;
        try
        {
            Localization.Initialize();
            ThemeUtil.LoadTheme("Default");
            LoadCheats();
            DoPatching();
            StartCoroutine(CollectObjects());
        }
        catch
            (Exception e)
        {
            DebugMessage = e.Message + "\n" + e.StackTrace;
        }
    }

    public void Update()
    {
        if (!StartOfRound.Instance || StartOfRound.Instance.inShipPhase) Settings.v_savedLocation = Vector3.zero;

        try
        {
            foreach (Hack hack in Enum.GetValues(typeof(Hack)))
            {
                if (StartOfRound.Instance && LocalPlayer && (LocalPlayer.isTypingChat ||
                                                             LocalPlayer.quickMenuManager.isMenuOpen ||
                                                             LocalPlayer.inTerminalMenu)) continue;


                if (hack.HasKeyBind() && hack.GetKeyBind().wasPressedThisFrame && !hack.IsAnyHackWaiting())
                    hack.Execute();
            }

            if (!StartOfRound.Instance) return;

            _cheats.ForEach(cheat => cheat.Update());
        }
        catch (Exception e)
        {
            DebugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
        }
    }

    public void FixedUpdate()
    {
        try
        {
            if (StartOfRound.Instance) _cheats.ForEach(cheat => cheat.FixedUpdate());
        }
        catch (Exception e)
        {
            DebugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
        }
    }


    public void OnGUI()
    {
        try
        {
            if (Event.current.type == EventType.Repaint)
            {
                VisualUtil.DrawString(new Vector2(5f, 2f),
                    "Lethal Menu " + Settings.Version + " By IcyRelic, and Dustin", Settings.c_primary,
                    false, bold: true, fontSize: 14);

                if (MenuUtil.Resizing)
                {
                    const string rTitle = "SettingsTab.ResizeTitle";
                    const string rConfirm = "SettingsTab.ResizeConfirm";
                    var rSize = $"{HackMenu.Instance.WindowRect.width}x{HackMenu.Instance.WindowRect.height}";

                    VisualUtil.DrawString(new Vector2(Screen.width / 2, 35f),
                        Localization.Localize([rTitle, rConfirm, rSize], true), Settings.c_playerESP, true, true, true,
                        22);
                    MenuUtil.ResizeMenu();
                }


                if (Settings.IsDebugMode)
                {
                    VisualUtil.DrawString(new Vector2(5f, 20f), "[DEBUG MODE]", new RgbaColor(50, 205, 50, 1f), false,
                        false, false, 10);
                    VisualUtil.DrawString(new Vector2(10f, 65f), new RgbaColor(255, 195, 0, 1f).AsString(DebugMessage),
                        false, false, false, 22);
                    VisualUtil.DrawString(new Vector2(10f, 125f),
                        new RgbaColor(255, 195, 0, 1f).AsString(DebugMessage2), false, false, false, 22);
                }

                if ((bool)StartOfRound.Instance) _cheats.ForEach(cheat => cheat.OnGui());
            }


            _menu.Draw();
        }
        catch (Exception e)
        {
            DebugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
        }
    }

    private void DoPatching()
    {
        _harmony = new Harmony("LethalMenu");
        Harmony.DEBUG = false;
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    private void LoadCheats()
    {
        try
        {
            Settings.Changelog.ReadChanges();
            _cheats = new List<Cheat>();
            _menu = new HackMenu();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                         string.Equals(t.Namespace, "LethalMenu.Cheats", StringComparison.Ordinal) &&
                         t.IsSubclassOf(typeof(Cheat)))) _cheats.Add((Cheat)Activator.CreateInstance(type));

            Settings.Config.SaveDefaultConfig();
            Settings.Config.LoadConfig();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogException(e);
        }
    }

    private static IEnumerator CollectObjects()
    {
        while (true)
        {
            CollectObjects(Items);
            CollectObjects(Landmines);
            CollectObjects(Turrets);
            CollectObjects(Doors);
            CollectObjects(Players, obj => !obj.playerUsername.StartsWith("Player #") && !obj.disconnectedMidGame);
            CollectObjects(Enemies);
            CollectObjects(SteamValves);
            CollectObjects(AllTerminalObjects);
            CollectObjects(Teleporters);
            CollectObjects(InteractTriggers);
            CollectObjects(BigDoors, obj => obj.isBigDoor);
            CollectObjects(DoorLocks);

            ShipDoor = FindObjectOfType<HangarShipDoor>();
            Breaker = FindObjectOfType<BreakerBox>();
            LocalPlayer = GameNetworkManager.Instance?.localPlayerController;

            yield return new WaitForSeconds(1f);
        }
    }

    private static void CollectObjects<T>(List<T> list, Func<T, bool> filter = null) where T : MonoBehaviour
    {
        list.Clear();
        list.AddRange(filter == null ? FindObjectsOfType<T>() : FindObjectsOfType<T>().Where(filter));
    }

    public void Unload()
    {
        Loader.Unload();
        StopCoroutine(CollectObjects());
    }
}