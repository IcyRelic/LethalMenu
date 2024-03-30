namespace LethalMenu.Cheats;

internal class UnlimitedBattery : Cheat
{
    public override void Update()
    {
        if (!Hack.UnlimitedBattery.IsEnabled()) return;


        if (!LethalMenu.LocalPlayer) return;
        foreach (var item in LethalMenu.LocalPlayer.ItemSlots)
        {
            if (!item || !item.itemProperties.requiresBattery) continue;

            item.insertedBattery.charge = 1f;
            item.SyncBatteryServerRpc(100);
        }
    }
}