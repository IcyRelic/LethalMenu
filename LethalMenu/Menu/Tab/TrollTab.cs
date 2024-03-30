using LethalMenu.Menu.Core;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Menu.Tab;

internal class TrollTab() : MenuTab("TrollTab.Title")
{
    private Vector2 _scrollPosition = Vector2.zero;

    public override void Draw()
    {
        GUILayout.BeginVertical(
            GUILayout.Width(HackMenu.Instance.ContentWidth * 0.5f - HackMenu.SpaceFromLeft));
        TrollMenuContent();
        GUILayout.EndVertical();
    }

    private void TrollMenuContent()
    {
        UI.Header("TrollTab.Title");

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        UI.Hack(Hack.ToggleShipHorn, "TrollTab.ShipHorn");
        UI.Hack(Hack.ToggleShipLights, "TrollTab.ShipLights");
        UI.Hack(Hack.ToggleFactoryLights, ["TrollTab.FactoryLights", "General.HostTag"]);
        UI.Hack(Hack.FlickerLights, ["TrollTab.FlickerFactoryLights", "General.HostTag"]);
        UI.Hack(Hack.ForceBridgeFall, ["TrollTab.BridgeFall", "General.HostVowTag"]);
        UI.Hack(Hack.BlowUpAllLandmines, "TrollTab.BlowAllMines");
        UI.Hack(Hack.ToggleAllLandmines, "TrollTab.ToggleMines");
        UI.Hack(Hack.ToggleAllTurrets, "TrollTab.ToggleTurrets");
        UI.Hack(Hack.ForceTentacleAttack, "TrollTab.TentacleAttack");
        UI.Hack(Hack.FixAllValves, "TrollTab.FixValves");
        UI.Hack(Hack.SpawnMaskedEnemy, "TrollTab.SpawnMasks");
        UI.Hack(Hack.SellEverything, "TrollTab.SellEverything");
        UI.Hack(Hack.TeleportAllItems, "TrollTab.TeleportAllItems");
        UI.Hack(Hack.TeleportOneItem, "TrollTab.TeleportOneItem");
        UI.Hack(Hack.EjectEveryone, ["TrollTab.EjectEveryone", "General.HostTag"]);

        GUILayout.EndScrollView();
    }
}