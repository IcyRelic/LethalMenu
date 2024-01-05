using GameNetcodeStuff;

namespace LethalMenu.Cheats
{
    internal class UnlimitedBattery : Cheat
    {
        public override void Update()
        {
            if (!Hack.UnlimitedBattery.IsEnabled()) return;


            if(LethalMenu.localPlayer == null) return;
            foreach (GrabbableObject item in LethalMenu.localPlayer.ItemSlots)
            {
                if (item == null || !item.itemProperties.requiresBattery) continue;

                item.insertedBattery.charge = 1f;
                item.SyncBatteryServerRpc(100);

               
            }
        }
    }
}
