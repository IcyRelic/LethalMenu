namespace LethalMenu.Cheats
{
    internal class KeepShipItems : Cheat
    {
        public override void Update()
        {
            KeepItems();
        }

        public void KeepItems()
        {
            if (Hack.KeepShipItems.IsEnabled() && StartOfRound.Instance.allPlayersDead)
            {
                StartOfRound.Instance.allPlayersDead = false;
            }
        }
    }
}
