namespace LethalMenu.Cheats
{
    internal class EggsAlwaysExplode : Cheat
    {
        public override void Update()
        {
            if (Hack.EggsAlwaysExplode.IsEnabled() && !Hack.EggsNeverExplode.IsEnabled() && LethalMenu.localPlayer != null && LethalMenu.localPlayer?.currentlyHeldObjectServer is StunGrenadeItem egg && egg != null && egg.explodeSFX.name == "EasterEggPop") egg.SetExplodeOnThrowClientRpc(true);
        }
    }
}
