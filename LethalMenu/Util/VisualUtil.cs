using LethalMenu.Handler;
using LethalMenu.Language;
using Newtonsoft.Json;
using UnityEngine;

namespace LethalMenu.Util
{
    public class RGBAColor
    {
        public float r, g, b, a;


        [JsonConstructor]
        public RGBAColor(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        
        public RGBAColor(int r, int g, int b, float a)
        {
            this.r = r / 255f;
            this.g = g / 255f;
            this.b = b / 255f;
            this.a = a;
        }

        public RGBAColor(string hexCode, float alpha = 1f)
        {
            try
            {
                if (hexCode.StartsWith("#")) hexCode = hexCode.Substring(1);

                int rgb = int.Parse(hexCode, System.Globalization.NumberStyles.HexNumber);

                r = hexCode.Length == 8 ? ((rgb >> 24) & 0xFF) / 255f : ((rgb >> 16) & 0xFF) / 255f;
                g = hexCode.Length == 8 ? ((rgb >> 16) & 0xFF) / 255f : ((rgb >> 8) & 0xFF) / 255f;
                b = hexCode.Length == 8 ? ((rgb >> 8) & 0xFF) / 255f : (rgb & 0xFF) / 255f;
                a = hexCode.Length == 8 ? (rgb & 0xFF) / 255f : alpha;
            }
            catch
            {
                r = 1f;
                g = 1f;
                b = 1f;
                a = 1f;
            }
        }

        public Color GetColor()
        {
            return new Color(r, g, b, a);
        }

        public string GetHexCode()
        {
            if(a < 1f) return GetHexCodeAlpha();

            int red = Mathf.Clamp((int)(r * 255), 0, 255);
            int green = Mathf.Clamp((int)(g * 255), 0, 255);
            int blue = Mathf.Clamp((int)(b * 255), 0, 255);

            int rgb = (red << 16) | (green << 8) | blue;

            string hexCode = rgb.ToString("X6");

            return hexCode;
        }

        public string GetHexCodeAlpha()
        {
            int red = Mathf.Clamp((int)(r * 255), 0, 255);
            int green = Mathf.Clamp((int)(g * 255), 0, 255);
            int blue = Mathf.Clamp((int)(b * 255), 0, 255);
            int alpha = Mathf.Clamp((int)(a * 255), 0, 255);

            int rgb = (red << 24) | (green << 16) | (blue << 8) | alpha;

            string hexCode = rgb.ToString("X8");

            return hexCode;
        }

        public string AsString(string text)
        {
            return $"<color=#{GetHexCode()}>{Localization.Localize(text)}</color>";
        }

    }
    public class VisualUtil : Cheat
    {
        public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);

        public static Color Color
        {
            get { return GUI.color; }
            set { GUI.color = value; }
        }

        public static void DrawString(Vector2 position, string label, Color color, bool centered = true, bool alignMiddle = false, bool bold = false, bool forceOnScreen = false, int fontSize = -1)
        {
            Color color2 = GUI.color;
            Color = color;
            DrawString(position, label, centered, alignMiddle, bold, forceOnScreen, fontSize);
            GUI.color = color2;
        }

        public static void DrawDistanceString(Vector2 position, string label, RGBAColor color, float distance, bool showDistance = true)
        {
            if (showDistance) label += "\n" + distance.ToString() + "m";
            DrawString(position, label, color.GetColor(), true, true);
        }

        public static void DrawString(Vector2 position, string label, RGBAColor color, bool centered = true, bool alignMiddle = false, bool bold = false, bool forceOnScreen = false, int fontSize = -1)
        {
            DrawString(position, label, color.GetColor(), centered, alignMiddle, bold, forceOnScreen, fontSize);
        }

        public static void DrawString(Vector2 position, string label, bool centered = true, bool alignMiddle = false, bool bold = false, bool forceOnScreen = false, int fontSize = -1)
        {
            var content = new GUIContent(label);

            StringStyle.fontSize = fontSize < 0 ? Settings.i_menuFontSize : fontSize;
            StringStyle.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
            

            var size = StringStyle.CalcSize(content);
            var upperLeft = centered ? position - size / 2f : position;
            var style = new GUIStyle(GUI.skin.label);



            if (alignMiddle) style.alignment = TextAnchor.MiddleCenter;
            else style.alignment = TextAnchor.MiddleLeft;

            if(bold) style.fontStyle = FontStyle.Bold;

            if(fontSize > 0) style.fontSize = fontSize;

            Rect pos = new Rect(upperLeft, size);

            if(forceOnScreen)
            {
                if (pos.x < 0) pos.x = 10;
                if (pos.y < 0) pos.y = 10;
                if (pos.x + pos.width > Screen.width) pos.x = Screen.width - pos.width - 10;
                if (pos.y + pos.height > Screen.height) pos.y = Screen.height - pos.height - 10;
            }

            GUI.Label(pos, content, style);
        }

        public static Texture2D lineTex;
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            Matrix4x4 matrix = GUI.matrix;
            if (!lineTex) lineTex = new Texture2D(1, 1);
            Color color2 = GUI.color;
            GUI.color = color;
            float num = Vector3.Angle(pointB - pointA, Vector2.right);
            if (pointA.y > pointB.y) num = -num;
            Vector2 scale = new Vector2((pointB - pointA).magnitude, width);
            if (scale.x == 0 || scale.y == 0) return;
            GUIUtility.ScaleAroundPivot(scale, new Vector2(pointA.x, pointA.y + 0.5f));
            GUIUtility.RotateAroundPivot(num, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), lineTex);
            GUI.matrix = matrix;
            GUI.color = color2;
        }

        public static void DrawLine(Vector2 pointA, Vector2 pointB, RGBAColor color, float width)
        {
            DrawLine(pointA, pointB, color.GetColor(), width);
        }

        public static void DrawBoxOutline(GameObject GameObject, Color color, float thickness)
        {
            Bounds bounds = GameObject.GetBounds();
            Vector3[] corners = new Vector3[8];
            corners[0] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
            corners[1] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
            corners[2] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
            corners[3] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
            corners[4] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
            corners[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
            corners[6] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
            corners[7] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
            if (WorldToScreen(corners[0], out var topL) && WorldToScreen(corners[1], out var topR) && WorldToScreen(corners[2], out var topRN) && WorldToScreen(corners[3], out var topLN) && WorldToScreen(corners[4], out var bottomLF) && WorldToScreen(corners[5], out var bottomRF) && WorldToScreen(corners[6], out var bottomRN) && WorldToScreen(corners[7], out var bottomLN))
            {
                DrawLine(topL, topR, color, thickness);
                DrawLine(topR, topRN, color, thickness);
                DrawLine(topRN, topLN, color, thickness);
                DrawLine(topLN, topL, color, thickness);
                DrawLine(bottomLF, bottomRF, color, thickness);
                DrawLine(bottomRF, bottomRN, color, thickness);
                DrawLine(bottomRN, bottomLN, color, thickness);
                DrawLine(bottomLN, bottomLF, color, thickness);
                DrawLine(topL, bottomLF, color, thickness);
                DrawLine(topR, bottomRF, color, thickness);
                DrawLine(topRN, bottomRN, color, thickness);
                DrawLine(topLN, bottomLN, color, thickness);
            }
        }

        public static void DrawBoxOutline(GameObject GameObject, RGBAColor color, float thickness)
        {
            DrawBoxOutline(GameObject, color.GetColor(), thickness);
        }

        public static void DrawDot(Vector2 position, Color color, float radius)
        {
            Color Color = GUI.color;
            Vector2 center = position;
            for (float angle = 0; angle < 360; angle += 1)
            {
                float x = center.x + Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                float y = center.y + Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                Vector2 pointOnCircle = new Vector2(x, y);
                GUI.DrawTexture(new Rect(pointOnCircle.x, pointOnCircle.y, 1f, 1f), Texture2D.whiteTexture);
            }
            GUI.color = Color;
        }

        public static void DrawDot(Vector2 position, RGBAColor color, float radius)
        {
            DrawDot(position, color.GetColor(), radius);
        }
    }
}
