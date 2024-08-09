using LethalMenu.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System.IO;

namespace LethalMenu.Util
{

    public class UIButton
    {
        public string label;
        public Action action;
        private GUIStyle style = null;

        public UIButton(string label, Action action, GUIStyle style = null)
        {
            this.label = Localization.Localize(label);
            this.action = action;

            this.style = style ;
        }

        public void Draw()
        {
            if(style != null ? GUILayout.Button(label, style) : GUILayout.Button(label)) action.Invoke();
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
            GUILayout.Label(Localization.Localize(header), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
        }

        public static void SubHeader(string header, bool space = false)
        {
            if (space) GUILayout.Space(20);
            GUILayout.Label(Localization.Localize(header), new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        }

        public static void Label(string header, string label, RGBAColor color = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();
            GUILayout.Label(color is null ? Localization.Localize(label) : color.AsString(label));
            GUILayout.EndHorizontal();
        }

        public static void Label(string label, RGBAColor color = null)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(color is null ? Localization.Localize(label) : color.AsString(label));
            GUILayout.EndHorizontal();
        }

        public static void Hack(Hack hack, string header, params object[] param)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();
            if (hack.Button()) hack.Execute(param);
            GUILayout.EndHorizontal();
        }

        public static void Hack(Hack hack, string[] header, params object[] param)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();
            if (hack.Button()) hack.Execute(param);
            GUILayout.EndHorizontal();
        }

        public static void Hack(Hack hack, string header, string buttonText, params object[] param)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(Localization.Localize(buttonText))) hack.Execute(param);
            GUILayout.EndHorizontal();
        }

        public static void Button(string header, Action action, string btnText = "General.Execute")
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(Localization.Localize(btnText))) action();
            GUILayout.EndHorizontal();
        }

        public static void Button(string[] header, Action action, string btnText = "General.Execute")
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(Localization.Localize(btnText))) action();
            GUILayout.EndHorizontal();
        }

        public static void Toggle(string header, ref bool value, string enabled, string disabled, params Action<bool>[] action)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();
            bool newValue = !value;
            if (GUILayout.Button(Localization.Localize(value ? enabled : disabled)))
            {
                value = newValue;
                action.ToList().ForEach(a => a.Invoke(newValue));
            }
            GUILayout.EndHorizontal();
        }


        public static void HackSlider(Hack hack, string header, string displayValue, ref float value, float min, float max, params object[] param)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header) + " ( " + displayValue + " )");
            GUILayout.FlexibleSpace();

            GUIStyle slider = new GUIStyle(GUI.skin.horizontalSlider) { alignment = TextAnchor.MiddleCenter, fixedWidth = Settings.i_sliderWidth };


            value = GUILayout.HorizontalSlider(value, min, max, slider, GUI.skin.horizontalSliderThumb);

            if (hack.Button()) hack.Execute(param);
            GUILayout.EndHorizontal();
        }

        public static void Textbox(string label, ref string value, string regex = "", bool big = true)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(label));
            GUILayout.FlexibleSpace();
            value = GUILayout.TextField(value, GUILayout.Width(big ? Settings.i_textboxWidth * 3 : Settings.i_textboxWidth));
            value = Regex.Replace(value, regex, "");
            GUILayout.EndHorizontal();
        }

        public static void TextboxAction(string[] label, ref string value, string regex, int length, params UIButton[] buttons)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(label));
            GUILayout.FlexibleSpace();
            value = GUILayout.TextField(value, length, GUILayout.Width(Settings.i_textboxWidth));
            value = Regex.Replace(value, regex, "");
            buttons.ToList().ForEach(btn => btn.Draw());
            GUILayout.EndHorizontal();
        }
        public static void TextboxAction(string label, ref string value, string regex, int length, params UIButton[] buttons)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(label));
            GUILayout.FlexibleSpace();
            value = GUILayout.TextField(value, length, GUILayout.Width(Settings.i_textboxWidth));
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
            GUILayout.Label(Localization.Localize(header) + " ( " + displayValue + " )");
            GUILayout.FlexibleSpace();
            value = GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(Settings.i_sliderWidth));
            GUILayout.EndHorizontal();
        }

        public static void SliderAction(string header, string displayValue, ref float value, float min, float max, float defaultValue)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header) + " ( " + displayValue + " )");
            GUILayout.FlexibleSpace();
            value = GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(Settings.i_sliderWidth));
            if (GUILayout.Button(Localization.Localize("General.Reset")))
            {
                value = defaultValue;
            }
            GUILayout.EndHorizontal();
        }

        public static void SliderAction(string header, string displayvalue, ref int value, int min, int max, int defaultvalue)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{Localization.Localize(header)} {displayvalue}");
            GUILayout.FlexibleSpace();
            value = Mathf.RoundToInt(GUILayout.HorizontalSlider(value, min, max, GUILayout.Width(Settings.i_sliderWidth)));
            if (GUILayout.Button(Localization.Localize("General.Reset")))
            {
                value = defaultvalue;
            }
            GUILayout.EndHorizontal();
        }

        public static void NumSelect(string header, ref int value, int min, int max)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();
            GUILayout.Label(value.ToString());
            if (GUILayout.Button("-")) value = Mathf.Clamp(value - 1, min, max);
            if (GUILayout.Button("+")) value = Mathf.Clamp(value + 1, min, max);
            GUILayout.EndHorizontal();
        }

        public static void PercentSelect(string header, ref float value, float min = 0.1f)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();
            GUILayout.Label(value.ToString());
            if (GUILayout.Button("-")) value = Mathf.Clamp(value - 0.01f, min, 1);
            if (GUILayout.Button("+")) value = Mathf.Clamp(value + 0.01f, min, 1);
            GUILayout.EndHorizontal();
        }

        public static void Checkbox(string header, ref bool value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();
            value = GUILayout.Toggle(value, "");
            GUILayout.EndHorizontal();
        }

        public static void Checkbox(string header, bool isChecked, Action onChanged)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();
            bool modifiedValue = GUILayout.Toggle(isChecked, "");

            if (modifiedValue != isChecked) onChanged.Invoke();
            GUILayout.EndHorizontal();
        }

        public static void IndexSelect(string header, ref int index, params string[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();

            GUILayout.Label(Localization.Localize(options[index]));

            if (GUILayout.Button("-")) index = index == 0 ? options.Length - 1 : Mathf.Clamp(index - 1, 0, options.Length - 1);
            if (GUILayout.Button("+")) index = index == options.Length - 1 ? 0 : Mathf.Clamp(index + 1, 0, options.Length - 1);


            GUILayout.EndHorizontal();
        }

        public static void IndexSelectAction(string header, ref int index, params string[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label($"{Localization.Localize(header)} {options[index]}");
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("-"))
            {
                index = index == 0 ? options.Length - 1 : Mathf.Clamp(index - 1, 0, options.Length - 1);
                ThemeUtil.ApplyTheme(options[index]);
            }

            if (GUILayout.Button("+"))
            {
                index = index == options.Length - 1 ? 0 : Mathf.Clamp(index + 1, 0, options.Length - 1);
                ThemeUtil.ApplyTheme(options[index]);
            }

            GUILayout.EndHorizontal();
        }

        public static void Select(string header, ref int index, params UIOption[] options)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(Localization.Localize(header));
            GUILayout.FlexibleSpace();

            options[index].Draw();

            if (GUILayout.Button("-")) index = Mathf.Clamp(index - 1, 0, options.Length - 1);
            if (GUILayout.Button("+")) index = Mathf.Clamp(index + 1, 0, options.Length - 1);


            GUILayout.EndHorizontal();
        }

        public static void ButtonGrid<T>(List<T> objects, Func<T, string> textSelector, string search, Action<T> action, int numPerRow, int btnWidth = 175)
        {
            List<T> filtered = objects.FindAll(x => textSelector(x).ToLower().Contains(search.ToLower()));

            int rows = Mathf.CeilToInt(filtered.Count / (float)numPerRow);

            for (int i = 0; i < rows; i++)
            {
                GUILayout.BeginHorizontal();
                for (int j = 0; j < numPerRow; j++)
                {
                    int index = i * numPerRow + j;
                    if (index >= filtered.Count) break;
                    var obj = filtered[index];

                    if (GUILayout.Button(textSelector((T)obj), GUILayout.Width(btnWidth))) action((T)obj);
                }
                GUILayout.EndHorizontal();
            }
        }
    }
}