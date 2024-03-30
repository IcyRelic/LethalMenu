using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LethalMenu.Cheats;
using LethalMenu.Language;
using LethalMenu.Types;
using LethalMenu.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace LethalMenu;

public static class Settings
{
    public const string Version = "v1.4.1";
    public const bool IsDebugMode = false;
    public static bool IsFirstLaunch = true;

    /* *
     * Menu Settings
     * */
    public static int i_menuFontSize = 14;
    public static int i_menuWidth = 810;
    public static int i_menuHeight = 410;
    public static int i_sliderWidth = 100;
    public static int i_textboxWidth = 85;
    public static float f_menuAlpha = 1f;

    /* *
     * Color Settings
     * */
    public static RgbaColor c_background = new(51, 51, 51, 1f);
    public static RgbaColor c_primary = new(165, 55, 253, 1f);
    public static RgbaColor c_menuText = new(255, 255, 255, 1f);
    public static RgbaColor c_crosshair = new(255, 43, 43, 1f);

    //ESP Colors

    public static RgbaColor c_objectESP = new(255, 255, 255, 1f);
    public static RgbaColor c_playerESP = new(0, 255, 0, 1f);
    public static RgbaColor c_enemyESP = new(255, 0, 0, 1f);
    public static RgbaColor c_shipESP = new(0, 0, 255, 1f);
    public static RgbaColor c_landmineESP = new(255, 0, 0, 1f);
    public static RgbaColor c_turretESP = new(255, 0, 0, 1f);
    public static RgbaColor c_bigDoorESP = new(0, 255, 255, 1f);
    public static RgbaColor c_doorLockESP = new(0.5f, 0.5f, 0.5f, 1f);
    public static RgbaColor c_entranceExitESP = new(0, 0, 255, 1f);
    public static RgbaColor c_steamHazardESP = new(255, 0, 255, 1f);
    public static RgbaColor c_causeOfDeath = new(1f, 47f / 51f, 0.0156862754f, 1f);
    public static RgbaColor c_breakerESP = new(255, 0, 116, 1f);
    public static RgbaColor c_chams = new(238, 111, 255, 0.1f);


    //Other Colors
    public static RgbaColor c_error = new(221, 11, 11, 1f);
    public static RgbaColor c_deadPlayer = new(255, 0, 0, 1);

    /* *
     * Vectors
     *  */
    public static Vector3 v_savedLocation = Vector3.zero;


    /* *
     * Hack Settings
     * */
    public static float f_nvIntensity = 3000f;
    public static float f_nvRange = 10000f;
    public static float f_climbSpeed = 4f;
    public static float f_jumpForce = 5f;
    public static float f_grabDistance = 10000f;
    public static float f_movementSpeed = 0.5f;
    public static float f_noclipSpeed = 10f;
    public static float f_crosshairScale = 14f;
    public static float f_crosshairThickness = 1.75f;
    public static float f_breadcrumbInterval = 1;
    public static float f_espDistance = 5000;
    public static float f_chamDistance = 15;
    public static float f_enemyKillDistance = 15;
    public static float f_fov = 66f;
    public static float f_mouseSensitivity = 0.15f;
    public static float f_inputMovementSpeed = 15f;
    public static bool b_disableSpectatorModels = true;
    public static bool b_useScrapTiers = false;
    public static bool b_VCDisplay;

    public static CrosshairType ct_crosshairType = CrosshairType.Plus;

    public static bool b_chamsObject;
    public static bool b_chamsEnemy;
    public static bool b_chamsPlayer;
    public static bool b_chamsLandmine;
    public static bool b_chamsTurret;
    public static bool b_chamsBigDoor;
    public static bool b_chamsDoorLock;
    public static bool b_chamsSteamHazard;
    public static bool b_chamsBreaker;
    public static bool b_chamsShip;

    public static float f_defaultGrabDistance = -1f;
    public static float f_defaultClimbSpeed = 3f;
    public static float f_defaultJumpForce = 13f;
    public static float f_defaultMovementSpeed = 4.6f;
    public static float f_defaultNightVisionIntensity = 360f;
    public static float f_defaultNightVisionRange = 12f;
    public static float f_defaultFOV = 66f;

    public static int[] i_scrapValueThresholds = { 30, 50, 75, 100 };

    public static RgbaColor[] c_scrapValueColors =
    {
        new(0.5f, 0.5f, 0.5f, 1f),
        new(10, 187, 10, 1f),
        new(255, 0, 255, 1f),
        new(255, 165, 0, 1f)
    };

    public static CursorLockMode clm_lastCursorState = Cursor.lockState;

    public static bool IsMenuOpen
    {
        get => Hack.OpenMenu.IsEnabled();
        set => Hack.OpenMenu.SetToggle(value);
    }

    internal static class Changelog
    {
        public static List<string> Changes;

        public static void ReadChanges()
        {
            Changes = [];

            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("LethalMenu.Resources.Changelog.txt");

            if (stream == null) return;

            using var reader = new StreamReader(stream);
            while (!reader.EndOfStream) Changes.Add(reader.ReadLine());
        }
    }

    internal static class Config
    {
        private const string ConfigJson = "lethelmenu.config.json";
        private const string DefaultConf = "lethalmenu.default.config.json";

        public static void CreateConfigIfNotExists()
        {
            if (HasConfig()) return;

            SaveConfig();
        }

        public static void SaveDefaultConfig()
        {
            SaveConfig(DefaultConf);
        }

        public static bool HasConfig()
        {
            return ConfigJson != null && File.Exists(ConfigJson);
        }

        public static void SaveConfig()
        {
            SaveConfig(ConfigJson);
        }

        public static void SaveConfig(string conf)
        {
            var keybindings = new Dictionary<string, string>();
            var toggles = new Dictionary<string, string>();
            var enemyFilter = new Dictionary<string, string>();


            foreach (var item in HackExtensions.KeyBinds)
            {
                var key = item.Key.ToString();
                var value = item.Value.GetType() == typeof(KeyControl)
                    ? ((KeyControl)item.Value).keyCode.ToString()
                    : item.Value.displayName;

                keybindings.Add(key, value);
            }

            foreach (var item in HackExtensions.ToggleFlags)
            {
                if (item.Key.CanBeExecuted()) continue;

                var key = item.Key.ToString();
                var value = item.Value.ToString();

                toggles.Add(key, value);
            }

            foreach (var item in EnemyAITypeExtensions.EnemyFilter)
            {
                if (item.Key == EnemyAIType.Unknown) continue;
                var key = item.Key.ToString();
                var value = item.Value.ToString();

                enemyFilter.Add(key, value);
            }

            var json = new JObject();
            var settings = new JObject();
            var colors = new JObject();
            var hackSettings = new JObject();
            var chams = new JObject();


            hackSettings["NightVisionIntensity"] = f_nvIntensity.ToString();
            hackSettings["NightVisionRange"] = f_nvRange.ToString();
            hackSettings["ClimbSpeed"] = f_climbSpeed.ToString();
            hackSettings["JumpForce"] = f_jumpForce.ToString();
            hackSettings["GrabDistance"] = f_grabDistance.ToString();
            hackSettings["MovementSpeed"] = f_movementSpeed.ToString();
            hackSettings["NoClipSpeed"] = f_noclipSpeed.ToString();
            hackSettings["BreadcrumbInterval"] = f_breadcrumbInterval.ToString();
            hackSettings["CrosshairThickness"] = f_crosshairThickness.ToString();
            hackSettings["CrosshairType"] = ct_crosshairType.ToString();
            hackSettings["CrosshairScale"] = f_crosshairScale.ToString();
            hackSettings["ESPDistance"] = f_espDistance.ToString();
            hackSettings["DisableSpectatorModels"] = b_disableSpectatorModels.ToString();
            hackSettings["EnemyFilter"] = JObject.FromObject(enemyFilter);
            hackSettings["VCDisplay"] = b_VCDisplay.ToString();
            hackSettings["FOV"] = f_fov.ToString();
            chams["Distance"] = f_chamDistance.ToString();
            chams["Object"] = b_chamsObject.ToString();
            chams["Enemy"] = b_chamsEnemy.ToString();
            chams["Player"] = b_chamsPlayer.ToString();
            chams["Landmine"] = b_chamsLandmine.ToString();
            chams["Turret"] = b_chamsTurret.ToString();
            chams["BigDoor"] = b_chamsBigDoor.ToString();
            chams["DoorLock"] = b_chamsDoorLock.ToString();
            chams["SteamHazard"] = b_chamsSteamHazard.ToString();
            chams["Breaker"] = b_chamsBreaker.ToString();
            chams["Ship"] = b_chamsShip.ToString();
            hackSettings["Chams"] = chams;


            colors["Background"] = JsonConvert.SerializeObject(c_background);
            colors["Primary"] = JsonConvert.SerializeObject(c_primary);
            colors["MenuText"] = JsonConvert.SerializeObject(c_menuText);
            colors["Crosshair"] = JsonConvert.SerializeObject(c_crosshair);
            colors["Chams"] = JsonConvert.SerializeObject(c_chams);
            colors["ObjectESP"] = JsonConvert.SerializeObject(c_objectESP);
            colors["PlayerESP"] = JsonConvert.SerializeObject(c_playerESP);
            colors["EnemyESP"] = JsonConvert.SerializeObject(c_enemyESP);
            colors["ShipESP"] = JsonConvert.SerializeObject(c_shipESP);
            colors["BreakerESP"] = JsonConvert.SerializeObject(c_breakerESP);
            colors["LandmineESP"] = JsonConvert.SerializeObject(c_landmineESP);
            colors["TurretESP"] = JsonConvert.SerializeObject(c_turretESP);
            colors["BigDoorESP"] = JsonConvert.SerializeObject(c_bigDoorESP);
            colors["DoorLockESP"] = JsonConvert.SerializeObject(c_doorLockESP);
            colors["EntranceExitESP"] = JsonConvert.SerializeObject(c_entranceExitESP);
            colors["SteamHazardESP"] = JsonConvert.SerializeObject(c_steamHazardESP);
            colors["CauseOfDeath"] = JsonConvert.SerializeObject(c_causeOfDeath);

            settings["FirstLaunch"] = IsFirstLaunch.ToString();
            settings["MenuFontSize"] = i_menuFontSize.ToString();
            settings["MenuWidth"] = i_menuWidth.ToString();
            settings["MenuHeight"] = i_menuHeight.ToString();
            settings["SliderWidth"] = i_sliderWidth.ToString();
            settings["TextboxWidth"] = i_textboxWidth.ToString();
            settings["MenuAlpha"] = f_menuAlpha.ToString();


            json["Language"] = Localization.Language.Name;
            json["Colors"] = colors;
            json["HackSettings"] = hackSettings;
            json["MenuSettings"] = settings;
            json["KeyBinds"] = JObject.FromObject(keybindings);
            json["Toggles"] = JObject.FromObject(toggles);


            File.WriteAllText(conf, json.ToString());
        }

        public static void LoadConfig()
        {
            CreateConfigIfNotExists();

            var jsonStr = File.ReadAllText(ConfigJson);


            var json = JObject.Parse(jsonStr);

            if (json.TryGetValue("Language", out var languageToken))
                Localization.SetLanguage(languageToken.ToString());

            if (json.TryGetValue("HackSettings", out var hackSettingsToken))
            {
                var hackSettings = hackSettingsToken.ToObject<JObject>();

                if (hackSettings.TryGetValue("NightVisionIntensity", out var nightVisionIntensityToken))
                    f_nvIntensity = float.Parse(nightVisionIntensityToken.ToString());
                if (hackSettings.TryGetValue("NightVisionRange", out var nightVisionRangeToken))
                    f_nvRange = float.Parse(nightVisionRangeToken.ToString());
                if (hackSettings.TryGetValue("ClimbSpeed", out var climbSpeedToken))
                    f_climbSpeed = float.Parse(climbSpeedToken.ToString());
                if (hackSettings.TryGetValue("JumpForce", out var jumpForceToken))
                    f_jumpForce = float.Parse(jumpForceToken.ToString());
                if (hackSettings.TryGetValue("GrabDistance", out var grabDistanceToken))
                    f_grabDistance = float.Parse(grabDistanceToken.ToString());
                if (hackSettings.TryGetValue("MovementSpeed", out var movementSpeedToken))
                    f_movementSpeed = float.Parse(movementSpeedToken.ToString());
                if (hackSettings.TryGetValue("NoClipSpeed", out var noClipSpeedToken))
                    f_noclipSpeed = float.Parse(noClipSpeedToken.ToString());
                if (hackSettings.TryGetValue("BreadcrumbInterval", out var breadcrumbIntervalToken))
                    f_breadcrumbInterval = int.Parse(breadcrumbIntervalToken.ToString());
                if (hackSettings.TryGetValue("CrosshairThickness", out var crosshairThicknessToken))
                    f_crosshairThickness = float.Parse(crosshairThicknessToken.ToString());
                if (hackSettings.TryGetValue("CrosshairType", out var crosshairTypeToken))
                    ct_crosshairType = (CrosshairType)Enum.Parse(typeof(CrosshairType), crosshairTypeToken.ToString());
                if (hackSettings.TryGetValue("CrosshairScale", out var crosshairScaleToken))
                    f_crosshairScale = float.Parse(crosshairScaleToken.ToString());
                if (hackSettings.TryGetValue("ESPDistance", out var espDistanceToken))
                    f_espDistance = int.Parse(espDistanceToken.ToString());
                if (hackSettings.TryGetValue("DisableSpectatorModels", out var disableSpectatorModelsToken))
                    b_disableSpectatorModels = bool.Parse(disableSpectatorModelsToken.ToString());
                if (hackSettings.TryGetValue("VCDisplay", out var vcDisplayToken))
                    b_VCDisplay = bool.Parse(vcDisplayToken.ToString());
                if (hackSettings.TryGetValue("FOV", out var fovToken))
                    f_fov = float.Parse(fovToken.ToString());


                if (hackSettings.TryGetValue("EnemyFilter", out var enemyFilterToken))
                    foreach (var (sType, sValue) in enemyFilterToken.ToObject<Dictionary<string, string>>())
                    {
                        if (!Enum.TryParse(sType, out EnemyAIType type)) continue;
                        if (type == EnemyAIType.Unknown) continue;
                        var value = bool.TryParse(sValue, out var result) && result;

                        EnemyAITypeExtensions.EnemyFilter[type] = value;
                    }

                if (hackSettings.TryGetValue("Chams", out var chamsToken))
                {
                    var chams = chamsToken.ToObject<JObject>();
                    if (chams.TryGetValue("Distance", out var distanceToken))
                        f_chamDistance = float.Parse(distanceToken.ToString());
                    if (chams.TryGetValue("Object", out var objectToken))
                        b_chamsObject = bool.Parse(objectToken.ToString());
                    if (chams.TryGetValue("Enemy", out var enemyToken))
                        b_chamsEnemy = bool.Parse(enemyToken.ToString());
                    if (chams.TryGetValue("Player", out var playerToken))
                        b_chamsPlayer = bool.Parse(playerToken.ToString());
                    if (chams.TryGetValue("Landmine", out var landmineToken))
                        b_chamsLandmine = bool.Parse(landmineToken.ToString());
                    if (chams.TryGetValue("Turret", out var turretToken))
                        b_chamsTurret = bool.Parse(turretToken.ToString());
                    if (chams.TryGetValue("BigDoor", out var bigDoorToken))
                        b_chamsBigDoor = bool.Parse(bigDoorToken.ToString());
                    if (chams.TryGetValue("DoorLock", out var doorLockToken))
                        b_chamsDoorLock = bool.Parse(doorLockToken.ToString());
                    if (chams.TryGetValue("SteamHazard", out var steamHazardToken))
                        b_chamsSteamHazard = bool.Parse(steamHazardToken.ToString());
                    if (chams.TryGetValue("Breaker", out var breakerToken))
                        b_chamsBreaker = bool.Parse(breakerToken.ToString());
                    if (chams.TryGetValue("Ship", out var shipToken))
                        b_chamsShip = bool.Parse(shipToken.ToString());
                }
            }

            if (json.TryGetValue("Colors", out var colorsToken))
            {
                var colors = colorsToken.ToObject<JObject>();

                if (colors.TryGetValue("Background", out var backgroundToken))
                    c_background = JsonConvert.DeserializeObject<RgbaColor>(backgroundToken.ToString());
                if (colors.TryGetValue("Primary", out var primaryToken))
                    c_primary = JsonConvert.DeserializeObject<RgbaColor>(primaryToken.ToString());
                if (colors.TryGetValue("MenuText", out var menuTextToken))
                    c_menuText = JsonConvert.DeserializeObject<RgbaColor>(menuTextToken.ToString());
                if (colors.TryGetValue("Crosshair", out var crosshairToken))
                    c_crosshair = JsonConvert.DeserializeObject<RgbaColor>(crosshairToken.ToString());
                if (colors.TryGetValue("Chams", out var chams))
                    c_chams = JsonConvert.DeserializeObject<RgbaColor>(chams.ToString());
                if (colors.TryGetValue("ObjectESP", out var objectESP))
                    c_objectESP = JsonConvert.DeserializeObject<RgbaColor>(objectESP.ToString());
                if (colors.TryGetValue("PlayerESP", out var playerESP))
                    c_playerESP = JsonConvert.DeserializeObject<RgbaColor>(playerESP.ToString());
                if (colors.TryGetValue("EnemyESP", out var enemyESP))
                    c_enemyESP = JsonConvert.DeserializeObject<RgbaColor>(enemyESP.ToString());
                if (colors.TryGetValue("ShipESP", out var shipESP))
                    c_shipESP = JsonConvert.DeserializeObject<RgbaColor>(shipESP.ToString());
                if (colors.TryGetValue("BreakerESP", out var breakerESP))
                    c_breakerESP = JsonConvert.DeserializeObject<RgbaColor>(breakerESP.ToString());
                if (colors.TryGetValue("LandmineESP", out var landmineESP))
                    c_landmineESP = JsonConvert.DeserializeObject<RgbaColor>(landmineESP.ToString());
                if (colors.TryGetValue("TurretESP", out var turretESP))
                    c_turretESP = JsonConvert.DeserializeObject<RgbaColor>(turretESP.ToString());
                if (colors.TryGetValue("BigDoorESP", out var bigDoorESP))
                    c_bigDoorESP = JsonConvert.DeserializeObject<RgbaColor>(bigDoorESP.ToString());
                if (colors.TryGetValue("DoorLockESP", out var doorLockESP))
                    c_doorLockESP = JsonConvert.DeserializeObject<RgbaColor>(doorLockESP.ToString());
                if (colors.TryGetValue("EntranceExitESP", out var entranceExitESP))
                    c_entranceExitESP = JsonConvert.DeserializeObject<RgbaColor>(entranceExitESP.ToString());
                if (colors.TryGetValue("SteamHazardESP", out var steamHazardESP))
                    c_steamHazardESP = JsonConvert.DeserializeObject<RgbaColor>(steamHazardESP.ToString());
                if (colors.TryGetValue("CauseOfDeath", out var causeOfDeath))
                    c_causeOfDeath = JsonConvert.DeserializeObject<RgbaColor>(causeOfDeath.ToString());
            }

            if (json.TryGetValue("MenuSettings", out var settingsToken))
            {
                var settings = settingsToken.ToObject<JObject>();

                if (settings.TryGetValue("FirstLaunch", out var firstLaunchToken))
                    IsFirstLaunch = bool.Parse(firstLaunchToken.ToString());
                if (settings.TryGetValue("MenuFontSize", out var menuFontSizeToken))
                    i_menuFontSize = int.Parse(menuFontSizeToken.ToString());
                if (settings.TryGetValue("MenuWidth", out var menuWidthToken))
                    i_menuWidth = int.Parse(menuWidthToken.ToString());
                if (settings.TryGetValue("MenuHeight", out var menuHeightToken))
                    i_menuHeight = int.Parse(menuHeightToken.ToString());
                if (settings.TryGetValue("SliderWidth", out var sliderWidthToken))
                    i_sliderWidth = int.Parse(sliderWidthToken.ToString());
                if (settings.TryGetValue("TextboxWidth", out var textboxWidthToken))
                    i_textboxWidth = int.Parse(textboxWidthToken.ToString());
                if (settings.TryGetValue("MenuAlpha", out var menuAlphaToken))
                    f_menuAlpha = float.Parse(menuAlphaToken.ToString());
            }


            if (json.TryGetValue("KeyBinds", out var keybindsToken))
            {
                HackExtensions.KeyBinds.Clear();

                ButtonControl[] mouseButtons =
                [
                    Mouse.current.leftButton, Mouse.current.rightButton, Mouse.current.middleButton,
                    Mouse.current.forwardButton, Mouse.current.backButton
                ];


                foreach (var (sHack, sKey) in keybindsToken.ToObject<Dictionary<string, string>>())
                {
                    var mouseBtn = mouseButtons.FirstOrDefault(k => k.displayName == sKey);

                    ButtonControl key = Keyboard.current.allKeys.FirstOrDefault(k => k.keyCode.ToString() == sKey);


                    if (!Enum.TryParse(sHack, out Hack hack)) continue;
                    if (mouseBtn == null && key == null) continue;

                    hack.SetKeyBind(mouseBtn ?? key);
                }
            }


            if (!json.TryGetValue("Toggles", out var togglesToken)) return;
            {
                foreach (var (sHack, sKey) in togglesToken.ToObject<Dictionary<string, string>>())
                {
                    if (!Enum.TryParse(sHack, out Hack hack)) continue;
                    if (hack.CanBeExecuted()) continue;

                    var toggle = bool.TryParse(sKey, out var result) && result;

                    hack.SetToggle(toggle);
                }
            }
        }

        public static void RegenerateConfig()
        {
            if (HasConfig()) File.Delete(ConfigJson);
            File.Copy(DefaultConf, ConfigJson);

            HackExtensions.KeyBinds.Clear();

            LoadConfig();
        }
    }
}