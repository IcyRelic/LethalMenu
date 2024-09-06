using GameNetcodeStuff;
using LethalMenu.Language;
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Util;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace LethalMenu.Menu.Tab
{
    internal class SelfTab : MenuTab
    {
        private Vector2 scrollPos = Vector2.zero;
        private Vector2 scrollPos2 = Vector2.zero;
        private string s_xp = "";

        public SelfTab() : base("SelfTab.Title") { }

        public override void Draw()
        {
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
            SelfHackContent();
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(HackMenu.Instance.contentWidth * 0.5f - HackMenu.Instance.spaceFromLeft));
            ExperienceContent();
            TeleportContent();
            GUILayout.EndVertical();
        }

        private void SelfHackContent()
        {
            UI.Header("SelfTab.Title");

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            UI.Hack(Hack.GodMode, "SelfTab.GodMode");
            UI.Hack(Hack.GhostMode, "SelfTab.GhostMode");
            UI.HackSlider(Hack.SuperJump, "SelfTab.SuperJump", Settings.f_jumpForce.ToString("0.0"), ref Settings.f_jumpForce, Settings.f_defaultJumpForce, Settings.f_defaultJumpForce + 200);
            UI.HackSlider(Hack.FastClimb, "SelfTab.FastClimb", Settings.f_climbSpeed.ToString("0.0"), ref Settings.f_climbSpeed, Settings.f_defaultClimbSpeed, Settings.f_defaultClimbSpeed + 20);
            UI.HackSlider(Hack.SuperSpeed, "SelfTab.SuperSpeed", Settings.f_movementSpeed.ToString("0.0"), ref Settings.f_movementSpeed, Settings.f_defaultMovementSpeed, Settings.f_defaultMovementSpeed + 20);
            UI.HackSlider(Hack.NoClip, "SelfTab.NoClip", Settings.f_noclipSpeed.ToString("0.0"), ref Settings.f_noclipSpeed, 10f, 30f);
            UI.Hack(Hack.ClickTeleport, "SelfTab.ClickTeleport");
            UI.HackSlider(Hack.ItemSlots, ["SelfTab.ItemSlots", "General.RejoinRequired"], Settings.f_slots.ToString("0.0"), ref Settings.f_slots, 1f, 20f);
            UI.Hack(Hack.NightVision, "SelfTab.NightVision");
            UI.Hack(Hack.UnlimitedStamina, "SelfTab.UnlimitedStamina");
            UI.Hack(Hack.UnlimitedBattery, "SelfTab.UnlimitedBattery");
            UI.Hack(Hack.UnlimitedAmmo, "SelfTab.UnlimitedAmmo");
            UI.Hack(Hack.UnlimitedOxygen, "SelfTab.UnlimitedOxygen");
            UI.Hack(Hack.UnlimitedZapGun, "SelfTab.UnlimitedZapGun");
            UI.Hack(Hack.UnlimitedPresents, "SelfTab.UnlimitedPresents");
            UI.Hack(Hack.LootThroughWalls, "SelfTab.LootThroughWalls");
            UI.Hack(Hack.InteractThroughWalls, "SelfTab.InteractThroughWalls");
            UI.Hack(Hack.Reach, "SelfTab.Reach");
            UI.Hack(Hack.Weight, "SelfTab.Weight");
            UI.Hack(Hack.UnlockDoors, "SelfTab.UnlockDoors");
            UI.Hack(Hack.BuildAnywhere, "SelfTab.BuildAnywhere");
            UI.Hack(Hack.FreeCam, "SelfTab.FreeCam");
            UI.Hack(Hack.NoCooldown, "SelfTab.NoCooldown");
            UI.Hack(Hack.InstantInteract, "SelfTab.InstantInteract");
            UI.Hack(Hack.SuperShovel, "SelfTab.SuperShovel");
            UI.Hack(Hack.SuperKnife, "SelfTab.SuperKnife");
            UI.Hack(Hack.StrongHands, "SelfTab.StrongHands");
            UI.Hack(Hack.Invisibility, "SelfTab.Invisibility");
            UI.Hack(Hack.NoFallDamage, "SelfTab.NoFallDamage");
            UI.Hack(Hack.HearAllAlivePeople, "SelfTab.HearAllAlivePeople");
            UI.Hack(Hack.HearAllDeadPeople, "SelfTab.HearAllDeadPeople");
            UI.Hack(Hack.NoFlash, "SelfTab.NoFlash");
            UI.Hack(Hack.NoQuicksand, "SelfTab.NoQuicksand");
            UI.Hack(Hack.NoCameraShake, "SelfTab.NoCameraShake");
            UI.Hack(Hack.TeleportWithItems, "SelfTab.TeleportWithItems");
            UI.Hack(Hack.BridgeNeverFalls, ["SelfTab.BridgeNeverFalls", "General.Vow/Adamance"]);
            UI.Hack(Hack.DropAllItems, "SelfTab.DropAllItems");
            UI.Hack(Hack.DeleteHeldItem, "SelfTab.DeleteHeldItem");
            UI.Hack(Hack.VoteShipLeaveEarly, "SelfTab.VoteShipLeaveEarly");
            UI.Hack(Hack.VehicleGodMode, "SelfTab.VehicleGodMode");
            UI.Hack(Hack.EggsNeverExplode, "SelfTab.EggsNeverExplode");
            UI.Hack(Hack.UnlockAllDoors, "SelfTab.UnlockAllDoors");
            UI.Hack(Hack.OpenAllBigDoors, "SelfTab.OpenAllBigDoors");
            UI.Hack(Hack.CloseAllBigDoors, "SelfTab.CloseAllBigDoors");
            UI.Hack(Hack.GrabItemsBeforeGame, "SelfTab.GrabItemsBeforeGame");

            GUILayout.EndScrollView();
        }

        private void ExperienceContent()
        {
            UI.Header("SelfTab.Experience");

            int level = (bool)HUDManager.Instance ? HUDManager.Instance.localPlayerLevel : 0;
            int xp = (bool)HUDManager.Instance ? HUDManager.Instance.localPlayerXP : 0;

            UI.Label("SelfTab.Level", level.ToString());
            UI.Label("SelfTab.XP", xp.ToString());

            UI.TextboxAction("SelfTab.AddXP", ref s_xp, @"[^0-9]", 8,
                new UIButton("General.Remove", () => Hack.Experience.Execute(int.Parse(s_xp), ActionType.Remove)),
                new UIButton("General.Add", () => Hack.Experience.Execute(int.Parse(s_xp), ActionType.Add)),
                new UIButton("General.Set", () => Hack.Experience.Execute(int.Parse(s_xp), ActionType.Set))
            );
        }


        private void TeleportContent()
        {
            UI.Header("SelfTab.TeleportTitle");

            if (!(bool)StartOfRound.Instance) return;

            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;

            scrollPos2 = GUILayout.BeginScrollView(scrollPos2);

            UI.Hack(Hack.TeleportShip, "SelfTab.TeleportShip", "SelfTab.Teleport");

            DoorTeleportLocations(player, LethalMenu.doors.FindAll(door => door.isEntranceToBuilding));
            DoorTeleportLocations(player, LethalMenu.doors.FindAll(door => !door.isEntranceToBuilding));

            UI.Hack(Hack.TeleportSavedPosition, "SelfTab.TeleportSaved", "SelfTab.Teleport");
            UI.Hack(Hack.SaveTeleportPosition, "SelfTab.SavePosition", "General.Save");

            GUILayout.EndScrollView();
        }

        private void DoorTeleportLocations(PlayerControllerB player, List<EntranceTeleport> doors)
        {
            char c = 'A';
            foreach (EntranceTeleport door in doors)
            {
                string textKey = door.isEntranceToBuilding ? "SelfTab.TeleportEntrance" : "SelfTab.TeleportExit";
                string localizedText = Localization.Localize(textKey);

                UI.Hack(Hack.Teleport, $"{localizedText} {c}", door.entrancePoint.position, false, true, door.isEntranceToBuilding);
                c++;
            }
        }
    }
}