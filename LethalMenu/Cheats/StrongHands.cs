using GameNetcodeStuff;

namespace LethalMenu.Cheats
{
    internal class StrongHands : Cheat
    {
        public override void Update()
        {
            PlayerControllerB? localPlayer = LethalMenu.localPlayer;
            if (localPlayer == null) return;
            GrabbableObject? heldObject = localPlayer.currentlyHeldObjectServer;
            if (heldObject == null) return;
            localPlayer.twoHanded = heldObject != null && !Hack.StrongHands.IsEnabled() && heldObject.itemProperties.twoHanded;
        }
    }
}
