using LethalMenu.Util;

namespace LethalMenu.Cheats
{
    internal class NoTZPEffects : Cheat
    {
        public override void Update()
        {
            if (Hack.NoTZPEffects.IsEnabled() && LethalMenu.localPlayer != null && HUDManager.Instance != null && LethalMenu.localPlayer != null && LethalMenu.localPlayer?.currentlyHeldObjectServer is TetraChemicalItem TZP && TZP != null)
            {
                LethalMenu.localPlayer.drunknessInertia = 0f;
                LethalMenu.localPlayer.increasingDrunknessThisFrame = false;
                HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", false);
                TZP?.Reflect().SetValue("emittingGas", false);
                LethalMenu.localPlayer.increasingDrunknessThisFrame = false;
            }
        }
    }
}
