using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace LethalMenu.Util;

public static class KbUtil
{
    public static void BeginChangeKeyBind(Hack hack, params Action[] callbacks)
    {
        hack.SetWaiting(true);
        _ = TryGetPressedKeyTask(new KbCallback(hack).Invoke, callbacks);
    }

    private static async Task TryGetPressedKeyTask(Action<ButtonControl> callback, params Action[] otherCallbacks)
    {
        await Task.Run(async () =>
        {
            //wait .5 seconds

            await Task.Delay(250);

            var startTime = Time.time;
            ButtonControl btn = null;
            do
            {
                var pressed = GetPressedBtn();


                if (pressed != null)
                    //if (!HackExtensions.KeyBindInUse(pressed)) 
                    btn = pressed;
                //else kbError = "SettingsTab.BindInUseError";
                if (Time.time - startTime > 15f) break;
            } while (btn == null);

            if (btn == null) return;

            callback?.Invoke(btn);
            otherCallbacks.ToList().ForEach(cb => cb?.Invoke());
        });
    }

    private static ButtonControl GetPressedBtn()
    {
        foreach (var key in Keyboard.current.allKeys.Where(key => key.wasPressedThisFrame)) return key;

        var mouseButtons = new[]
        {
            Mouse.current.leftButton, Mouse.current.rightButton, Mouse.current.middleButton,
            Mouse.current.forwardButton, Mouse.current.backButton
        };

        return mouseButtons.FirstOrDefault(btn => btn.wasPressedThisFrame);
    }

    private class KbCallback(Hack hack)
    {
        public async void Invoke(ButtonControl btn)
        {
            hack.SetKeyBind(btn);
            await Task.Delay(100);
            hack.SetWaiting(false);
            Settings.Config.SaveConfig();
        }
    }
}