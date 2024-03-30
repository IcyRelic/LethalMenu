using System;
using System.Globalization;
using System.Linq;
using LethalMenu.Cheats;
using LethalMenu.Language;
using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace LethalMenu.Menu.Tab;

internal class SettingsTab() : MenuTab("SettingsTab.Title")
{
    private readonly string[] _modes = ["Default", "Green", "Blue"];
    private readonly string _sBreakerEspColor = Settings.c_breakerESP.GetHexCode();

    private float _fLeftWidth;
    private int _iLanguageIndex = -1;

    private int _iSelectedCrosshairIndex = -1;
    private Vector2 _kbScrollPosition = Vector2.zero;

    private string _sBgColor = Settings.c_background.GetHexCode();
    private string _sBigDoorEspColor = Settings.c_bigDoorESP.GetHexCode();
    private string _sCauseOfDeath = Settings.c_causeOfDeath.GetHexCode();

    //esp colors
    private string _sChamsColor = Settings.c_chams.GetHexCode();

    private Vector2 _scrollPosition = Vector2.zero;
    private string _sCrosshairColor = Settings.c_crosshair.GetHexCode();
    private string _sDoorEspColor = Settings.c_entranceExitESP.GetHexCode();
    private string _sDoorLockEspColor = Settings.c_doorLockESP.GetHexCode();
    private int _selectedMode;
    private string _sEnemyEspColor = Settings.c_enemyESP.GetHexCode();
    private string _sKbSearch = "";
    private string _sLandmineEspColor = Settings.c_landmineESP.GetHexCode();

    private string _sLootTierColors =
        string.Join(",", Array.ConvertAll(Settings.c_scrapValueColors, x => x.GetHexCode()));

    private string _sLootTiers = string.Join(",", Settings.i_scrapValueThresholds);
    private string _sMenuText = Settings.c_menuText.GetHexCode();
    private string _sObjectEspColor = Settings.c_objectESP.GetHexCode();
    private string _sPlayerEspColor = Settings.c_playerESP.GetHexCode();
    private string _sPrimaryColor = Settings.c_primary.GetHexCode();
    private string _sShipEspColor = Settings.c_shipESP.GetHexCode();
    private string _sTierColorError = "";
    private string _sTurretEspColor = Settings.c_turretESP.GetHexCode();
    private string _sValveEspColor = Settings.c_steamHazardESP.GetHexCode();

    public override void Draw()
    {
        _fLeftWidth = HackMenu.Instance.ContentWidth * 0.55f - HackMenu.SpaceFromLeft;

        if (_iSelectedCrosshairIndex == -1) _iSelectedCrosshairIndex = (int)Settings.ct_crosshairType;
        if (_iLanguageIndex == -1)
            _iLanguageIndex = Array.IndexOf(Localization.GetLanguages(), Localization.Language.Name);

        GUILayout.BeginVertical(GUILayout.Width(_fLeftWidth));

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        MenuContent();
        ControlSettingsContent();
        VisualSettingsContent();
        ColorContent();
        EspSettingsContent();
        GUILayout.EndScrollView();
        GUILayout.EndVertical();
        GUILayout.BeginVertical(
            GUILayout.Width(HackMenu.Instance.ContentWidth * 0.45f - HackMenu.SpaceFromLeft));

        KeybindContent();

        GUILayout.EndVertical();
    }


    private void MenuContent()
    {
        UI.Actions(
            new UIButton("SettingsTab.ResetSettings", Settings.Config.RegenerateConfig),
            new UIButton("SettingsTab.SaveSettings", Settings.Config.SaveConfig),
            new UIButton("SettingsTab.ReloadSettings", Settings.Config.LoadConfig)
        );
        //UI.Checkbox("Debug Mode", ref Settings.isDebugMode);

        UI.Header("SettingsTab.General");

        UI.IndexSelectAction("Theme:", ref _selectedMode, _modes);
        UI.Select("SettingsTab.Language", ref _iLanguageIndex,
            Localization.GetLanguages().Select(x => new UIOption(x, () => Localization.SetLanguage(x))).ToArray());
        UI.NumSelect("SettingsTab.FontSize", ref Settings.i_menuFontSize, 5, 30);
        UI.NumSelect("SettingsTab.SliderSize", ref Settings.i_sliderWidth, 50, 120);
        UI.NumSelect("SettingsTab.TextboxSize", ref Settings.i_textboxWidth, 50, 120);
        UI.Slider("SettingsTab.MenuAlpha", Settings.f_menuAlpha.ToString("0.00"), ref Settings.f_menuAlpha, 0.1f, 1f);
        UI.Button("SettingsTab.ResizeMenu", MenuUtil.BeginResizeMenu, "SettingsTab.Resize");
        UI.Button("SettingsTab.ResetMenu", () => HackMenu.Instance.ResetMenuSize(), "General.Reset");
    }

    private static void ControlSettingsContent()
    {
        UI.Header("SettingsTab.Control");

        UI.Slider("SettingsTab.mouseSens", Settings.f_mouseSensitivity.ToString("0.00"),
            ref Settings.f_mouseSensitivity, 0.1f, 1f);
        UI.Slider("SettingsTab.movementSpeed", Settings.f_inputMovementSpeed.ToString("0"),
            ref Settings.f_inputMovementSpeed, 10, 30);
    }

    private void VisualSettingsContent()
    {
        UI.Header("SettingsTab.Visual");

        UI.Slider("SettingsTab.MaxESP", Settings.f_espDistance.ToString("0") + "m", ref Settings.f_espDistance, 0,
            10000);
        UI.Slider("SettingsTab.MinCham", Settings.f_chamDistance.ToString("0") + "m", ref Settings.f_chamDistance, 0,
            100);
        UI.Slider("SettingsTab.CrosshairScale", Settings.f_crosshairScale.ToString("0.00"),
            ref Settings.f_crosshairScale, 4f, 24f);
        UI.Slider("SettingsTab.CrosshairThickness", Settings.f_crosshairThickness.ToString("0.00"),
            ref Settings.f_crosshairThickness, 1f, 5f);
        UI.Select("SettingsTab.CrosshairType", ref _iSelectedCrosshairIndex,
            new UIOption("X", () => Settings.ct_crosshairType = CrosshairType.X),
            new UIOption("+", () => Settings.ct_crosshairType = CrosshairType.Plus)
        );
        UI.Slider("SettingsTab.BreadcrumbInterval", Settings.f_breadcrumbInterval.ToString(CultureInfo.CurrentCulture),
            ref Settings.f_breadcrumbInterval, 1f, 10f);
        UI.Slider("SettingsTab.NVIntensity", Settings.f_nvIntensity.ToString(CultureInfo.CurrentCulture),
            ref Settings.f_nvIntensity,
            Settings.f_defaultNightVisionIntensity, 10000f);
        UI.Slider("SettingsTab.NVRange", Settings.f_nvRange.ToString(CultureInfo.CurrentCulture),
            ref Settings.f_nvRange,
            Settings.f_defaultNightVisionRange, 10000f);
        UI.Checkbox("SettingsTab.DisableModels", ref Settings.b_disableSpectatorModels);
    }

    private void EspSettingsContent()
    {
        UI.Header("SettingsTab.ESP", true);
        UI.SubHeader("SettingsTab.Chams");

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(_fLeftWidth * 0.465f));
        UI.Checkbox("SettingsTab.Objects", ref Settings.b_chamsObject);
        UI.Checkbox("SettingsTab.Enemies", ref Settings.b_chamsEnemy);
        UI.Checkbox("SettingsTab.Players", ref Settings.b_chamsPlayer);
        UI.Checkbox("SettingsTab.Landmines", ref Settings.b_chamsLandmine);
        UI.Checkbox("SettingsTab.Breaker", ref Settings.b_chamsBreaker);
        GUILayout.EndVertical();

        GUILayout.BeginVertical(GUILayout.Width(_fLeftWidth * 0.465f));
        UI.Checkbox("SettingsTab.Turrets", ref Settings.b_chamsTurret);
        UI.Checkbox("SettingsTab.Ship", ref Settings.b_chamsShip);
        UI.Checkbox("SettingsTab.SteamValves", ref Settings.b_chamsSteamHazard);
        UI.Checkbox("SettingsTab.BigDoors", ref Settings.b_chamsBigDoor);
        UI.Checkbox("SettingsTab.LockedDoors", ref Settings.b_chamsDoorLock);
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        UI.SubHeader("SettingsTab.EnemyTypes", true);

        var types = Enum.GetValues(typeof(EnemyAIType)).Cast<EnemyAIType>().ToList();

        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(_fLeftWidth * 0.465f));
        for (var i = 0; i < types.Count / 2; i++)
        {
            var type = types[i];
            if (type == EnemyAIType.Unknown) continue;
            UI.Checkbox(type.ToString(), type.IsEspEnabled(), () => type.ToggleEsp());
        }

        GUILayout.EndVertical();
        GUILayout.BeginVertical(GUILayout.Width(_fLeftWidth * 0.465f));
        for (var i = types.Count / 2; i < types.Count; i++)
        {
            var type = types[i];
            if (type == EnemyAIType.Unknown) continue;
            UI.Checkbox(type.ToString(), type.IsEspEnabled(), () => type.ToggleEsp());
        }

        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }


    private void ColorContent()
    {
        UI.Header("SettingsTab.ColorsHeader");

        UI.TextboxAction("SettingsTab.MenuBG", ref _sBgColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_background, _sBgColor))
        );
        UI.TextboxAction("SettingsTab.MenuText", ref _sMenuText, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_menuText, _sMenuText))
        );
        UI.TextboxAction("SettingsTab.Primary", ref _sPrimaryColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_primary, _sPrimaryColor))
        );
        UI.TextboxAction("SettingsTab.Crosshair", ref _sCrosshairColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_crosshair, _sCrosshairColor))
        );

        UI.Header("SettingsTab.ESPColors", true);
        UI.TextboxAction("SettingsTab.Chams", ref _sChamsColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_chams, _sChamsColor))
        );

        UI.TextboxAction("SettingsTab.Objects", ref _sObjectEspColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_objectESP, _sObjectEspColor))
        );
        UI.TextboxAction("SettingsTab.Enemies", ref _sEnemyEspColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_enemyESP, _sEnemyEspColor))
        );
        UI.TextboxAction("SettingsTab.Players", ref _sPlayerEspColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_playerESP, _sPlayerEspColor))
        );
        UI.TextboxAction("SettingsTab.EntExtDoors", ref _sDoorEspColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_entranceExitESP, _sDoorEspColor))
        );
        UI.TextboxAction("SettingsTab.Landmines", ref _sLandmineEspColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_landmineESP, _sLandmineEspColor))
        );
        UI.TextboxAction("SettingsTab.Turrets", ref _sTurretEspColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_turretESP, _sTurretEspColor))
        );
        UI.TextboxAction("SettingsTab.Ship", ref _sShipEspColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_shipESP, _sShipEspColor))
        );
        UI.TextboxAction("SettingsTab.SteamValves", ref _sValveEspColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_steamHazardESP, _sValveEspColor))
        );
        UI.TextboxAction("SettingsTab.BigDoors", ref _sBigDoorEspColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_bigDoorESP, _sBigDoorEspColor))
        );
        UI.TextboxAction("SettingsTab.LockedDoors", ref _sDoorLockEspColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_doorLockESP, _sDoorLockEspColor))
        );
        UI.TextboxAction("SettingsTab.Breaker", ref _sDoorLockEspColor, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_breakerESP, _sBreakerEspColor))
        );


        UI.Header("SettingsTab.TieredLootHeader", true);
        if (_sTierColorError != "") UI.Label(_sTierColorError, Settings.c_error);


        UI.Button(["SettingsTab.TieredLoot", $"({GetTiersColored()})"], EditTierColors, "General.Set");
        UI.Textbox("SettingsTab.Tiers", ref _sLootTiers, @"[^0-9,]");
        UI.Textbox("SettingsTab.Colors", ref _sLootTierColors, @"[^0-9A-Za-z,]");

        UI.Header("SettingsTab.OtherColors", true);

        UI.TextboxAction("SettingsTab.CauseOfDeath", ref _sCauseOfDeath, @"[^0-9A-Za-z]", 8,
            new UIButton("General.Set", () => SetColor(ref Settings.c_causeOfDeath, _sCauseOfDeath))
        );
    }

    private void KeybindContent()
    {
        UI.Header("SettingsTab.Keybinds");

        GUILayout.BeginVertical();
        _kbScrollPosition = GUILayout.BeginScrollView(_kbScrollPosition);
        UI.Textbox("General.Search", ref _sKbSearch, big: false);

        var hacks = Enum.GetValues(typeof(Hack)).Cast<Hack>().ToList()
            .FindAll(x => x.ToString().ToLower().Contains(_sKbSearch.ToLower()));

        foreach (var hack in hacks)
        {
            if (!hack.CanHaveKeyBind()) continue;

            GUILayout.BeginHorizontal();

            var bind = hack.GetKeyBind();

            var kb = hack.HasKeyBind()
                ? bind.GetType() == typeof(KeyControl) ? ((KeyControl)bind).keyCode.ToString() : bind.displayName
                : "None";


            GUILayout.Label(hack.ToString());
            GUILayout.FlexibleSpace();

            if (hack.HasKeyBind() && hack != Hack.OpenMenu && hack != Hack.UnlockDoorAction && GUILayout.Button("-"))
                hack.RemoveKeyBind();

            var btnText = hack.IsWaiting() ? "Waiting" : kb;
            if (GUILayout.Button(btnText, GUILayout.Width(85))) KbUtil.BeginChangeKeyBind(hack);


            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private static void SetColor(ref RgbaColor color, string hexCode)
    {
        while (hexCode.Length < 6) hexCode += "0";
        color = new RgbaColor(hexCode);
        Settings.Config.SaveConfig();
    }

    private void EditTierColors()
    {
        var thresholds = Array.ConvertAll(_sLootTiers.Split(','), x => int.TryParse(x, out var i) ? i : 0);
        var rgbaColors = Array.ConvertAll(_sLootTierColors.Split(','), x => new RgbaColor(x));

        if (thresholds.Length != rgbaColors.Length)
        {
            _sTierColorError = "SettingsTab.TierColorError";
            return;
        }

        Settings.i_scrapValueThresholds = thresholds;
        Settings.c_scrapValueColors = rgbaColors;

        Settings.Config.SaveConfig();
    }

    private static string GetTiersColored()
    {
        var tiers = new string[Settings.i_scrapValueThresholds.Length];

        for (var i = 0; i < Settings.i_scrapValueThresholds.Length; i++)
        {
            var threshold = Settings.i_scrapValueThresholds[i];
            var color = Settings.c_scrapValueColors[i];

            tiers[i] = color.AsString(threshold.ToString());
        }

        return string.Join(",", tiers);
    }
}