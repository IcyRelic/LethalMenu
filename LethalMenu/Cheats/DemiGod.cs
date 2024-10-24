using GameNetcodeStuff;
using LethalMenu.Handler;
using System.Collections.Generic;
using System.Linq;

namespace LethalMenu.Cheats
{ 
    internal class DemiGodCheat : Cheat
    {
        public static readonly List<PlayerControllerB> DemiGodPlayers = new List<PlayerControllerB>();

        public override void Update()
        {
            if (!Hack.DemiGod.IsEnabled()) return;
            DemiGodPlayers.Where(player => player != null && !player.isPlayerDead && player.health != 100).ToList().ForEach(p => p.Handle().Heal());
        }

        public static void ToggleDemiGod(PlayerControllerB player)
        {
            if (!DemiGodPlayers.Contains(player)) DemiGodPlayers.Add(player);
            else DemiGodPlayers.Remove(player);
        }
    }
}
