using LethalMenu.Menu.Core;
using UnityEngine;

namespace LethalMenu.Menu.Tab
{
    internal class GeneralTab : MenuTab
    {
        Vector2 scrollPos;
        Texture2D avatar;

        public GeneralTab() : base("GeneralTab.Title")
        {
            GetImage("https://icyrelic.com/img/Avatar2.jpg", Avatar);
        }

        public void Avatar(Texture2D texture)
        {
            avatar = texture;
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

            GUILayout.BeginHorizontal();
            GUILayout.Label(avatar, GUILayout.Width(100), GUILayout.Height(100));
            GUILayout.Label("Thank you for using Lethal Menu.\n\nIf you have any suggestions please leave a comment on the forum post.\nAny bugs you find please provide some steps to recreate the issue and leave a comment.");
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            foreach (string line in Settings.Changelog.changes)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label);

                if (line.StartsWith("v")) style.fontStyle = FontStyle.Bold;
                GUILayout.Label(line.StartsWith("v") ? "Changelog " + line : line, style);
            }

            GUILayout.EndScrollView();
        }
    }
}
