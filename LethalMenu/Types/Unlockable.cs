namespace LethalMenu.Types;

public enum Unlockable
{
    /**
     * USELESS
     * OrangeSuit = 0,
     * Cupboard = 7,
     * FileCabinet = 8,
     * LightSwitch = 11,
     * Bunkbeds = 15,
     * Terminal = 16,
     * *
     */
    GreenSuit = 1,
    HazardSuit = 2,
    PajamaSuit = 3,
    CozyLights = 4,
    Teleporter = 5,
    Television = 6,
    Toilet = 9,
    Shower = 10,
    RecordPlayer = 12,
    Table = 13,
    RomanticTable = 14,
    SignalTranslator = 17,
    LoudHorn = 18,
    InverseYeleporter = 19,
    JackOLantern = 20,
    WelcomeMat = 21,
    Goldfish = 22,
    PlushiePajamaMan = 23
}

public static class UnlockableExtensions
{
    public static UnlockableItem GetItem(this Unlockable unlockable)
    {
        return StartOfRound.Instance.unlockablesList.unlockables[(int)unlockable];
    }

    public static void SetLocked(this Unlockable unlockable)
    {
        var item = unlockable.GetItem();
        item.alreadyUnlocked = false;
        item.hasBeenUnlockedByPlayer = false;
    }

    public static void Buy(this Unlockable unlockable, int credits)
    {
        var item = unlockable.GetItem();

        unlockable.SetLocked();

        StartOfRound.Instance.BuyShipUnlockableServerRpc((int)unlockable, credits);
    }
}