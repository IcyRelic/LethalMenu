using GameNetcodeStuff;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace LethalMenu.Cheats
{

    internal class Breadcrumbs : Cheat
    {
        private readonly List<Vector3> crumbs = new List<Vector3>();
        private long lastDropTime;

        public override void OnGui()
        {
            if (!Hack.Breadcrumbs.IsEnabled()) return;
            for (int i = 0; i < crumbs.Count; i++)
            {
                if (!WorldToScreen(crumbs[i], out Vector3 screen)) continue;
                VisualUtil.DrawString(new Vector2(screen.x, screen.y), i.ToString(), true, true);
            }
        }

        public override void Update()
        {
            if (!StartOfRound.Instance.shipHasLanded && crumbs.Count > 0)
            {
                crumbs.Clear();
                return;
            }
            if (!Hack.Breadcrumbs.IsEnabled()) return;
            PlayerControllerB? localPlayer = LethalMenu.localPlayer;
            if (localPlayer == null || localPlayer.isPlayerDead) return;
            long timeNow = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            if (timeNow - lastDropTime < Settings.f_breadcrumbInterval * 1000) return;
            lastDropTime = timeNow;
            Vector3 position = localPlayer.transform.position;
            position.y -= 0.5f;
            crumbs.Add(position);
        }
    }
}
