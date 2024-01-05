using GameNetcodeStuff;
using LethalMenu.Util;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace LethalMenu.Cheats
{

    internal class Breadcrumbs : Cheat
    {
        private List<Vector3> crumbs = new List<Vector3>();
        private long last = 0;

        public override void OnGui()
        {
            if(!Hack.Breadcrumbs.IsEnabled()) return;


            foreach(Vector3 crumb in crumbs)
            {
                Vector3 screen;
                if (!WorldToScreen(crumb, out screen)) continue;
                
                VisualUtil.DrawString(new Vector2(screen.x, screen.y), crumbs.IndexOf(crumb).ToString(), true, true);
            }
            
        }

        public override void Update()
        {
            if ((!(bool)StartOfRound.Instance || !StartOfRound.Instance.shipHasLanded) && crumbs.Count > 0) crumbs.Clear();

            if (!Hack.Breadcrumbs.IsEnabled()) return;

            PlayerControllerB player = GameNetworkManager.Instance.localPlayerController;

            long currentTimeMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            if (currentTimeMillis - last >= (Settings.f_breadcrumbInterval * 1000L) && !player.isPlayerDead)
            {
                last = currentTimeMillis;
                Vector3 pos = player.transform.position;
                pos.y -= 0.5f;

                crumbs.Add(pos);
            }
        }
    }
}
