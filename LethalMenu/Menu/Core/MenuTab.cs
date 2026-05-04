using LethalMenu.Language;

namespace LethalMenu.Menu.Core
{

    internal class MenuTab : MenuFragment
    {
        public string? name;
        public MenuTab(string name)
        {
            this.name = Localization.Localize(name);
        }

        public virtual void Draw() { }

    }
}
