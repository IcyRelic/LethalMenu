using LethalMenu.Cheats;
using LethalMenu.Handler;
using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    internal enum GuiSize
    {
        XSmall = 0,
        Small = 1,
        Medium = 2,
        Large = 3
    }

    internal static class GuiSizeExtensions
    {
        public static int GetFontSize(this GuiSize guiSize)
        {
            switch (guiSize)
            {
                case GuiSize.XSmall:
                    return 10;
                case GuiSize.Small:
                    return 12;
                case GuiSize.Medium:
                    return 14;
                case GuiSize.Large:
                    return 16;
                default:
                    return 14;
            }
        }

        public static float GetMenuTabWidth(this GuiSize guiSize)
        {
            switch (guiSize)
            {
                case GuiSize.XSmall:
                    return 0.15f;
                case GuiSize.Small:
                    return 0.15f;
                case GuiSize.Medium:
                    return 0.15f;
                case GuiSize.Large:
                    return 0.15f;
                default:
                    return 0.15f;
            }
        }
        public static int GetMenuWidth(this GuiSize guiSize)
        {
            switch (guiSize)
            {
                case GuiSize.XSmall:
                    return 600;
                case GuiSize.Small:
                    return 700;
                case GuiSize.Medium:
                    return 810;
                case GuiSize.Large:
                    return 900;
                default:
                    return 800;
            }
        }

        public static int GetMenuHeight(this GuiSize guiSize)
        {
            switch (guiSize)
            {
                case GuiSize.XSmall:
                    return 305;
                case GuiSize.Small:
                    return 400;
                case GuiSize.Medium:
                    return 410;
                case GuiSize.Large:
                    return 450;
                default:
                    return 400;
            }
        }

        public static int GetMenuTabPadding(this GuiSize guiSize)
        {
            switch (guiSize)
            {
                case GuiSize.XSmall:
                    return 5;
                case GuiSize.Small:
                    return 7;
                case GuiSize.Medium:
                    return 10;
                case GuiSize.Large:
                    return 10;
                default:
                    return 10;
            }
        }

        public static int GetSliderWidth(this GuiSize guiSize)
        {
            switch (guiSize)
            {
                case GuiSize.XSmall:
                    return 75;
                case GuiSize.Small:
                    return 85;
                case GuiSize.Medium:
                    return 100;
                case GuiSize.Large:
                    return 120;
                default:
                    return 100;
            }
        }

        public static int GetTextboxWidth(this GuiSize guiSize)
        {
            switch (guiSize)
            {
                case GuiSize.XSmall:
                    return 50;
                case GuiSize.Small:
                    return 75;
                case GuiSize.Medium:
                    return 85;
                case GuiSize.Large:
                    return 100;
                default:
                    return 85;
            }
        }
    }

    internal class Settings
    {

        public static string version = "v1.3.1";
        public static bool isDebugMode = false;

        public static bool isMenuOpen
        {
            get { return Hack.OpenMenu.IsEnabled(); }
            set { Hack.OpenMenu.SetToggle(value);  }
        }

        /* *
         * Menu Settings
         * */
        private static GuiSize _guiSize = GuiSize.Medium;
        public static GuiSize GUISize
        {
            get { return _guiSize; }
            set
            {
                _guiSize = value;
                HackMenu.Instance.Resize();
            }
        }

        /* *
         * Color Settings
         * */
        public static RGBAColor c_background = new RGBAColor(51, 51, 51, 1f);
        public static RGBAColor c_primary = new RGBAColor(165, 55, 253, 1f);
        public static RGBAColor c_menuText = new RGBAColor(255, 255, 255, 1f);
        public static RGBAColor c_crosshair = new RGBAColor(255, 43, 43, 1f);

        //ESP Colors
        
        public static RGBAColor c_objectESP = new RGBAColor(255, 255, 255, 1f);
        public static RGBAColor c_playerESP = new RGBAColor(0, 255, 0, 1f);
        public static RGBAColor c_enemyESP = new RGBAColor(255, 0, 0, 1f);
        public static RGBAColor c_shipESP = new RGBAColor(0, 0, 255, 1f);
        public static RGBAColor c_landmineESP = new RGBAColor(255, 0, 0, 1f);
        public static RGBAColor c_turretESP = new RGBAColor(255, 0, 0, 1f);
        public static RGBAColor c_bigDoorESP = new RGBAColor(0, 255, 255, 1f);
        public static RGBAColor c_doorLockESP = new RGBAColor(0.5f, 0.5f, 0.5f, 1f);
        public static RGBAColor c_entranceExitESP = new RGBAColor(0, 0, 255, 1f);
        public static RGBAColor c_steamHazardESP = new RGBAColor(255, 0, 255, 1f);
        public static RGBAColor c_causeOfDeath = new RGBAColor(1f, 47f / 51f, 0.0156862754f, 1f);
        public static RGBAColor c_breakerESP = new RGBAColor(255,0,116, 1f);
        public static RGBAColor c_chams = new RGBAColor(238, 111, 255, 0.1f);



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
        public static bool b_disableSpectatorModels = true;
        public static bool b_useScrapTiers = false;

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

        public static float f_defaultGrabDistance = -1f;
        public static float f_defaultClimbSpeed = -1f;
        public static float f_defaultJumpForce = -1f;
        public static float f_defaultMovementSpeed = -1f;
        public static float f_defaultNightVisionIntensity = 360f;
        public static float f_defaultNightVisionRange = 12f;

        public static int[] i_scrapValueThresholds = new int[] { 30,50,75,100 };

        public static RGBAColor[] c_scrapValueColors = new RGBAColor[] 
        { 
            new RGBAColor(0.5f, 0.5f, 0.5f, 1f),
            new RGBAColor(10, 187, 10, 1f),
            new RGBAColor(255, 0, 255, 1f),
            new RGBAColor(255, 165, 0, 1f),
        };

        public static CursorLockMode clm_lastCursorState = Cursor.lockState;
        

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
            private static string config = "lethelmenu.config.json";
            private static string defaultConf = "lethalmenu.default.config.json";
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
                hackSettings["EnemyFilter"] = JObject.FromObject(enemyFilter);
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


                settings["GUISize"] = GUISize.ToString();

                


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

               
                //JObject settings = (JObject)json["MenuSettings"];
                //JObject colors = (JObject)json["Colors"];
                //JObject hackSettings = (JObject)json["HackSettings"];

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
                        f_espDistance = int.Parse(espDistanceToken.ToString());
                    if (hackSettings.TryGetValue("DisableSpectatorModels", out JToken disableSpectatorModelsToken))
                        b_disableSpectatorModels = bool.Parse(disableSpectatorModelsToken.ToString());


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
                    if (colors.TryGetValue("CauseOfDeath", out JToken causeOfDeath))
                        c_causeOfDeath = JsonConvert.DeserializeObject<RGBAColor>(causeOfDeath.ToString());
                }

                if (json.TryGetValue("MenuSettings", out JToken settingsToken))
                {
                    JObject settings = settingsToken.ToObject<JObject>();

                    if (settings.TryGetValue("GUISize", out JToken guiSizeToken))
                        GUISize = (GuiSize) Enum.Parse(typeof(GuiSize), guiSizeToken.ToString());
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

