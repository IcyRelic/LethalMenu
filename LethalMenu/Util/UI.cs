using System;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace LethalMenu.Util
{
    public enum UIElementType
    {
        Button,
        Toggle,
        Slider,
        Label,
        Textbox,
    }

    public class UIButton
    {
        public string label;
        public Action action;

        public UIButton(string label, Action action)
        {
            this.label = label;
            this.action = action;
        }

        public void Draw()
        {
            if (GUILayout.Button(label)) action.Invoke();
        }
    }

    public class UIOption
    {
        public string label;
        public object value;
        public Action action;


        public UIOption(string label, object value)
        {
            this.label = label;
            this.value = value;
        }

        public UIOption(string label, Action action)
        {
            this.label = label;
            this.action = action;
        }

        public void Draw(ref object refValue)
        {
            if (GUILayout.Button(label)) refValue = value;
        }

        public void Draw()
        {
            if (GUILayout.Button(label)) action.Invoke();
        }
    }


    public class UI
    {
        public static void Header(string header, bool space = false)
        {
            if (space) GUILayout.Space(20);
            GUILayout.Label(header, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
        }

        public static void SubHeader(string header, bool space = false)
        {
            if (space) GUILayout.Space(20);
            GUILayout.Label(header, new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        }

        public static void Label(string header, string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header);
            GUILayout.FlexibleSpace();
            GUILayout.Label(label);
            GUILayout.EndHorizontal();
        }

        public static void Label(string label)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            GUILayout.EndHorizontal();
        }

        public static void Hack(Hack hack, string header, params object[] param)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header);
            GUILayout.FlexibleSpace();
            if (hack.Button()) hack.Execute(param);
            GUILayout.EndHorizontal();
        }

        public static void Hack(Hack hack, string header, string buttonText, params object[] param)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(buttonText)) hack.Execute(param);
            GUILayout.EndHorizontal();
        }

        public static void Button(string header, Action action, string btnText = "Execute")
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(btnText)) action();
            GUILayout.EndHorizontal();
        }
        public static void Toggle(string header, ref bool value)
        {
            Toggle(header, ref value, "Disable", "Enable");
        }

        public static void Toggle(string header, ref bool value, string enabled, string disabled)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(value ? enabled : disabled)) value = !value;
            GUILayout.EndHorizontal();
        }


        public static void HackSlider(Hack hack, string header, string displayValue, ref float value, float min, float max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header + " ( " + displayValue + " )");
            GUILayout.FlexibleSpace();

            GUIStyle slider = new GUIStyle(GUI.skin.horizontalSlider) { alignment = TextAnchor.MiddleCenter, fixedWidth = Settings.GUISize.GetSliderWidth() };
            

            value = GUILayout.HorizontalSlider(value, min, max, slider, GUI.skin.horizontalSliderThumb);
            if (hack.Button()) hack.Execute();
            GUILayout.EndHorizontal();
        }

        public static void Textbox(string label, ref string value, string regex)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            GUILayout.FlexibleSpace();
            value = GUILayout.TextField(value, GUILayout.Width(Settings.GUISize.GetTextboxWidth() * 3));
            value = Regex.Replace(value, regex, "");
            GUILayout.EndHorizontal();
        }

        public static void TextboxAction(string label, ref string value, string regex, int length, params UIButton[] buttons)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label);
            GUILayout.FlexibleSpace();
            value = GUILayout.TextField(value, length, GUILayout.Width(Settings.GUISize.GetTextboxWidth()));
            value = Regex.Replace(value, regex, "");
            buttons.ToList().ForEach(btn => btn.Draw());
            GUILayout.EndHorizontal();
        }

        public static void Actions(params UIButton[] buttons)
        {
            GUILayout.BeginHorizontal();
            buttons.ToList().ForEach(btn => btn.Draw());
            GUILayout.EndHorizontal();
        }

        public static void Slider(string header, string displayValue, ref float value, float min, float max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header + " ( " + displayValue + " )");
            GUILayout.FlexibleSpace();
            value = GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(Settings.GUISize.GetSliderWidth()));
            GUILayout.EndHorizontal();
        }

        public static void Checkbox(string header, ref bool value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header);
            GUILayout.FlexibleSpace();
            value = GUILayout.Toggle(value, "");
            GUILayout.EndHorizontal();
        }

        public static void Checkbox(string header, bool isChecked, Action onChanged)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header);
            GUILayout.FlexibleSpace();
            bool modifiedValue = GUILayout.Toggle(isChecked, "");
            
            if(modifiedValue != isChecked) onChanged.Invoke();
            GUILayout.EndHorizontal();
        }

        public static void Select(string header, ref object value, ref int index, params UIOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header);
            GUILayout.FlexibleSpace();

            options[index].Draw(ref value);

            if (GUILayout.Button("-")) index = Mathf.Clamp(index-1, 0, options.Length - 1);
            if (GUILayout.Button("+")) index = Mathf.Clamp(index+1, 0, options.Length - 1);


            GUILayout.EndHorizontal();
        }

        public static void Select(string header, ref int index, params UIOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(header);
            GUILayout.FlexibleSpace();

            options[index].Draw();

            if (GUILayout.Button("-")) index = Mathf.Clamp(index-1, 0, options.Length - 1);
            if (GUILayout.Button("+")) index = Mathf.Clamp(index+1, 0, options.Length - 1);


            GUILayout.EndHorizontal();
        }
    }
}
