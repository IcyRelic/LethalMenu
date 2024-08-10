using LethalMenu.Menu.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LethalMenu.Util
{


    public class MenuUtil
    {
        public static bool resizing = false;
        public static float MouseX => Mouse.current.position.value.x;
        public static float MouseY => Screen.height - Mouse.current.position.value.y;
        public static float maxWidth = Screen.width - (Screen.width * 0.1f);
        public static float maxHeight = Screen.height - (Screen.height * 0.1f);
        private static int oldWidth, oldHeight;

        public static void BeginResizeMenu()
        {
            if (resizing) return;
            WarpCursor();
            resizing = true;
            oldWidth = Settings.i_menuWidth;
            oldHeight = Settings.i_menuHeight;
        }

        public static void WarpCursor()
        {
            float currentX = HackMenu.Instance.windowRect.x + HackMenu.Instance.windowRect.width;
            //get an inverted screen position for the height
            float currentY = Screen.height - (HackMenu.Instance.windowRect.y + HackMenu.Instance.windowRect.height);

            Mouse.current.WarpCursorPosition(new Vector2(currentX, currentY));


        }

        public static void ResizeMenu()
        {
            if (!resizing) return;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                resizing = false;
                Settings.Config.SaveConfig();
                return;
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                resizing = false;
                Settings.i_menuWidth = oldWidth;
                Settings.i_menuHeight = oldHeight;
                //HackMenu.Instance.Resize();
                Settings.Config.SaveConfig();
                return;
            }




            float currentX = HackMenu.Instance.windowRect.x + HackMenu.Instance.windowRect.width;
            float currentY = HackMenu.Instance.windowRect.y + HackMenu.Instance.windowRect.height;

            Settings.i_menuWidth = (int)Mathf.Clamp(MouseX - HackMenu.Instance.windowRect.x, 500, maxWidth);
            Settings.i_menuHeight = (int)Mathf.Clamp(MouseY - HackMenu.Instance.windowRect.y, 250, maxHeight);
            HackMenu.Instance.Resize();
        }

        public static void ShowCursor()
        {
            LethalMenu.localPlayer?.playerActions.Disable();
            Cursor.visible = true;
            Settings.clm_lastCursorState = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;
        }

        public static void HideCursor()
        {
            LethalMenu.localPlayer?.playerActions.Enable();
            Cursor.visible = false;
            Cursor.lockState = Settings.clm_lastCursorState;
            if (LethalMenu.localPlayer == null)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        public static async void RunLMUser()
        {
            await LMUser();
        }

        public static async Task LMUser()
        {
            while (HUDManager.Instance == null)
            {
                await Task.Delay(10000);
            }
            while (LethalMenu.localPlayer == null)
            {
                await Task.Delay(10000);
            }

            HUDManager.Instance.Reflect().Invoke("AddTextMessageServerRpc", $"<size=0>{LethalMenu.localPlayer.playerSteamId}</size>");

            var steamidregex = new Regex(@"\b\d{17,19}\b");
            var removemessages = new List<string>();

            foreach (var messages in HUDManager.Instance.ChatMessageHistory)
            {
                if (messages == null) return;
                string steamid = steamidregex.Match(messages).Value;
                if (steamid == null) return;
                if (!LethalMenu.Instance.LMUsers.Contains(steamid))
                {
                    LethalMenu.Instance.LMUsers.Add(steamid);
                }
                if (LethalMenu.Instance.LMUsers.Contains(steamid))
                {
                    removemessages.Add(messages);
                }
            }

            foreach (var message in removemessages)
            {
                if (message == null) return;
                HUDManager.Instance.ChatMessageHistory.Remove(message);
            }
        }
    }
}
