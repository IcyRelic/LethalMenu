using LethalMenu.Menu.Core;
using UnityEngine;

namespace LethalMenu.Menu.Tab;

internal class GeneralTab : MenuTab
{
    private readonly Texture2D avatar;
    private Vector2 scrollPos;

    public GeneralTab() : base("General")
    {
        avatar = GetImage("https://icyrelic.com/img/Avatar2.jpg");
    }


    public override void Draw()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        MenuContent();
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    private void MenuContent()
    {
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        var intoText =
            "Thank you for using Lethal Menu.\n\nIf you have any suggestions please leave a comment on the forum post.\nAny bugs you find please provide some steps to recreate the issue and leave a comment.";

        //draw the avatar with the intoText on the right
        GUILayout.BeginHorizontal();
        GUILayout.Label(avatar, GUILayout.Width(100), GUILayout.Height(100));
        GUILayout.Label(intoText);
        GUILayout.EndHorizontal();


        GUILayout.Space(20);

        foreach (var line in Settings.Changelog.changes)
        {
            var style = new GUIStyle(GUI.skin.label);

            if (line.StartsWith("v")) style.fontStyle = FontStyle.Bold;
            GUILayout.Label(line.StartsWith("v") ? "Changelog " + line : line, style);
        }


        GUILayout.EndScrollView();
    }
}