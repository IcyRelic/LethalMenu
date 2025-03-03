using LethalMenu.Util;

namespace LethalMenu.Cheats
{
    internal class UnlimitedTZP : Cheat
    {
        public override void Update()
        {
            if (Hack.UnlimitedTZP.IsEnabled() && LethalMenu.localPlayer != null && LethalMenu.localPlayer?.currentlyHeldObjectServer is TetraChemicalItem TZP && TZP != null) TZP?.Reflect().SetValue("fuel", 1f);
        }
    }
}
