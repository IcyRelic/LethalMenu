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
    public static List<GrabbableObject> items = new();
    public static List<Landmine> landmines = new();
    public static List<Turret> turrets = new();
    public static List<EntranceTeleport> doors = new();
    public static List<PlayerControllerB> players = new();
    public static List<EnemyAI> enemies = new();
    public static List<SteamValveHazard> steamValves = new();
    public static List<TerminalAccessibleObject> bigDoors = new();
    public static List<TerminalAccessibleObject> allTerminalObjects = new();
    public static List<DoorLock> doorLocks = new();
    public static List<ShipTeleporter> teleporters = new();
    public static List<InteractTrigger> interactTriggers = new();
    public static HangarShipDoor shipDoor;
    public static BreakerBox breaker;
    public static PlayerControllerB localPlayer;
    public static int selectedPlayer = -1;


    public static string debugMessage = "";
    public static string debugMessage2 = "";
    private static LethalMenu instance;
    private List<Cheat> cheats;

    private Harmony harmony;
    private HackMenu menu;

    public static LethalMenu Instance
    {
        get
        {
            if (instance == null)
                instance = new LethalMenu();
            return instance;
        }
    }


    public void Start()
    {
        instance = this;
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
            debugMessage = e.Message + "\n" + e.StackTrace;
        }
    }

    public void Update()
    {
        if (!(bool)StartOfRound.Instance || StartOfRound.Instance.inShipPhase) Settings.v_savedLocation = Vector3.zero;

        try
        {
            foreach (Hack hack in Enum.GetValues(typeof(Hack)))
            {
                if ((bool)StartOfRound.Instance && localPlayer != null && (localPlayer.isTypingChat ||
                                                                           localPlayer.quickMenuManager.isMenuOpen ||
                                                                           localPlayer.inTerminalMenu)) continue;


                if (hack.HasKeyBind() && hack.GetKeyBind().wasPressedThisFrame && !hack.IsAnyHackWaiting())
                    hack.Execute();
            }

            if (!(bool)StartOfRound.Instance) return;

            cheats.ForEach(cheat => cheat.Update());
        }
        catch (Exception e)
        {
            debugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
        }
    }

    public void FixedUpdate()
    {
        try
        {
            if ((bool)StartOfRound.Instance) cheats.ForEach(cheat => cheat.FixedUpdate());
        }
        catch (Exception e)
        {
            debugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
        }
    }


    public void OnGUI()
    {
        try
        {
            if (Event.current.type == EventType.Repaint)
            {
                VisualUtil.DrawString(new Vector2(5f, 2f),
                    "Lethal Menu " + Settings.version + " By IcyRelic, and Dustin", Settings.c_primary,
                    false, bold: true, fontSize: 14);

                if (MenuUtil.Resizing)
                {
                    var rTitle = "SettingsTab.ResizeTitle";
                    var rConfirm = "SettingsTab.ResizeConfirm";
                    var rSize = $"{HackMenu.Instance.WindowRect.width}x{HackMenu.Instance.WindowRect.height}";

                    VisualUtil.DrawString(new Vector2(Screen.width / 2, 35f),
                        Localization.Localize([rTitle, rConfirm, rSize], true), Settings.c_playerESP, true, true, true,
                        22);
                    MenuUtil.ResizeMenu();
                }


                if (Settings.isDebugMode)
                {
                    VisualUtil.DrawString(new Vector2(5f, 20f), "[DEBUG MODE]", new RgbaColor(50, 205, 50, 1f), false,
                        false, false, 10);
                    VisualUtil.DrawString(new Vector2(10f, 65f), new RgbaColor(255, 195, 0, 1f).AsString(debugMessage),
                        false, false, false, 22);
                    VisualUtil.DrawString(new Vector2(10f, 125f),
                        new RgbaColor(255, 195, 0, 1f).AsString(debugMessage2), false, false, false, 22);
                }

                if ((bool)StartOfRound.Instance) cheats.ForEach(cheat => cheat.OnGui());
            }


            menu.Draw();
        }
        catch (Exception e)
        {
            debugMessage = "Msg: " + e.Message + "\nSrc: " + e.Source + "\n" + e.StackTrace;
        }
    }

    private void DoPatching()
    {
        harmony = new Harmony("LethalMenu");
        Harmony.DEBUG = false;
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    private void LoadCheats()
    {
        try
        {
            Settings.Changelog.ReadChanges();
            cheats = new List<Cheat>();
            menu = new HackMenu();
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(t =>
                         string.Equals(t.Namespace, "LethalMenu.Cheats", StringComparison.Ordinal) &&
                         t.IsSubclassOf(typeof(Cheat)))) cheats.Add((Cheat)Activator.CreateInstance(type));

            Settings.Config.SaveDefaultConfig();
            Settings.Config.LoadConfig();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogException(e);
        }
    }

    public IEnumerator CollectObjects()
    {
        while (true)
        {
            CollectObjects(items);
            CollectObjects(landmines);
            CollectObjects(turrets);
            CollectObjects(doors);
            CollectObjects(players, obj => !obj.playerUsername.StartsWith("Player #") && !obj.disconnectedMidGame);
            CollectObjects(enemies);
            CollectObjects(steamValves);
            CollectObjects(allTerminalObjects);
            CollectObjects(teleporters);
            CollectObjects(interactTriggers);
            CollectObjects(bigDoors, obj => obj.isBigDoor);
            CollectObjects(doorLocks);

            shipDoor = FindObjectOfType<HangarShipDoor>();
            breaker = FindObjectOfType<BreakerBox>();
            localPlayer = GameNetworkManager.Instance?.localPlayerController;

            yield return new WaitForSeconds(1f);
        }
    }

    private void CollectObjects<T>(List<T> list, Func<T, bool> filter = null) where T : MonoBehaviour
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