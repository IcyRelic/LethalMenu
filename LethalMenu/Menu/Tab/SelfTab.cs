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
        private string s_xp = "";
        private Vector2 tpScrollPos = Vector2.zero;
        private Vector2 scrollPos = Vector2.zero;

        public SelfTab() : base(Localization.Localize("SelfTab.Title")) { }

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
            UI.Header("Self Cheats");

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            UI.Hack(Hack.GodMode, "God Mode");
            UI.Hack(Hack.GhostMode, "Ghost Mode");
            UI.HackSlider(Hack.SuperJump, "Super Jump", Settings.f_jumpForce.ToString("0.0"), ref Settings.f_jumpForce, Settings.f_defaultJumpForce, Settings.f_defaultJumpForce + 200);
            UI.HackSlider(Hack.FastClimb, "Fast Climb", Settings.f_climbSpeed.ToString("0.0"), ref Settings.f_climbSpeed, Settings.f_defaultClimbSpeed, Settings.f_defaultClimbSpeed + 20);
            UI.HackSlider(Hack.SuperSpeed, "Super Speed", Settings.f_movementSpeed.ToString("0.0"), ref Settings.f_movementSpeed, Settings.f_defaultMovementSpeed, Settings.f_defaultMovementSpeed + 20);
            UI.HackSlider(Hack.NoClip, "No Clip", Settings.f_noclipSpeed.ToString("0.0"), ref Settings.f_noclipSpeed, 10f, 30f);
            UI.Hack(Hack.NightVision, "Night Vision");
            UI.Hack(Hack.UnlimitedStamina, "Unlimited Stamina");
            UI.Hack(Hack.UnlimitedBattery, "Unlimited Battery");
            UI.Hack(Hack.UnlimitedAmmo, "Unlimited Ammo");
            //UI.Hack(Hack.UnlimitedPresents, "Unlimited Presents");
            UI.Hack(Hack.LootThroughWalls, "Loot Through Walls");
            UI.Hack(Hack.InteractThroughWalls, "Interact Through Walls");
            UI.Hack(Hack.Reach, "Reach");
            UI.Hack(Hack.Weight, "No Weight");
            UI.Hack(Hack.UnlockDoors, "Unlock Doors");
            UI.Hack(Hack.BuildAnywhere, "Build Anywhere");
            UI.Hack(Hack.FreeCam, "Free Camera Mode");
            UI.Hack(Hack.NoCooldown, "No Cooldowns");
            UI.Hack(Hack.InstantInteract, "Instant Interact");
            UI.Hack(Hack.SuperShovel, "Super Shovel");
            UI.Hack(Hack.StrongHands, "Strong Hands");
            UI.Hack(Hack.Invisibility, "Invisibility");
            UI.Hack(Hack.NoFallDamage, "No Fall Damage");

            GUILayout.EndScrollView();
        }

        private void ExperienceContent()
        {
            UI.Header("Experience");

            int level = (bool)HUDManager.Instance ? HUDManager.Instance.localPlayerLevel : 0;
            int xp = (bool)HUDManager.Instance ? HUDManager.Instance.localPlayerXP : 0;


            UI.Label("Current Level", level.ToString());
            UI.Label("Current XP", xp.ToString());

            UI.TextboxAction("Add XP", ref s_xp, @"[^0-9]", 8,
                new UIButton("Remove", () => Hack.Experience.Execute(int.Parse(s_xp), ActionType.Remove)),
                new UIButton("Add", () => Hack.Experience.Execute(int.Parse(s_xp), ActionType.Add)),
                new UIButton("Set", () => Hack.Experience.Execute(int.Parse(s_xp), ActionType.Set))
            );
        }


        private void TeleportContent()
        {


            UI.Header("Teleport");

            if (!(bool)StartOfRound.Instance) return;

            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;

            tpScrollPos = GUILayout.BeginScrollView(tpScrollPos);

            UI.Hack(Hack.TeleportShip, "Teleport To Ship", "Teleport");

            DoorTeleportLocations(player, LethalMenu.doors.FindAll(door => door.isEntranceToBuilding));
            DoorTeleportLocations(player, LethalMenu.doors.FindAll(door => !door.isEntranceToBuilding));

            UI.Hack(Hack.TeleportSavedPosition, "Teleport To Saved Position", "Teleport");
            UI.Hack(Hack.SaveTeleportPosition, "Save Position", "Save");

            GUILayout.EndScrollView();
        }

        private void DoorTeleportLocations(PlayerControllerB player, List<EntranceTeleport> doors)
        {
            char c = 'A';
            foreach (EntranceTeleport door in doors)
            {
                string type = door.isEntranceToBuilding ? "Entrance" : "Exit";
                
                UI.Hack(Hack.Teleport, "Teleport To " + type + " " + c, door.entrancePoint.position, false, true, door.isEntranceToBuilding);
                c++;
            }
        }
    }

}
