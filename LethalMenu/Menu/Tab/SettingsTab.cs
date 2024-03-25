using LethalMenu.Cheats;
using LethalMenu.Language;
using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace LethalMenu.Menu.Tab
{
    internal class SettingsTab : MenuTab
    {
        private string s_kbError = "";
        private string s_tierColorError = "";
        private string s_kbSearch = "";

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
        private string s_breakerESPColor = Settings.c_breakerESP.GetHexCode();
        private string s_causeOfDeath = Settings.c_causeOfDeath.GetHexCode();

        private string s_lootTierColors = string.Join(",", Array.ConvertAll(Settings.c_scrapValueColors, x => x.GetHexCode()));
        private string s_lootTiers = string.Join(",", Settings.i_scrapValueThresholds);

        private int i_selectedCrosshairIndex = -1;
        private int i_languageIndex = -1;

        public SettingsTab() : base("SettingsTab.Title") { }

        private float f_leftWidth;

        public override void Draw()
        {
            f_leftWidth = HackMenu.Instance.contentWidth * 0.55f - HackMenu.Instance.spaceFromLeft;

            if(i_selectedCrosshairIndex == -1) i_selectedCrosshairIndex = (int) Settings.ct_crosshairType;
            if(i_languageIndex == -1) i_languageIndex = Array.IndexOf(Localization.GetLanguages(), Localization.Language.Name);

            GUILayout.BeginVertical(GUILayout.Width(f_leftWidth));

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            MenuContent();
            ControlSettingsContent();
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
            
            UI.Actions(
                new UIButton("SettingsTab.ResetSettings", () => Settings.Config.RegenerateConfig()),
                new UIButton("SettingsTab.SaveSettings", () => Settings.Config.SaveConfig()),
                new UIButton("SettingsTab.ReloadSettings", () => Settings.Config.LoadConfig())
            );
            //UI.Checkbox("Debug Mode", ref Settings.isDebugMode);

            UI.Header("SettingsTab.General");

            UI.Select("SettingsTab.Language", ref i_languageIndex, Localization.GetLanguages().Select(x => new UIOption(x, () => Localization.SetLanguage(x))).ToArray());

            UI.NumSelect("SettingsTab.FontSize", ref Settings.i_menuFontSize, 5, 30);
            UI.NumSelect("SettingsTab.SliderSize", ref Settings.i_sliderWidth, 50, 120);
            UI.NumSelect("SettingsTab.TextboxSize", ref Settings.i_textboxWidth, 50, 120);
            UI.Slider("SettingsTab.MenuAlpha", Settings.f_menuAlpha.ToString("0.00"), ref Settings.f_menuAlpha, 0.1f, 1f);  
            UI.Button("SettingsTab.ResizeMenu", () => MenuUtil.BeginResizeMenu(), "SettingsTab.Resize");
            UI.Button("SettingsTab.ResetMenu", () => HackMenu.Instance.ResetMenuSize(), "General.Reset");
        }

        private void ControlSettingsContent()
        {
            UI.Header("SettingsTab.Control");

            UI.Slider("SettingsTab.mouseSens", Settings.f_mouseSensitivity.ToString("0.00"), ref Settings.f_mouseSensitivity, 0.1f, 1f);
            UI.Slider("SettingsTab.movementSpeed", Settings.f_inputMovementSpeed.ToString("0"), ref Settings.f_inputMovementSpeed, 10, 30);
        }

        private void VisualSettingsContent()
        {
            UI.Header("SettingsTab.Visual");
         
            UI.Slider("SettingsTab.MaxESP", Settings.f_espDistance.ToString("0") + "m", ref Settings.f_espDistance, 0, 10000);
            UI.Slider("SettingsTab.MinCham", Settings.f_chamDistance.ToString("0") + "m", ref Settings.f_chamDistance, 0, 100);
            UI.Slider("SettingsTab.CrosshairScale", Settings.f_crosshairScale.ToString("0.00"), ref Settings.f_crosshairScale, 4f, 24f);
            UI.Slider("SettingsTab.CrosshairThickness", Settings.f_crosshairThickness.ToString("0.00"), ref Settings.f_crosshairThickness, 1f, 5f);
            UI.Select("SettingsTab.CrosshairType", ref i_selectedCrosshairIndex,
                new UIOption("X", () => Settings.ct_crosshairType = CrosshairType.X),
                new UIOption("+", () => Settings.ct_crosshairType = CrosshairType.Plus)
            );
            UI.Slider("SettingsTab.BreadcrumbInterval", Settings.f_breadcrumbInterval.ToString(), ref Settings.f_breadcrumbInterval, 1f, 10f);
            UI.Slider("SettingsTab.NVIntensity", Settings.f_nvIntensity.ToString(), ref Settings.f_nvIntensity, Settings.f_defaultNightVisionIntensity, 10000f);
            UI.Slider("SettingsTab.NVRange", Settings.f_nvRange.ToString(), ref Settings.f_nvRange, Settings.f_defaultNightVisionRange, 10000f);
            UI.Checkbox("SettingsTab.DisableModels", ref Settings.b_disableSpectatorModels);
        }

        private void ESPSettingsContent()
        {
            UI.Header("SettingsTab.ESP", true);
            UI.SubHeader("SettingsTab.Chams");

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width((f_leftWidth * 0.465f)));
            UI.Checkbox("SettingsTab.Objects", ref Settings.b_chamsObject);
            UI.Checkbox("SettingsTab.Enemies", ref Settings.b_chamsEnemy);
            UI.Checkbox("SettingsTab.Players", ref Settings.b_chamsPlayer);
            UI.Checkbox("SettingsTab.Landmines", ref Settings.b_chamsLandmine);
            UI.Checkbox("SettingsTab.Breaker", ref Settings.b_chamsBreaker);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width((f_leftWidth * 0.465f)));
            UI.Checkbox("SettingsTab.Turrets", ref Settings.b_chamsTurret);
            UI.Checkbox("SettingsTab.Ship", ref Settings.b_chamsShip);
            UI.Checkbox("SettingsTab.SteamValves", ref Settings.b_chamsSteamHazard);
            UI.Checkbox("SettingsTab.BigDoors", ref Settings.b_chamsBigDoor);
            UI.Checkbox("SettingsTab.LockedDoors", ref Settings.b_chamsDoorLock);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            UI.SubHeader("SettingsTab.EnemyTypes", true);

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
            UI.Header("SettingsTab.ColorsHeader");

            UI.TextboxAction("SettingsTab.MenuBG", ref s_bgColor, @"[^0-9A-Za-z]", 8, 
                new UIButton("General.Set", () => SetColor(ref Settings.c_background, s_bgColor))
            );
            UI.TextboxAction("SettingsTab.MenuText", ref s_menuText, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_menuText, s_menuText))
            );
            UI.TextboxAction("SettingsTab.Primary", ref s_primaryColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_primary, s_primaryColor))
            );
            UI.TextboxAction("SettingsTab.Crosshair", ref s_crosshairColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_crosshair, s_crosshairColor))
            );

            UI.Header("SettingsTab.ESPColors", true);
            UI.TextboxAction("SettingsTab.Chams", ref s_chamsColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_chams, s_chamsColor))
            );

            UI.TextboxAction("SettingsTab.Objects", ref s_objectESPColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_objectESP, s_objectESPColor))
            );
            UI.TextboxAction("SettingsTab.Enemies", ref s_enemyESPColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_enemyESP, s_enemyESPColor))
            );
            UI.TextboxAction("SettingsTab.Players", ref s_playerESPColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_playerESP, s_playerESPColor))
            );
            UI.TextboxAction("SettingsTab.EntExtDoors", ref s_doorESPColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_entranceExitESP, s_doorESPColor))
            );
            UI.TextboxAction("SettingsTab.Landmines", ref s_landmineESPColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_landmineESP, s_landmineESPColor))
            );
            UI.TextboxAction("SettingsTab.Turrets", ref s_turretESPColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_turretESP, s_turretESPColor))
            );
            UI.TextboxAction("SettingsTab.Ship", ref s_shipESPColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_shipESP, s_shipESPColor))
            );
            UI.TextboxAction("SettingsTab.SteamValves", ref s_valveESPColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_steamHazardESP, s_valveESPColor))
            );
            UI.TextboxAction("SettingsTab.BigDoors", ref s_bigDoorESPColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_bigDoorESP, s_bigDoorESPColor))
            );
            UI.TextboxAction("SettingsTab.LockedDoors", ref s_doorLockESPColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_doorLockESP, s_doorLockESPColor))
            );
            UI.TextboxAction("SettingsTab.Breaker", ref s_doorLockESPColor, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_breakerESP, s_breakerESPColor))
            );


            UI.Header("SettingsTab.TieredLootHeader", true);
            if (s_tierColorError != "") UI.Label(s_tierColorError, Settings.c_error);

            


            UI.Button(["SettingsTab.TieredLoot", $"({GetTiersColored()})"], () => EditTierColors(), "General.Set");
            UI.Textbox("SettingsTab.Tiers", ref s_lootTiers, @"[^0-9,]");
            UI.Textbox("SettingsTab.Colors", ref s_lootTierColors, @"[^0-9A-Za-z,]");

            UI.Header("SettingsTab.OtherColors", true);

            UI.TextboxAction("SettingsTab.CauseOfDeath", ref s_causeOfDeath, @"[^0-9A-Za-z]", 8,
                new UIButton("General.Set", () => SetColor(ref Settings.c_causeOfDeath, s_causeOfDeath))
            );   
        }

        private void KeybindContent()
        {

            UI.Header("SettingsTab.Keybinds");

            if (s_kbError != "") UI.Label(s_kbError, Settings.c_error);


            GUILayout.BeginVertical();
            kbScrollPos = GUILayout.BeginScrollView(kbScrollPos);
            UI.Textbox("General.Search", ref s_kbSearch, big: false);

            List<Hack> hacks = Enum.GetValues(typeof(Hack)).Cast<Hack>().ToList().FindAll(x => x.ToString().ToLower().Contains(s_kbSearch.ToLower()));

            foreach (Hack hack in hacks)
            {
                if (!hack.CanHaveKeyBind()) continue;

                GUILayout.BeginHorizontal();

                ButtonControl bind = hack.GetKeyBind();

                string kb = hack.HasKeyBind() ? bind.GetType() == typeof(KeyControl) ? ((KeyControl)bind).keyCode.ToString() : bind.displayName : "None";

                

                GUILayout.Label(hack.ToString());
                GUILayout.FlexibleSpace();

                if (hack.HasKeyBind() && hack != Hack.OpenMenu && hack != Hack.UnlockDoorAction && GUILayout.Button("-")) hack.RemoveKeyBind();

                string btnText = hack.IsWaiting() ? "Waiting" : kb;
                if (GUILayout.Button(btnText, GUILayout.Width(85))) KBUtil.BeginChangeKeyBind(hack);
                

                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();


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
                s_tierColorError = "SettingsTab.TierColorError";
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


