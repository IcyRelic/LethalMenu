using System;
using System.Collections.Generic;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats;

internal class Breadcrumbs : Cheat
{
    private readonly List<Vector3> crumbs = [];
    private long last;

    public override void Update()
    {
        if ((!(bool)StartOfRound.Instance || !StartOfRound.Instance.shipHasLanded) && crumbs.Count > 0) crumbs.Clear();

        if (!Hack.Breadcrumbs.IsEnabled()) return;

        var player = GameNetworkManager.Instance.localPlayerController;

        var currentTimeMillis = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        if (!(currentTimeMillis - last >= Settings.f_breadcrumbInterval * 1000L) || player.isPlayerDead) return;

        last = currentTimeMillis;
        var pos = player.transform.position;
        pos.y -= 0.5f;

        crumbs.Add(pos);
    }

    public override void OnGui()
    {
        if (!Hack.Breadcrumbs.IsEnabled()) return;


        foreach (var crumb in crumbs)
        {
            Vector3 screen;
            if (!WorldToScreen(crumb, out screen)) continue;

            VisualUtil.DrawString(new Vector2(screen.x, screen.y), crumbs.IndexOf(crumb).ToString(), true, true);
        }
    }
}