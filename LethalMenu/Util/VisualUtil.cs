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
    public class VisualUtil
    {
        public static GUIStyle StringStyle { get; set; } = new GUIStyle(GUI.skin.label);

        public static Color Color
        {
            get { return GUI.color; }
            set { GUI.color = value; }
        }

        public static void DrawBox(Vector2 position, Vector2 size, Color color, bool centered = true)
        {
            Color = color;
            DrawBox(position, size, centered);
        }

        public static void DrawBox(Vector2 position, Vector2 size, RGBAColor color, bool centered = true)
        {
            DrawBox(position, size, color.GetColor(), centered);
        }
        public static void DrawBox(Vector2 position, Vector2 size, bool centered = true)
        {
            var upperLeft = centered ? position - size / 2f : position;
            GUI.DrawTexture(new Rect(position, size), Texture2D.whiteTexture, ScaleMode.StretchToFill);
        }

        public static void DrawString(Vector2 position, string label, Color color, bool centered = true, bool alignMiddle = false, bool bold = false, int fontSize = -1)
        {
            Color color2 = GUI.color;
            Color = color;
            DrawString(position, label, centered, alignMiddle, bold, fontSize);
            GUI.color = color2;
        }

        public static void DrawDistanceString(Vector2 position, string label, RGBAColor color, float distance, bool showDistance = true)
        {
            if (showDistance) label += "\n" + distance.ToString() + "m";
            DrawString(position, label, color.GetColor(), true, true);
        }

        public static void DrawString(Vector2 position, string label, RGBAColor color, bool centered = true, bool alignMiddle = false, bool bold = false, int fontSize = -1)
        {
            DrawString(position, label, color.GetColor(), centered, alignMiddle, bold, fontSize);
        }

        public static void DrawString(Vector2 position, string label, bool centered = true, bool alignMiddle = false, bool bold = false, int fontSize = -1)
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

            GUI.Label(new Rect(upperLeft, size), content, style);
        }

        public static Texture2D lineTex;
        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            Matrix4x4 matrix = GUI.matrix;
            if (!lineTex)
                lineTex = new Texture2D(1, 1);

            Color color2 = GUI.color;
            GUI.color = color;
            float num = Vector3.Angle(pointB - pointA, Vector2.right);

            if (pointA.y > pointB.y)
                num = -num;

            GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width), new Vector2(pointA.x, pointA.y + 0.5f));
            GUIUtility.RotateAroundPivot(num, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), lineTex);
            GUI.matrix = matrix;
            GUI.color = color2;
        }

        public static void DrawLine(Vector2 pointA, Vector2 pointB, RGBAColor color, float width)
        {
            DrawLine(pointA, pointB, color.GetColor(), width);
        }

        public static void DrawBox(float x, float y, float w, float h, Color color, float thickness)
        {
            DrawLine(new Vector2(x, y), new Vector2(x + w, y), color, thickness);
            DrawLine(new Vector2(x, y), new Vector2(x, y + h), color, thickness);
            DrawLine(new Vector2(x + w, y), new Vector2(x + w, y + h), color, thickness);
            DrawLine(new Vector2(x, y + h), new Vector2(x + w, y + h), color, thickness);
        }

        public static void DrawBox(float x, float y, float w, float h, RGBAColor color, float thickness)
        {
            DrawBox(x, y, w, h, color.GetColor(), thickness);
        }

        public static void DrawBoxOutline(Vector2 Point, float width, float height, Color color, float thickness)
        {
            DrawLine(Point, new Vector2(Point.x + width, Point.y), color, thickness);
            DrawLine(Point, new Vector2(Point.x, Point.y + height), color, thickness);
            DrawLine(new Vector2(Point.x + width, Point.y + height), new Vector2(Point.x + width, Point.y), color, thickness);
            DrawLine(new Vector2(Point.x + width, Point.y + height), new Vector2(Point.x, Point.y + height), color, thickness);
        }

        public static void DrawBoxOutline(Vector2 Point, float width, float height, RGBAColor color, float thickness)
        {
            DrawBoxOutline(Point, width, height, color.GetColor(), thickness);
        }
    }
}
