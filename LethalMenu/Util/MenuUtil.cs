using LethalMenu.Menu.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LethalMenu.Util;

public static class MenuUtil
{
    public static bool Resizing;
    private static readonly float MaxWidth = Screen.width - Screen.width * 0.1f;
    private static readonly float MaxHeight = Screen.height - Screen.height * 0.1f;
    private static int _oldWidth, _oldHeight;
    private static float MouseX => Mouse.current.position.value.x;
    private static float MouseY => Screen.height - Mouse.current.position.value.y;


    public static void BeginResizeMenu()
    {
        if (Resizing) return;
        WarpCursor();
        Resizing = true;
        _oldWidth = Settings.i_menuWidth;
        _oldHeight = Settings.i_menuHeight;
    }

    private static void WarpCursor()
    {
        var currentX = HackMenu.Instance.WindowRect.x + HackMenu.Instance.WindowRect.width;
        //get an inverted screen position for the height
        var currentY = Screen.height - (HackMenu.Instance.WindowRect.y + HackMenu.Instance.WindowRect.height);

        Mouse.current.WarpCursorPosition(new Vector2(currentX, currentY));
    }

    public static void ResizeMenu()
    {
        if (!Resizing) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Resizing = false;
            Settings.Config.SaveConfig();
            return;
        }

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Resizing = false;
            Settings.i_menuWidth = _oldWidth;
            Settings.i_menuHeight = _oldHeight;
            //HackMenu.Instance.Resize();
            Settings.Config.SaveConfig();
            return;
        }


        var currentX = HackMenu.Instance.WindowRect.x + HackMenu.Instance.WindowRect.width;
        var currentY = HackMenu.Instance.WindowRect.y + HackMenu.Instance.WindowRect.height;

        Settings.i_menuWidth = (int)Mathf.Clamp(MouseX - HackMenu.Instance.WindowRect.x, 500, MaxWidth);
        Settings.i_menuHeight = (int)Mathf.Clamp(MouseY - HackMenu.Instance.WindowRect.y, 250, MaxHeight);
        HackMenu.Instance.Resize();
    }

    public static void ShowCursor()
    {
        LethalMenu.LocalPlayer.playerActions.Disable();
        Cursor.visible = true;
        Settings.clm_lastCursorState = Cursor.lockState;
        Cursor.lockState = CursorLockMode.None;
    }

    public static void HideCursor()
    {
        LethalMenu.LocalPlayer.playerActions.Enable();
        Cursor.visible = false;
        Cursor.lockState = Settings.clm_lastCursorState;
    }
}