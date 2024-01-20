using LethalMenu.Language;

namespace LethalMenu.Menu.Core
{

    internal class MenuTab : MenuFragment
    {
        public string name;
        private string localization;
        public MenuTab(string name)
        {
            this.localization = name;
            LocalizeName();
        }

        public void LocalizeName() => name = Localization.Localize(localization);
        public virtual void Draw() { }

    }
}
