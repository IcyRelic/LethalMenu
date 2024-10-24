using LethalMenu.Language;
using LethalMenu.Manager;
using LethalMenu.Menu.Core;
using LethalMenu.Types;
using LethalMenu.Util;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LethalMenu.Menu.Popup
{
    internal class NotificationWindow : PopupMenu
    {
        private static readonly Queue<(string title, string message)> QNotification = new Queue<(string title, string message)>();
        private static bool _isOpen = false;

        public NotificationWindow(int id) : base("NotificationWindow.Title", new Rect((Screen.width - 300f) / 2, (Screen.height - 150f) / 2, 300f, 150f), id) { }

        public override void DrawContent(int windowID)
        {
            GUILayout.BeginVertical();

            UI.Header(QNotification.Peek().title);

            GUILayout.Label(QNotification.Peek().message, GUILayout.ExpandWidth(true));

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(Localization.Localize("General.Close"), GUILayout.Width(30), GUILayout.ExpandWidth(true))) CloseNotification();

            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        public static void Notification(string _title, string msg)
        {
            QNotification.Enqueue((_title, msg));
            DisplayNotification();
        }

        private static void DisplayNotification()
        {
            if (!_isOpen && QNotification.Count > 0) Toggle();
        }

        private static void CloseNotification()
        {
            if (QNotification.Count <= 0) return;
            Toggle();
            QNotification.Dequeue();
            DisplayNotification();
        }

        private static void Toggle()
        {
            HackMenu.Instance.NotificationWindow.isOpen = !HackMenu.Instance.NotificationWindow.isOpen;
            _isOpen = !_isOpen;
        }
    }
}
