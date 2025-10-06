using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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

            public IEnumerator Invoke(ButtonControl btn)
            {
                hack.SetKeyBind(btn);
                yield return new WaitForSeconds(0.1f);
                hack.SetWaiting(false);
                Settings.Config.SaveConfig();
            }
        }

        public static void BeginChangeKeyBind(Hack hack, params Action[] callbacks)
        {
            hack.SetWaiting(true);
            LethalMenu.Instance.StartCoroutine(TryGetPressedKeyCoroutine(new KBCallback(hack).Invoke, callbacks));
        }

        private static IEnumerator TryGetPressedKeyCoroutine(Func<ButtonControl, IEnumerator> callback, params Action[] otherCallbacks)
        {
            yield return new WaitForSeconds(0.25f);
            float startTime = Time.time;
            ButtonControl btn = null;

            while (btn == null && Time.time - startTime <= 15f)
            {
                ButtonControl pressed = GetPressedBtn(); 
                if (pressed != null && pressed != btn) btn = pressed;
                yield return null;
            }
            if (btn == null) yield break;
            LethalMenu.Instance.StartCoroutine(callback(btn));
            foreach (var cb in otherCallbacks) cb?.Invoke();
        }

        public static ButtonControl GetPressedBtn()
        {
            if (Keyboard.current != null)
            {
                foreach (KeyControl key in Keyboard.current.allKeys)
                {
                    if (key != null && key.wasPressedThisFrame) return key;
                }
            }
            if (Mouse.current != null)
            {
                ButtonControl[] mouseButtons =
                {
                    Mouse.current.leftButton,
                    Mouse.current.rightButton,
                    Mouse.current.middleButton,
                    Mouse.current.forwardButton,
                    Mouse.current.backButton
                };
                foreach (ButtonControl btn in mouseButtons)
                {
                    if (btn != null && btn.wasPressedThisFrame) return btn;
                }
            }
            return null;
        }
    }
}
