using GameNetcodeStuff;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class Weight : Cheat
    {
        public override void Update()
        {
            PlayerControllerB? localPlayer = LethalMenu.localPlayer;
            if (localPlayer == null) return;
            localPlayer.carryWeight = Hack.Weight.IsEnabled() ? 1f : GetHeldWeight(localPlayer);
        }

        private static float GetHeldWeight(PlayerControllerB player)
        {
            float weight = 1f;
            if (player.ItemSlots == null) return weight;
            foreach (GrabbableObject grabbableObject in player.ItemSlots)
            {
                if (grabbableObject == null || grabbableObject.itemProperties == null) continue;
                weight += Mathf.Clamp(grabbableObject.itemProperties.weight - 1f, 0.0f, 100f);
            }
            return weight;
        }
    }
}
