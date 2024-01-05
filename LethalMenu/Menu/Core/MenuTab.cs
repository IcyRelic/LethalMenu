namespace LethalMenu.Menu.Core
{

    internal class MenuTab : MenuFragment
    {
        public string name;

        public MenuTab(string name)
        {
            this.name = name;
        }

        public virtual void Draw() { }
    }
}
