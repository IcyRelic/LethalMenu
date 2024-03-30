using System.Globalization;
using LethalMenu.Language;
using Newtonsoft.Json;
using UnityEngine;

namespace LethalMenu.Util;

public class RgbaColor
{
    public float R, G, B, A;


    [JsonConstructor]
    public RgbaColor(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }


    public RgbaColor(int r, int g, int b, float a)
    {
        R = r / 255f;
        G = g / 255f;
        B = b / 255f;
        A = a;
    }

    public RgbaColor(string hexCode, float alpha = 1f)
    {
        try
        {
            if (hexCode.StartsWith("#")) hexCode = hexCode.Substring(1);

            var rgb = int.Parse(hexCode, NumberStyles.HexNumber);

            R = hexCode.Length == 8 ? ((rgb >> 24) & 0xFF) / 255f : ((rgb >> 16) & 0xFF) / 255f;
            G = hexCode.Length == 8 ? ((rgb >> 16) & 0xFF) / 255f : ((rgb >> 8) & 0xFF) / 255f;
            B = hexCode.Length == 8 ? ((rgb >> 8) & 0xFF) / 255f : (rgb & 0xFF) / 255f;
            A = hexCode.Length == 8 ? (rgb & 0xFF) / 255f : alpha;
        }
        catch
        {
            R = 1f;
            G = 1f;
            B = 1f;
            A = 1f;
        }
    }

    public Color GetColor()
    {
        return new Color(R, G, B, A);
    }

    public string GetHexCode()
    {
        if (A < 1f) return GetHexCodeAlpha();

        var red = Mathf.Clamp((int)(R * 255), 0, 255);
        var green = Mathf.Clamp((int)(G * 255), 0, 255);
        var blue = Mathf.Clamp((int)(B * 255), 0, 255);

        var rgb = (red << 16) | (green << 8) | blue;

        var hexCode = rgb.ToString("X6");

        return hexCode;
    }

    public string GetHexCodeAlpha()
    {
        var red = Mathf.Clamp((int)(R * 255), 0, 255);
        var green = Mathf.Clamp((int)(G * 255), 0, 255);
        var blue = Mathf.Clamp((int)(B * 255), 0, 255);
        var alpha = Mathf.Clamp((int)(A * 255), 0, 255);

        var rgb = (red << 24) | (green << 16) | (blue << 8) | alpha;

        var hexCode = rgb.ToString("X8");

        return hexCode;
    }

    public string AsString(string text)
    {
        return $"<color=#{GetHexCode()}>{Localization.Localize(text)}</color>";
    }
}

public static class VisualUtil
{
    private static Texture2D _lineTex;
    private static GUIStyle StringStyle { get; } = new(GUI.skin.label);

    public static Color Color
    {
        set => GUI.color = value;
    }

    public static void DrawBox(Vector2 position, Vector2 size, Color color, bool centered = true)
    {
        Color = color;
        DrawBox(position, size, centered);
    }

    public static void DrawBox(Vector2 position, Vector2 size, RgbaColor color, bool centered = true)
    {
        DrawBox(position, size, color.GetColor(), centered);
    }

    public static void DrawBox(Vector2 position, Vector2 size, bool centered = true)
    {
        var upperLeft = centered ? position - size / 2f : position;
        GUI.DrawTexture(new Rect(position, size), Texture2D.whiteTexture, ScaleMode.StretchToFill);
    }

    public static void DrawString(Vector2 position, string label, Color color, bool centered = true,
        bool alignMiddle = false, bool bold = false, int fontSize = -1)
    {
        var color2 = GUI.color;
        Color = color;
        DrawString(position, label, centered, alignMiddle, bold, fontSize);
        GUI.color = color2;
    }

    public static void DrawDistanceString(Vector2 position, string label, RgbaColor color, float distance,
        bool showDistance = true)
    {
        if (showDistance) label += "\n" + distance + "m";
        DrawString(position, label, color.GetColor(), true, true);
    }

    public static void DrawString(Vector2 position, string label, RgbaColor color, bool centered = true,
        bool alignMiddle = false, bool bold = false, int fontSize = -1)
    {
        DrawString(position, label, color.GetColor(), centered, alignMiddle, bold, fontSize);
    }

    public static void DrawString(Vector2 position, string label, bool centered = true, bool alignMiddle = false,
        bool bold = false, int fontSize = -1)
    {
        var content = new GUIContent(label);

        StringStyle.fontSize = fontSize < 0 ? Settings.i_menuFontSize : fontSize;
        StringStyle.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;


        var size = StringStyle.CalcSize(content);
        var upperLeft = centered ? position - size / 2f : position;
        var style = new GUIStyle(GUI.skin.label);


        if (alignMiddle) style.alignment = TextAnchor.MiddleCenter;
        else style.alignment = TextAnchor.MiddleLeft;

        if (bold) style.fontStyle = FontStyle.Bold;

        if (fontSize > 0) style.fontSize = fontSize;

        GUI.Label(new Rect(upperLeft, size), content, style);
    }

    public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
    {
        var matrix = GUI.matrix;
        if (!_lineTex)
            _lineTex = new Texture2D(1, 1);

        var color2 = GUI.color;
        GUI.color = color;
        var num = Vector3.Angle(pointB - pointA, Vector2.right);

        if (pointA.y > pointB.y)
            num = -num;

        GUIUtility.ScaleAroundPivot(new Vector2((pointB - pointA).magnitude, width),
            new Vector2(pointA.x, pointA.y + 0.5f));
        GUIUtility.RotateAroundPivot(num, pointA);
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, 1f, 1f), _lineTex);
        GUI.matrix = matrix;
        GUI.color = color2;
    }

    public static void DrawLine(Vector2 pointA, Vector2 pointB, RgbaColor color, float width)
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

    public static void DrawBox(float x, float y, float w, float h, RgbaColor color, float thickness)
    {
        DrawBox(x, y, w, h, color.GetColor(), thickness);
    }

    public static void DrawBoxOutline(Vector2 point, float width, float height, Color color, float thickness)
    {
        DrawLine(point, new Vector2(point.x + width, point.y), color, thickness);
        DrawLine(point, new Vector2(point.x, point.y + height), color, thickness);
        DrawLine(new Vector2(point.x + width, point.y + height), new Vector2(point.x + width, point.y), color,
            thickness);
        DrawLine(new Vector2(point.x + width, point.y + height), new Vector2(point.x, point.y + height), color,
            thickness);
    }

    public static void DrawBoxOutline(Vector2 point, float width, float height, RgbaColor color, float thickness)
    {
        DrawBoxOutline(point, width, height, color.GetColor(), thickness);
    }
}