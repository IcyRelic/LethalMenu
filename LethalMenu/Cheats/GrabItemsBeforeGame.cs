using System.Linq;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class GrabItemsBeforeGame : Cheat
    {
        public override void Update()
        {
            if (Hack.GrabItemsBeforeGame.IsEnabled())
            {
                LethalMenu.items.Where(i => i != null && !i.itemProperties.canBeGrabbedBeforeGameStart).ToList().ForEach(i => i.itemProperties.canBeGrabbedBeforeGameStart = true);
            }
        }
    }
}
