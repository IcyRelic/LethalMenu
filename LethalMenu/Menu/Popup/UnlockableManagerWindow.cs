using LethalMenu.Menu.Core;
using LethalMenu.Types;
using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LethalMenu.Menu.Popup
{
    internal class UnlockableManagerWindow : PopupMenu
    {
        private Vector2 scrollPos = Vector2.zero;

        public UnlockableManagerWindow(int id) : base("Unlockable Manager", new Rect(50f, 50f, 562f, 225f), id)
        {

        }

        public override void DrawContent(int windowID)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            if ((bool)StartOfRound.Instance)
            {
                var unlockables = Enum.GetValues(typeof(Unlockable)).Cast<Unlockable>().ToList();

                if (GUILayout.Button("Unlock All")) unlockables.ForEach(x => Hack.UnlockUnlockable.Execute(x));



                int itemsPerRow = 3;
                int rows = Mathf.CeilToInt(unlockables.Count / (float)itemsPerRow);

                for (int i = 0; i < rows; i++)
                {
                    GUILayout.BeginHorizontal();
                    for (int j = 0; j < itemsPerRow; j++)
                    {
                        int index = i * itemsPerRow + j;
                        if (index >= unlockables.Count) break;
                        var unlockable = unlockables[index];

                        if (GUILayout.Button(unlockable.GetItem().unlockableName, GUILayout.Width(175))) Hack.UnlockUnlockable.Execute(unlockable);
                    }
                    GUILayout.EndHorizontal();
                }

            }




            GUILayout.EndScrollView();
            GUI.DragWindow(new Rect(0.0f, 0.0f, 10000f, 45f));
        }


    }
}
