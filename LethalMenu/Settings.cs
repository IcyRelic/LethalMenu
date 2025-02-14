using LethalMenu.Cheats;
using LethalMenu.Language;
using LethalMenu.Menu.Core;
using LethalMenu.Themes;
using LethalMenu.Types;
using LethalMenu.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace LethalMenu
{
    internal class Settings
    {
        public static string version = "v1.4.6";
        public static bool DebugMode = false;
        public static bool isFirstLaunch = true;
        public static bool isMenuOpen
        {
            get { return Hack.OpenMenu.IsEnabled(); }
            set { Hack.OpenMenu.SetToggle(value);  }
        }

        /* * * * * * * * *
         * Menu Settings *
         * * * * * * * * */
        public static int i_menuFontSize = 14;
        public static int i_menuWidth = 810;
        public static int  i_menuHeight = 410;
        public static int i_sliderWidth = 100;
        public static int i_textboxWidth = 85;
        public static float f_menuAlpha = 1f;
        public static int i_screenFontSize = 14;

        /* * * * * * * * * *
         *  Color Settings *
         * * * * * * * * * */
        public static RGBAColor c_background = new RGBAColor(51, 51, 51, 1f);
        public static RGBAColor c_primary = new RGBAColor(165, 55, 253, 1f);
        public static RGBAColor c_menuText = new RGBAColor(255, 255, 255, 1f);
        public static RGBAColor c_crosshair = new RGBAColor(255, 43, 43, 1f);
        public static RGBAColor c_hackhighlight = new RGBAColor(165, 55, 253, 1f);

        /* * * * * * * *
         * ESP  Colors *
         * * * * * * * */
        public static RGBAColor c_objectESP = new RGBAColor(255, 255, 255, 1f);
        public static RGBAColor c_playerESP = new RGBAColor(0, 255, 0, 1f);
        public static RGBAColor c_bodyESP = new RGBAColor(255, 165, 0, 1f);
        public static RGBAColor c_enemyESP = new RGBAColor(255, 0, 0, 1f);
        public static RGBAColor c_shipESP = new RGBAColor(0, 0, 255, 1f);
        public static RGBAColor c_landmineESP = new RGBAColor(255, 0, 0, 1f);
        public static RGBAColor c_turretESP = new RGBAColor(255, 0, 0, 1f);
        public static RGBAColor c_bigDoorESP = new RGBAColor(0, 255, 255, 1f);
        public static RGBAColor c_doorLockESP = new RGBAColor(128, 128, 128, 255);
        public static RGBAColor c_entranceExitESP = new RGBAColor(0, 0, 255, 1f);
        public static RGBAColor c_steamHazardESP = new RGBAColor(255, 0, 255, 1f);
        public static RGBAColor c_breakerESP = new RGBAColor(255, 0, 116, 1f);
        public static RGBAColor c_spikeRoofTrapESP = new RGBAColor(139, 69, 19, 1f);
        public static RGBAColor c_vainShroudESP = new RGBAColor(102, 51, 153, 0.1f);
        public static RGBAColor c_enemyVentESP = new RGBAColor(211, 211, 211, 0.1f);
        public static RGBAColor c_itemDropShipESP = new RGBAColor(199, 21, 133, 0.1f);
        public static RGBAColor c_CruiserESP = new RGBAColor(255, 215, 0, 0.1f);

        public static RGBAColor c_mineshaftElevatorESP = new RGBAColor(0, 0, 255, 1f);

        /* * * * * * * * *
         * Chams  Colors *
         * * * * * * * * */
        public static RGBAColor c_objectChams = new RGBAColor(255, 255, 255, 0.1f);
        public static RGBAColor c_playerChams = new RGBAColor(0, 255, 0, 0.1f);
        public static RGBAColor c_enemyChams = new RGBAColor(255, 0, 0, 0.1f);
        public static RGBAColor c_shipChams = new RGBAColor(0, 0, 255, 0.1f);
        public static RGBAColor c_landmineChams = new RGBAColor(255, 0, 0, 0.1f);
        public static RGBAColor c_turretChams = new RGBAColor(255, 0, 0, 0.1f);
        public static RGBAColor c_bigDoorChams = new RGBAColor(0, 255, 255, 0.1f);
        public static RGBAColor c_doorLockChams = new RGBAColor(0.5f, 0.5f, 0.5f, 0.1f);
        public static RGBAColor c_steamHazardChams = new RGBAColor(255, 0, 255, 0.1f);
        public static RGBAColor c_breakerChams = new RGBAColor(255, 0, 116, 0.1f);
        public static RGBAColor c_enemyVentChams = new RGBAColor(211, 211, 211, 0.1f);
        public static RGBAColor c_vainShroudChams = new RGBAColor(102, 51, 153, 0.1f);
        public static RGBAColor c_spikeRoofTrapChams = new RGBAColor(139, 69, 19, 0.1f);
        public static RGBAColor c_itemDropShipChams = new RGBAColor(199, 21, 133, 0.1f); 
        public static RGBAColor c_CruiserChams = new RGBAColor(255, 215, 0, 0.1f);   
        public static RGBAColor c_chams = new RGBAColor(238, 111, 255, 0.1f);

        public static RGBAColor c_mineshaftElevatorChams = new RGBAColor(0, 0, 255, 0.1f);

        /* * * * * * * * *
         *  Other Colors *
         * * * * * * * * */
        public static RGBAColor c_error = new RGBAColor(221, 11, 11, 1f);
        public static RGBAColor c_deadPlayer = new RGBAColor(255, 0, 0, 1);
        public static RGBAColor c_causeOfDeath = new RGBAColor(255, 235, 4, 255);

        /* * * * * *
         * Vectors *
         * * * * * */
        public static Vector3 v_savedLocation = Vector3.zero;

        /* * * * * * * * *
         * Hack Settings *
         * * * * * * * * */
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
        public static float f_carAcceleration = 5f;
        public static float f_carMaxSpeed = 7f;
        public static float f_brakeSpeed = 7f;
        public static float f_pjSpamSpeed = 0.5f;


        public static bool b_disableSpectatorModels = true;
        public static bool b_useScrapTiers = false;
        public static bool b_VCDisplay = false;
        public static bool b_PlayerHPDisplay = false;
        public static bool b_ShowShipItems = false;
        public static bool b_DropItems = false;
        public static bool b_HackHighlight = false;
        public static bool b_FPSCounter = false; 
        public static bool b_DisplayLMUsers = false;
        public static bool b_DisplayHostKickedLobbies = false;
        public static bool b_WearBuy = false;

        public static CrosshairType ct_crosshairType = CrosshairType.Plus;

        public static bool b_chamsObject = false;
        public static bool b_chamsEnemy = false;
        public static bool b_chamsPlayer = false;
        public static bool b_chamsLandmine = false;
        public static bool b_chamsTurret = false;
        public static bool b_chamsBigDoor = false;
        public static bool b_chamsDoorLock = false;
        public static bool b_chamsSteamHazard = false;
        public static bool b_chamsBreaker = false;
        public static bool b_chamsShip = false;
        public static bool b_chamsSpikeRoofTrap = false;
        public static bool b_UseSingleChamColor = true;
        public static bool b_chamsDisableWithLOS = false;
        public static bool b_chamsMineshaftElevator = false;
        public static bool b_chamsEnemyVent = false;
        public static bool b_chamsItemDropship = false;
        public static bool b_chamsCruiser = false;
        public static bool b_chamsVainShroud = false;

        public static float f_defaultGrabDistance = -1f;
        public static float f_defaultClimbSpeed = 3f;
        public static float f_defaultJumpForce = 13f;
        public static float f_defaultMovementSpeed = 4.6f;
        public static float f_defaultNightVisionIntensity = 360f;
        public static float f_defaultNightVisionRange = 12f;
        public static float f_defaultFOV = 66f;
        public static float f_slots = 4f;

        public static int[] i_scrapValueThresholds = new int[] { 30,50,75,100 };

        public static SteamId s_lobbyid;

        public static RGBAColor[] c_scrapValueColors = new RGBAColor[] 
        { 
            new RGBAColor(0.5f, 0.5f, 0.5f, 1f),
            new RGBAColor(10, 187, 10, 1f),
            new RGBAColor(255, 0, 255, 1f),
            new RGBAColor(255, 165, 0, 1f),
        };

        public static CursorLockMode clm_lastCursorState = Cursor.lockState;

        public static string debugMessage = "";

        internal class Changelog
        {
            public static List<string> changes;
            public static void ReadChanges()
            {
                changes = new List<string>();
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("LethalMenu.Resources.Changelog.txt"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    while(!reader.EndOfStream)
                    {
                        changes.Add(reader.ReadLine());
                    }
                }
            }
        }

        internal class Config
        {
            public static string config = "lethelmenu.config.json";
            public static string defaultConf = "lethalmenu.default.config.json";

            public static void CreateConfigIfNotExists()
            {
                if (HasConfig()) return;
                SaveConfig();
            }

            public static void SaveDefaultConfig()
            {
                SaveConfig(defaultConf);
            }

            public static bool HasConfig()
            {
                return config != null && File.Exists(config);
            }

            public static void SaveConfig()
            {
                SaveConfig(config);
            }

            public static void SaveConfig(string conf)
            {
                Dictionary<string, string> keybinds = new Dictionary<string, string>();
                Dictionary<string, string> toggles = new Dictionary<string, string>();
                Dictionary<string, string> enemyFilter = new Dictionary<string, string>();

                foreach (var item in HackExtensions.KeyBinds)
                {
                    string key = item.Key.ToString();
                    string value = item.Value.GetType() == typeof(KeyControl) ? ((KeyControl) item.Value).keyCode.ToString() : item.Value.displayName;
                    keybinds.Add(key, value);
                }   

                foreach (var item in HackExtensions.ToggleFlags)
                {
                    if (item.Key.CanBeExecuted()) continue;
                    string key = item.Key.ToString();
                    string value = item.Value.ToString();
                    toggles.Add(key, value);
                }

                foreach(var item in EnemyAITypeExtensions.EnemyFilter)
                {
                    if (item.Key == EnemyAIType.Unknown) continue;
                    string key = item.Key.ToString();
                    string value = item.Value.ToString();
                    enemyFilter.Add(key, value);
                }

                JObject json = new JObject();
                JObject settings = new JObject();
                JObject colors = new JObject();
                JObject hackSettings = new JObject();
                JObject chams = new JObject();

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
                hackSettings["VCDisplay"] = b_VCDisplay.ToString();
                hackSettings["UseScrapTiers"] = b_useScrapTiers.ToString();
                hackSettings["PlayerHPDisplay"] = b_PlayerHPDisplay.ToString();
                hackSettings["ShowShipItems"] = b_ShowShipItems.ToString();
                hackSettings["FPSCounter"] = b_FPSCounter.ToString();
                hackSettings["DisplayLMUsers"] = b_DisplayLMUsers.ToString();
                hackSettings["HackHighlight"] = b_HackHighlight.ToString();
                hackSettings["FOV"] = f_fov.ToString();
                hackSettings["EnemyFilter"] = JObject.FromObject(enemyFilter);
                hackSettings["PJSpamSpeed"] = f_pjSpamSpeed.ToString();

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
                chams["SpikeRoofTrap"] = b_chamsSpikeRoofTrap.ToString();
                chams["UseSingleChamColor"] = b_UseSingleChamColor.ToString();
                chams["DisableWithLOS"] = b_chamsDisableWithLOS.ToString();
                chams["MineshaftElevator"] = b_chamsMineshaftElevator.ToString();
                chams["EnemyVent"] = b_chamsEnemyVent.ToString();
                chams["VainShroud"] = b_chamsVainShroud.ToString();
                chams["Cruiser"] = b_chamsCruiser.ToString();
                chams["ItemDropShip"] = b_chamsItemDropship.ToString();
                hackSettings["Chams"] = chams;   

                colors["Background"] = JsonConvert.SerializeObject(c_background);
                colors["Primary"] = JsonConvert.SerializeObject(c_primary);
                colors["MenuText"] = JsonConvert.SerializeObject(c_menuText);
                colors["Crosshair"] = JsonConvert.SerializeObject(c_crosshair);
                colors["HackHighlightColor"] = JsonConvert.SerializeObject(c_hackhighlight);
                colors["Chams"] = JsonConvert.SerializeObject(c_chams);
                colors["ObjectChams"] = JsonConvert.SerializeObject(c_objectChams);
                colors["PlayerChams"] = JsonConvert.SerializeObject(c_playerChams);
                colors["EnemyChams"] = JsonConvert.SerializeObject(c_enemyChams);
                colors["ShipChams"] = JsonConvert.SerializeObject(c_shipChams);
                colors["BreakerChams"] = JsonConvert.SerializeObject(c_breakerChams);
                colors["LandmineChams"] = JsonConvert.SerializeObject(c_landmineChams);
                colors["TurretChams"] = JsonConvert.SerializeObject(c_turretChams);
                colors["BigDoorChams"] = JsonConvert.SerializeObject(c_bigDoorChams);
                colors["DoorLockChams"] = JsonConvert.SerializeObject(c_doorLockChams);
                colors["SteamHazardChams"] = JsonConvert.SerializeObject(c_steamHazardChams);
                colors["SpikeRoofTrapChams"] = JsonConvert.SerializeObject(c_spikeRoofTrapChams);
                colors["MineshaftElevatorChams"] = JsonConvert.SerializeObject(c_mineshaftElevatorChams);
                colors["EnemyVentChams"] = JsonConvert.SerializeObject(c_enemyVentChams);
                colors["VainShroudChams"] = JsonConvert.SerializeObject(c_vainShroudChams);
                colors["CruiserChams"] = JsonConvert.SerializeObject(c_CruiserChams);
                colors["ItemDropShipChams"] = JsonConvert.SerializeObject(c_itemDropShipChams);
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
                colors["SpikeRoofTrapESP"] = JsonConvert.SerializeObject(c_spikeRoofTrapESP);
                colors["MineshaftElevatorESP"] = JsonConvert.SerializeObject(c_mineshaftElevatorESP);
                colors["EnemyVentESP"] = JsonConvert.SerializeObject(c_enemyVentESP);
                colors["VainShroudESP"] = JsonConvert.SerializeObject(c_vainShroudESP);
                colors["CruiserESP"] = JsonConvert.SerializeObject(c_CruiserESP);
                colors["ItemDropShipESP"] = JsonConvert.SerializeObject(c_itemDropShipESP);
                colors["CauseOfDeath"] = JsonConvert.SerializeObject(c_causeOfDeath);


                settings["FirstLaunch"] = isFirstLaunch.ToString();
                settings["MenuFontSize"] = i_menuFontSize.ToString();
                settings["ScreenFontSize"] = i_screenFontSize.ToString();
                settings["MenuWidth"] = i_menuWidth.ToString();
                settings["MenuHeight"] = i_menuHeight.ToString();
                settings["SliderWidth"] = i_sliderWidth.ToString();
                settings["TextboxWidth"] = i_textboxWidth.ToString();
                settings["MenuAlpha"] = f_menuAlpha.ToString();
                settings["DebugMode"] = DebugMode.ToString();


                json["Theme"] = Theme.name;
                json["Language"] = Localization.Language.Name;
                json["Colors"] = colors;
                json["HackSettings"] = hackSettings;
                json["MenuSettings"] = settings;
                json["KeyBinds"] = JObject.FromObject(keybinds);
                json["Toggles"] = JObject.FromObject(toggles);


                File.WriteAllText(conf, json.ToString());
            }

            public static void LoadConfig()
            {
                CreateConfigIfNotExists();

                string jsonStr = File.ReadAllText(config);

                JObject json = JObject.Parse(jsonStr);

                if (json.TryGetValue("Language", out JToken languageToken))
                    Localization.SetLanguage(languageToken.ToString());
                if (json.TryGetValue("Theme", out JToken themeToken))
                    Theme.SetTheme(themeToken.ToString());

                if (json.TryGetValue("MenuSettings", out JToken settingsToken))
                {
                    JObject settings = settingsToken.ToObject<JObject>();

                    if (settings.TryGetValue("FirstLaunch", out JToken firstLaunchToken))
                        isFirstLaunch = bool.Parse(firstLaunchToken.ToString());
                    if (settings.TryGetValue("MenuFontSize", out JToken menuFontSizeToken))
                        i_menuFontSize = int.Parse(menuFontSizeToken.ToString());
                    if (settings.TryGetValue("MenuWidth", out JToken menuWidthToken))
                        i_menuWidth = int.Parse(menuWidthToken.ToString());
                    if (settings.TryGetValue("MenuHeight", out JToken menuHeightToken))
                        i_menuHeight = int.Parse(menuHeightToken.ToString());
                    if (settings.TryGetValue("SliderWidth", out JToken sliderWidthToken))
                        i_sliderWidth = int.Parse(sliderWidthToken.ToString());
                    if (settings.TryGetValue("TextboxWidth", out JToken textboxWidthToken))
                        i_textboxWidth = int.Parse(textboxWidthToken.ToString());
                    if (settings.TryGetValue("MenuAlpha", out JToken menuAlphaToken))
                        f_menuAlpha = float.Parse(menuAlphaToken.ToString());
                    if (settings.TryGetValue("ScreenFontSize", out JToken screenFontSizeToken))
                        i_screenFontSize = int.Parse(screenFontSizeToken.ToString());
                    if (settings.TryGetValue("DebugMode", out JToken debugModeToken))
                    {
                        DebugMode = bool.Parse(debugModeToken.ToString());
                        HackMenu.Instance.ToggleDebugTab(DebugMode);
                    }
                }

                if (json.TryGetValue("HackSettings", out JToken hackSettingsToken))
                {
                    JObject hackSettings = hackSettingsToken.ToObject<JObject>();

                    if(hackSettings.TryGetValue("NightVisionIntensity", out JToken nightVisionIntensityToken))
                        f_nvIntensity = float.Parse(nightVisionIntensityToken.ToString());
                    if (hackSettings.TryGetValue("NightVisionRange", out JToken nightVisionRangeToken))
                        f_nvRange = float.Parse(nightVisionRangeToken.ToString());
                    if (hackSettings.TryGetValue("ClimbSpeed", out JToken climbSpeedToken))
                        f_climbSpeed = float.Parse(climbSpeedToken.ToString());
                    if (hackSettings.TryGetValue("JumpForce", out JToken jumpForceToken))
                        f_jumpForce = float.Parse(jumpForceToken.ToString());
                    if (hackSettings.TryGetValue("GrabDistance", out JToken grabDistanceToken))
                        f_grabDistance = float.Parse(grabDistanceToken.ToString());
                    if (hackSettings.TryGetValue("MovementSpeed", out JToken movementSpeedToken))
                        f_movementSpeed = float.Parse(movementSpeedToken.ToString());
                    if (hackSettings.TryGetValue("NoClipSpeed", out JToken noClipSpeedToken))
                        f_noclipSpeed = float.Parse(noClipSpeedToken.ToString());
                    if (hackSettings.TryGetValue("BreadcrumbInterval", out JToken breadcrumbIntervalToken))
                        f_breadcrumbInterval = int.Parse(breadcrumbIntervalToken.ToString());
                    if (hackSettings.TryGetValue("CrosshairThickness", out JToken crosshairThicknessToken))
                        f_crosshairThickness = float.Parse(crosshairThicknessToken.ToString());
                    if (hackSettings.TryGetValue("CrosshairType", out JToken crosshairTypeToken))
                        ct_crosshairType = (CrosshairType) Enum.Parse(typeof(CrosshairType), crosshairTypeToken.ToString());
                    if (hackSettings.TryGetValue("CrosshairScale", out JToken crosshairScaleToken))
                        f_crosshairScale = float.Parse(crosshairScaleToken.ToString());
                    if (hackSettings.TryGetValue("ESPDistance", out JToken espDistanceToken))
                        f_espDistance = float.Parse(espDistanceToken.ToString());
                    if (hackSettings.TryGetValue("DisableSpectatorModels", out JToken disableSpectatorModelsToken))
                        b_disableSpectatorModels = bool.Parse(disableSpectatorModelsToken.ToString());
                    if (hackSettings.TryGetValue("VCDisplay", out JToken VCDisplayToken))
                        b_VCDisplay = bool.Parse(VCDisplayToken.ToString());
                    if (hackSettings.TryGetValue("UseScrapTiers", out JToken UseScrapTiersToken))
                        b_useScrapTiers = bool.Parse(UseScrapTiersToken.ToString());
                    if (hackSettings.TryGetValue("PlayerHPDisplay", out JToken PlayerHPDisplayToken))
                        b_PlayerHPDisplay = bool.Parse(PlayerHPDisplayToken.ToString());
                    if (hackSettings.TryGetValue("ShowShipItems", out JToken ShowShipItemsToken))
                        b_ShowShipItems = bool.Parse(ShowShipItemsToken.ToString());
                    if (hackSettings.TryGetValue("FPSCounter", out JToken FPSCounterToken))
                        b_FPSCounter = bool.Parse(FPSCounterToken.ToString());
                    if (hackSettings.TryGetValue("HackHighlight", out JToken HackHighlightToken))
                        b_HackHighlight = bool.Parse(HackHighlightToken.ToString());
                    if (hackSettings.TryGetValue("DisplayLMUsers", out JToken DisplayLMUsersToken))
                        b_DisplayLMUsers = bool.Parse(DisplayLMUsersToken.ToString());
                    if (hackSettings.TryGetValue("UseSingleChamColor", out JToken singleChamColorToken))
                        b_UseSingleChamColor = bool.Parse(singleChamColorToken.ToString());
                    if (hackSettings.TryGetValue("FOV", out JToken fovToken))
                        f_fov = float.Parse(fovToken.ToString());
                    if (hackSettings.TryGetValue("PJSpamSpeed", out JToken pjSpamSpeedToken))
                        f_pjSpamSpeed = float.Parse(pjSpamSpeedToken.ToString());

                    if (hackSettings.TryGetValue("EnemyFilter", out JToken enemyFilterToken))
                    {
                        foreach (var item in enemyFilterToken.ToObject<Dictionary<string, string>>())
                        {
                            string s_type = item.Key;
                            string s_value = item.Value;
                            if (Enum.TryParse<EnemyAIType>(s_type, out EnemyAIType type))
                            {
                                if (type == EnemyAIType.Unknown) continue;
                                bool value = bool.TryParse(s_value, out bool result) ? result : false;
                                EnemyAITypeExtensions.EnemyFilter[type] = value;
                            }
                        }
                    }

                    if(hackSettings.TryGetValue("Chams", out JToken chamsToken))
                    {
                        JObject chams = chamsToken.ToObject<JObject>();
                        if (chams.TryGetValue("Distance", out JToken distanceToken))
                            f_chamDistance = float.Parse(distanceToken.ToString());
                        if (chams.TryGetValue("Object", out JToken objectToken))
                            b_chamsObject = bool.Parse(objectToken.ToString());
                        if (chams.TryGetValue("Enemy", out JToken enemyToken))
                            b_chamsEnemy = bool.Parse(enemyToken.ToString());
                        if (chams.TryGetValue("Player", out JToken playerToken))
                            b_chamsPlayer = bool.Parse(playerToken.ToString());
                        if (chams.TryGetValue("Landmine", out JToken landmineToken))
                            b_chamsLandmine = bool.Parse(landmineToken.ToString());
                        if (chams.TryGetValue("Turret", out JToken turretToken))
                            b_chamsTurret = bool.Parse(turretToken.ToString());
                        if (chams.TryGetValue("BigDoor", out JToken bigDoorToken))
                            b_chamsBigDoor = bool.Parse(bigDoorToken.ToString());
                        if (chams.TryGetValue("DoorLock", out JToken doorLockToken))
                            b_chamsDoorLock = bool.Parse(doorLockToken.ToString());
                        if (chams.TryGetValue("SteamHazard", out JToken steamHazardToken))
                            b_chamsSteamHazard = bool.Parse(steamHazardToken.ToString());
                        if (chams.TryGetValue("Breaker", out JToken breakerToken))
                            b_chamsBreaker = bool.Parse(breakerToken.ToString());
                        if (chams.TryGetValue("Ship", out JToken shipToken))
                            b_chamsShip = bool.Parse(shipToken.ToString());
                        if (chams.TryGetValue("SpikeRoofTrap", out JToken spikeRoofTrapToken))
                            b_chamsSpikeRoofTrap = bool.Parse(spikeRoofTrapToken.ToString());
                        if (chams.TryGetValue("DisableWithLOS", out JToken disableWithLOS))
                            b_chamsDisableWithLOS = bool.Parse(disableWithLOS.ToString());
                        if (chams.TryGetValue("MineshaftElevator", out JToken elevator))
                            b_chamsMineshaftElevator = bool.Parse(elevator.ToString());
                        if (chams.TryGetValue("EnemyVent", out JToken enemyvent))
                            b_chamsEnemyVent = bool.Parse(enemyvent.ToString());
                        if (chams.TryGetValue("VainShroud", out JToken vain))
                            b_chamsVainShroud = bool.Parse(vain.ToString());
                        if (chams.TryGetValue("Cruiser", out JToken vehicle))
                            b_chamsCruiser = bool.Parse(vehicle.ToString());
                        if (chams.TryGetValue("ItemDropShip", out JToken dropship))
                            b_chamsItemDropship = bool.Parse(dropship.ToString());
                    }
                }

                if (json.TryGetValue("Colors", out JToken colorsToken))
                {
                    JObject colors = colorsToken.ToObject<JObject>();

                    if(colors.TryGetValue("Background", out JToken backgroundToken))
                        c_background = JsonConvert.DeserializeObject<RGBAColor>(backgroundToken.ToString());
                    if (colors.TryGetValue("Primary", out JToken primaryToken))
                        c_primary = JsonConvert.DeserializeObject<RGBAColor>(primaryToken.ToString());
                    if (colors.TryGetValue("MenuText", out JToken menuTextToken))
                        c_menuText = JsonConvert.DeserializeObject<RGBAColor>(menuTextToken.ToString());
                    if (colors.TryGetValue("Crosshair", out JToken crosshairToken))
                        c_crosshair = JsonConvert.DeserializeObject<RGBAColor>(crosshairToken.ToString());
                    if (colors.TryGetValue("HackHighlightColor", out JToken hackhighlightToken))
                        c_hackhighlight = JsonConvert.DeserializeObject<RGBAColor>(hackhighlightToken.ToString());
                    if (colors.TryGetValue("Chams", out JToken chams))
                        c_chams = JsonConvert.DeserializeObject<RGBAColor>(chams.ToString());
                    if (colors.TryGetValue("ObjectESP", out JToken objectESP))
                        c_objectESP = JsonConvert.DeserializeObject<RGBAColor>(objectESP.ToString());
                    if (colors.TryGetValue("PlayerESP", out JToken playerESP))
                        c_playerESP = JsonConvert.DeserializeObject<RGBAColor>(playerESP.ToString());
                    if (colors.TryGetValue("EnemyESP", out JToken enemyESP))
                        c_enemyESP = JsonConvert.DeserializeObject<RGBAColor>(enemyESP.ToString());
                    if (colors.TryGetValue("ShipESP", out JToken shipESP))
                        c_shipESP = JsonConvert.DeserializeObject<RGBAColor>(shipESP.ToString());
                    if (colors.TryGetValue("BreakerESP", out JToken breakerESP))
                        c_breakerESP = JsonConvert.DeserializeObject<RGBAColor>(breakerESP.ToString());
                    if (colors.TryGetValue("LandmineESP", out JToken landmineESP))
                        c_landmineESP = JsonConvert.DeserializeObject<RGBAColor>(landmineESP.ToString());
                    if (colors.TryGetValue("TurretESP", out JToken turretESP))
                        c_turretESP = JsonConvert.DeserializeObject<RGBAColor>(turretESP.ToString());
                    if (colors.TryGetValue("BigDoorESP", out JToken bigDoorESP))
                        c_bigDoorESP = JsonConvert.DeserializeObject<RGBAColor>(bigDoorESP.ToString());
                    if (colors.TryGetValue("DoorLockESP", out JToken doorLockESP))
                        c_doorLockESP = JsonConvert.DeserializeObject<RGBAColor>(doorLockESP.ToString());
                    if (colors.TryGetValue("EntranceExitESP", out JToken entranceExitESP))
                        c_entranceExitESP = JsonConvert.DeserializeObject<RGBAColor>(entranceExitESP.ToString());
                    if(colors.TryGetValue("SteamHazardESP", out JToken steamHazardESP))
                        c_steamHazardESP = JsonConvert.DeserializeObject<RGBAColor>(steamHazardESP.ToString());
                    if (colors.TryGetValue("SpikeRoofTrapESP", out JToken roofSpikeTrapESP))
                        c_spikeRoofTrapESP = JsonConvert.DeserializeObject<RGBAColor>(roofSpikeTrapESP.ToString());
                    if (colors.TryGetValue("EnemyVentESP", out JToken enemyVentESP))
                        c_enemyVentESP = JsonConvert.DeserializeObject<RGBAColor>(enemyVentESP.ToString());
                    if (colors.TryGetValue("VainShroudESP", out JToken vainShroudESP))
                        c_vainShroudESP = JsonConvert.DeserializeObject<RGBAColor>(vainShroudESP.ToString());
                    if (colors.TryGetValue("CruiserESP", out JToken cruiserESP))
                        c_CruiserESP = JsonConvert.DeserializeObject<RGBAColor>(cruiserESP.ToString());
                    if (colors.TryGetValue("ItemDropShipESP", out JToken itemDropShipESP))
                        c_itemDropShipESP = JsonConvert.DeserializeObject<RGBAColor>(itemDropShipESP.ToString());
                    if (colors.TryGetValue("CauseOfDeath", out JToken causeOfDeath))
                        c_causeOfDeath = JsonConvert.DeserializeObject<RGBAColor>(causeOfDeath.ToString());
                    if (colors.TryGetValue("MineshaftElevatorESP", out JToken elevatorEsp))
                        c_mineshaftElevatorESP = JsonConvert.DeserializeObject<RGBAColor>(elevatorEsp.ToString());
                    if (colors.TryGetValue("MineshaftElevatorChams", out JToken elevatorChams))
                        c_mineshaftElevatorChams = JsonConvert.DeserializeObject<RGBAColor>(elevatorChams.ToString());
                    if (colors.TryGetValue("VainShroudChams", out JToken vainShroudChams))
                        c_vainShroudChams = JsonConvert.DeserializeObject<RGBAColor>(vainShroudChams.ToString());
                    if (colors.TryGetValue("CruiserChams", out JToken cruiserChams))
                        c_CruiserChams = JsonConvert.DeserializeObject<RGBAColor>(cruiserChams.ToString());
                    if (colors.TryGetValue("ItemDropShipChams", out JToken itemDropShipChams))
                        c_itemDropShipChams = JsonConvert.DeserializeObject<RGBAColor>(itemDropShipChams.ToString());
                    if (colors.TryGetValue("EnemyVentChams", out JToken enemyVentChams))
                        c_enemyVentChams = JsonConvert.DeserializeObject<RGBAColor>(enemyVentChams.ToString());
                }

                if (json.TryGetValue("KeyBinds", out JToken keybindsToken))
                {
                    HackExtensions.KeyBinds.Clear();
                    ButtonControl[] mouseButtons = new ButtonControl[] { Mouse.current.leftButton, Mouse.current.rightButton, Mouse.current.middleButton, Mouse.current.forwardButton, Mouse.current.backButton };
                    foreach (var item in keybindsToken.ToObject<Dictionary<string, string>>())
                    {
                        string s_hack = item.Key;
                        string s_key = item.Value;
                        ButtonControl mouseBtn = mouseButtons.FirstOrDefault(k => k.displayName == s_key);
                        ButtonControl key = Keyboard.current.allKeys.FirstOrDefault(k => k.keyCode.ToString() == s_key);
                        if (Enum.TryParse<Hack>(s_hack, out Hack hack))
                        {
                            if (mouseBtn == null && key == null) continue;
                            hack.SetKeyBind(mouseBtn ?? key);
                        }
                    }
                }
                

                if(json.TryGetValue("Toggles", out JToken togglesToken))
                {
                    foreach (var item in togglesToken.ToObject<Dictionary<string, string>>())
                    {
                        string s_hack = item.Key;
                        string s_key = item.Value;
                        if(Enum.TryParse<Hack>(s_hack, out Hack hack))
                        {
                            if (hack.CanBeExecuted()) continue;
                            bool toggle = bool.TryParse(s_key, out bool result) ? result : false;
                            hack.SetToggle(toggle);
                        }
                    }
                }
            }

            public static void RegenerateConfig()
            {
                if (HasConfig()) File.Delete(config);
                File.Copy(defaultConf, config);
                HackExtensions.KeyBinds.Clear();
                LoadConfig();
            }
        }
    }
}
