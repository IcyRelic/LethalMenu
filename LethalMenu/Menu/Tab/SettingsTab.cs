using DunGen;
using LethalMenu.Cheats;
using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace LethalMenu.Menu.Tab
{
    internal class SettingsTab : MenuTab
    {
        private string kbError = "";
        private string tierColorError = "";

        private Vector2 scrollPos = Vector2.zero;
        private Vector2 kbScrollPos = Vector2.zero;

        private string s_bgColor = Settings.c_background.GetHexCode();
        private string s_primaryColor = Settings.c_primary.GetHexCode();
        private string s_crosshairColor = Settings.c_crosshair.GetHexCode();
        private string s_menuText = Settings.c_menuText.GetHexCode();

        //esp colors
        private string s_chamsColor = Settings.c_chams.GetHexCode();
        private string s_objectESPColor = Settings.c_objectESP.GetHexCode();
        private string s_playerESPColor = Settings.c_playerESP.GetHexCode();
        private string s_enemyESPColor = Settings.c_enemyESP.GetHexCode();
        private string s_landmineESPColor = Settings.c_landmineESP.GetHexCode();
        private string s_turretESPColor = Settings.c_turretESP.GetHexCode();
        private string s_doorESPColor = Settings.c_entranceExitESP.GetHexCode();
        private string s_doorLockESPColor = Settings.c_doorLockESP.GetHexCode();
        private string s_valveESPColor = Settings.c_steamHazardESP.GetHexCode();
        private string s_bigDoorESPColor = Settings.c_bigDoorESP.GetHexCode();
        private string s_shipESPColor = Settings.c_shipESP.GetHexCode();
        private string s_causeOfDeath = Settings.c_causeOfDeath.GetHexCode();

        private string s_lootTierColors = string.Join(",", Array.ConvertAll(Settings.c_scrapValueColors, x => x.GetHexCode()));
        private string s_lootTiers = string.Join(",", Settings.i_scrapValueThresholds);

        private int i_selectedSizeIndex = -1;
        private int i_selectedCrosshairIndex = -1;

        public SettingsTab() : base("Settings") { }

        private float f_leftWidth;

        public override void Draw()
        {
            f_leftWidth = HackMenu.Instance.contentWidth * 0.55f - HackMenu.Instance.spaceFromLeft;

            if(i_selectedSizeIndex == -1) i_selectedSizeIndex = (int) Settings.GUISize;
            if(i_selectedCrosshairIndex == -1) i_selectedCrosshairIndex = (int) Settings.ct_crosshairType;

            GUILayout.BeginVertical(GUILayout.Width(f_leftWidth));

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            MenuContent();
            VisualSettingsContent();
            ColorContent();
            ESPSettingsContent();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.45f - HackMenu.Instance.spaceFromLeft));

            KeybindContent();

            GUILayout.EndVertical();
        }
        
        

        private void MenuContent()
        {
            UI.Header("General");
            UI.Actions(
                new UIButton("Reset Settings", () => Settings.Config.RegenerateConfig()),
                new UIButton("Save Settings", () => Settings.Config.SaveConfig()),
                new UIButton("Reload Settings", () => Settings.Config.LoadConfig())
            );
            UI.Checkbox("Debug Mode", ref Settings.isDebugMode);

            UI.Select("Gui Size", ref i_selectedSizeIndex,
                new UIOption("XSmall", () => Settings.GUISize = GuiSize.XSmall),
                new UIOption("Small", () => Settings.GUISize = GuiSize.Small),
                new UIOption("Medium", () => Settings.GUISize = GuiSize.Medium),
                new UIOption("Large", () => Settings.GUISize = GuiSize.Large)
            );
        }

        private void VisualSettingsContent()
        {
            UI.Header("Visual Settings");
            UI.Slider("Max ESP Distance", Settings.f_espDistance.ToString("0") + "m", ref Settings.f_espDistance, 0, 10000);
            UI.Slider("Min Cham Distance", Settings.f_chamDistance.ToString("0") + "m", ref Settings.f_chamDistance, 0, 100);
            UI.Slider("Crosshair Scale", Settings.f_crosshairScale.ToString("0.00"), ref Settings.f_crosshairScale, 4f, 24f);
            UI.Slider("Crosshair Thickness", Settings.f_crosshairThickness.ToString("0.00"), ref Settings.f_crosshairThickness, 1f, 5f);
            UI.Select("Crosshair Type", ref i_selectedCrosshairIndex,
                new UIOption("X", () => Settings.ct_crosshairType = CrosshairType.X),
                new UIOption("+", () => Settings.ct_crosshairType = CrosshairType.Plus)
            );
            UI.Slider("Breadcrumb Interval", Settings.f_breadcrumbInterval.ToString(), ref Settings.f_breadcrumbInterval, 1f, 10f);
            UI.Slider("Night Vision Intensity", Settings.f_nvIntensity.ToString(), ref Settings.f_nvIntensity, Settings.f_defaultNightVisionIntensity, 10000f);
            UI.Slider("Night Vision Range", Settings.f_nvRange.ToString(), ref Settings.f_nvRange, Settings.f_defaultNightVisionRange, 10000f);
            UI.Checkbox("Disable Spectator Player Models", ref Settings.b_disableSpectatorModels);
        }

        private void ESPSettingsContent()
        {
            UI.Header("ESP Settings", true);
            UI.SubHeader("Chams");

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width((f_leftWidth * 0.465f)));
            UI.Checkbox("Objects", ref Settings.b_chamsObject);
            UI.Checkbox("Enemies", ref Settings.b_chamsEnemy);
            UI.Checkbox("Players", ref Settings.b_chamsPlayer);
            UI.Checkbox("Entrance/Exit", ref Settings.b_chamsEntranceExit);
            UI.Checkbox("Landmines", ref Settings.b_chamsLandmine);
            UI.Checkbox("Breaker Box", ref Settings.b_chamsBreaker);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width((f_leftWidth * 0.465f)));
            UI.Checkbox("Turrets", ref Settings.b_chamsTurret);
            UI.Checkbox("Ship Door", ref Settings.b_chamsShip);
            UI.Checkbox("Steam Valves", ref Settings.b_chamsSteamHazard);
            UI.Checkbox("Big Doors", ref Settings.b_chamsBigDoor);
            UI.Checkbox("Locked Doors", ref Settings.b_chamsDoorLock);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            UI.SubHeader("Enemy Types", true);

            List<EnemyAIType> types = Enum.GetValues(typeof(EnemyAIType)).Cast<EnemyAIType>().ToList();

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width((f_leftWidth * 0.465f)));
            for (int i = 0; i < types.Count / 2; i++)
            {
                EnemyAIType type = types[i];
                if(type == EnemyAIType.Unknown) continue;
                UI.Checkbox(type.ToString(), type.IsESPEnabled(), () => type.ToggleESP());
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width((f_leftWidth * 0.465f)));
            for (int i = types.Count / 2; i < types.Count; i++)
            {
                EnemyAIType type = types[i];
                if (type == EnemyAIType.Unknown) continue;
                UI.Checkbox(type.ToString(), type.IsESPEnabled(), () => type.ToggleESP());
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }



        private void ColorContent()
        {
            UI.Header("Colors (HTML Color Codes)");

            UI.TextboxAction("Menu BG", ref s_bgColor, @"[^0-9A-Za-z]", 6, 
                new UIButton("Set", () => SetColor(ref Settings.c_background, s_bgColor))
            );
            UI.TextboxAction("Menu Text", ref s_menuText, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_menuText, s_menuText))
            );
            UI.TextboxAction("Primary", ref s_primaryColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_primary, s_primaryColor))
            );
            UI.TextboxAction("Crosshair", ref s_crosshairColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_crosshair, s_crosshairColor))
            );

            UI.Header("ESP Colors", true);
            UI.TextboxAction("Chams", ref s_chamsColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_chams, s_chamsColor))
            );
            UI.TextboxAction("Objects", ref s_objectESPColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_objectESP, s_objectESPColor))
            );
            UI.TextboxAction("Enemies", ref s_enemyESPColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_enemyESP, s_enemyESPColor))
            );
            UI.TextboxAction("Players", ref s_playerESPColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_playerESP, s_playerESPColor))
            );
            UI.TextboxAction("Entrance/Exit Doors", ref s_doorESPColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_entranceExitESP, s_doorESPColor))
            );
            UI.TextboxAction("Landmines", ref s_landmineESPColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_landmineESP, s_landmineESPColor))
            );
            UI.TextboxAction("Turrets", ref s_turretESPColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_turretESP, s_turretESPColor))
            );
            UI.TextboxAction("Ship", ref s_shipESPColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_shipESP, s_shipESPColor))
            );
            UI.TextboxAction("Steam Valves", ref s_valveESPColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_steamHazardESP, s_valveESPColor))
            );
            UI.TextboxAction("Big Doors", ref s_bigDoorESPColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_bigDoorESP, s_bigDoorESPColor))
            );
            UI.TextboxAction("Locked Doors", ref s_doorLockESPColor, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_doorLockESP, s_doorLockESPColor))
            );


            UI.Header("Tiered Loot Colors", true);
            if (tierColorError != "") UI.Label("<color=#DD0B0B>" + tierColorError + "</color>");

            UI.Button("Tiered Loot ( " + GetTiersColored() + " )", () => EditTierColors(), "Set");
            UI.Textbox("Tiers", ref s_lootTiers, @"[^0-9,]");
            UI.Textbox("Colors", ref s_lootTierColors, @"[^0-9A-Za-z,]");

            UI.Header("Other Colors", true);

            UI.TextboxAction("Cause of Death", ref s_causeOfDeath, @"[^0-9A-Za-z]", 6,
                new UIButton("Set", () => SetColor(ref Settings.c_causeOfDeath, s_causeOfDeath))
            );   
        }

        private void KeybindContent()
        {

            UI.Header("Keybinds");

            if (kbError != "") UI.Label("<color=#DD0B0B>" + kbError + "</color>");


            GUILayout.BeginVertical();
            kbScrollPos = GUILayout.BeginScrollView(kbScrollPos);

            foreach (Hack hack in Enum.GetValues(typeof(Hack)))
            {
                if (!hack.CanHaveKeyBind()) continue;

                GUILayout.BeginHorizontal();

                ButtonControl bind = hack.GetKeyBind();

                string kb = hack.HasKeyBind() ? bind.GetType() == typeof(KeyControl) ? ((KeyControl)bind).keyCode.ToString() : bind.displayName : "None";

                

                GUILayout.Label(hack.ToString());
                GUILayout.FlexibleSpace();

                if (hack.HasKeyBind() && hack != Hack.OpenMenu && hack != Hack.UnlockDoorAction && GUILayout.Button("-")) hack.RemoveKeyBind();

                string btnText = hack.IsWaiting() ? "Waiting" : kb;
                if (GUILayout.Button(btnText, GUILayout.Width(70)))
                {
                    hack.SetWaiting(true);
                    _ = TryGetPressedKeyTask(async (btn) =>
                    {
                        kbError = "";
                        hack.SetKeyBind(btn);
                        await Task.Delay(100);
                        hack.SetWaiting(false);
                        Settings.Config.SaveConfig();
                    });
                }

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();


        }

        private async Task TryGetPressedKeyTask(Action<ButtonControl> callback)
        {
            await Task.Run(() =>
            {
                float startTime = Time.time;
                ButtonControl btn = null;
                do
                {
                    ButtonControl pressed = GetPressedBtn();
                    

                    if (pressed != null)
                    {
                        if (!HackExtensions.KeyBindInUse(pressed)) btn = pressed;
                        else kbError = "Keybind already in use!";
                    }

                    if (Time.time - startTime > 15f) break;
                } while (btn == null);

                if (btn == null) return;

                callback?.Invoke(btn);
            });


        }

        private ButtonControl GetPressedBtn()
        {
            foreach (KeyControl key in Keyboard.current.allKeys)
            {
                if (key.wasPressedThisFrame) return key;

            }

            ButtonControl[] mouseButtons = new ButtonControl[] { Mouse.current.leftButton, Mouse.current.rightButton, Mouse.current.middleButton, Mouse.current.forwardButton, Mouse.current.backButton };

            foreach (ButtonControl btn in mouseButtons)
            {
                if (btn.wasPressedThisFrame) return btn;
            }

            return null;
        }

        private void SetColor(ref RGBAColor color, string hexCode)
        {
            while (hexCode.Length < 6) hexCode += "0";
            color = new RGBAColor(hexCode);
            Settings.Config.SaveConfig();
        }

        private void EditTierColors()
        {
            int[] thresholds = Array.ConvertAll(s_lootTiers.Split(','), x => int.TryParse(x, out int i) ? i : 0);
            RGBAColor[] rgbaColors = Array.ConvertAll(s_lootTierColors.Split(','), x => new RGBAColor(x));

            if (thresholds.Length != rgbaColors.Length)
            {
                tierColorError = " Thresholds and Colors must be the same length!";
                return;
            }

            Settings.i_scrapValueThresholds = thresholds;
            Settings.c_scrapValueColors = rgbaColors;

            Settings.Config.SaveConfig();
        }

        private string GetTiersColored()
        {
            string[] tiers = new string[Settings.i_scrapValueThresholds.Length];

            for (int i = 0; i < Settings.i_scrapValueThresholds.Length; i++)
            {
                int threshold = Settings.i_scrapValueThresholds[i];
                RGBAColor color = Settings.c_scrapValueColors[i];

                tiers[i] = color.AsString(threshold.ToString());
            }

            return string.Join(",", tiers);
        }

    }
}


