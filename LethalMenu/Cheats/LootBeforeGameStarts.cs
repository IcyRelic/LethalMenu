using System.Collections.Generic;
using System.Linq;

namespace LethalMenu.Cheats
{
    internal class LootBeforeGameStarts : Cheat
    {
        private readonly List<GrabbableObject> citem = new List<GrabbableObject>();

        public override void Update()
        {
            if (!Hack.LootBeforeGameStarts.IsEnabled())
            {
                if (citem.Any())
                {
                    citem.ForEach(i => i.itemProperties.canBeGrabbedBeforeGameStart = false);
                    citem.Clear();
                }
                return;
            }
            LethalMenu.items.Where(i => i != null && !i.itemProperties.canBeGrabbedBeforeGameStart).ToList().ForEach(i =>
            {
                citem.Add(i);
                i.itemProperties.canBeGrabbedBeforeGameStart = true;
            });
        }
    }
}
