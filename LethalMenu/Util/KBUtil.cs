using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using UnityEngine;

namespace LethalMenu.Util
{
    public class KBUtil
    {
        internal class KBCallback
        {
            private Hack hack;

            public KBCallback(Hack hack)
            {
                this.hack = hack;
            }

            public async void Invoke(ButtonControl btn)
            {
                hack.SetKeyBind(btn);
                await Task.Delay(100);
                hack.SetWaiting(false);
                Settings.Config.SaveConfig();
            }
            
        }

        public static void BeginChangeKeyBind(Hack hack)
        {
            hack.SetWaiting(true);
            _ = TryGetPressedKeyTask(new KBCallback(hack).Invoke);
        }

        private static async Task TryGetPressedKeyTask(Action<ButtonControl> callback)
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
                        //if (!HackExtensions.KeyBindInUse(pressed)) 
                        btn = pressed;
                        //else kbError = "SettingsTab.BindInUseError";
                    }

                    if (Time.time - startTime > 15f) break;
                } while (btn == null);

                if (btn == null) return;

                callback?.Invoke(btn);
            });


        }

        private static ButtonControl GetPressedBtn()
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

    }
}
