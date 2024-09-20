using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class OpenDropShipLand : Cheat
    {
        public override void Update()
        {
            ItemDropship dropship = Object.FindAnyObjectByType<ItemDropship>();
            if (!Hack.OpenDropShipLand.IsEnabled() || dropship == null || !dropship.shipLanded) return;
            dropship.OpenShipServerRpc();
        }
    }
}
