namespace LethalMenu.Cheats;

internal class NeverLoseScrap : Cheat
{
    public override void Update()
    {
        KeepItems();
    }

    public void KeepItems()
    {
        if (Hack.NeverLoseScrap.IsEnabled() && StartOfRound.Instance.allPlayersDead)
            StartOfRound.Instance.allPlayersDead = false;
    }
}