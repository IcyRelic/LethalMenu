using LethalMenu.Language;

namespace LethalMenu.Menu.Core;

internal class MenuTab : MenuFragment
{
    private readonly string _localization;
    public string Name;

    protected MenuTab(string name)
    {
        _localization = name;
        LocalizeName();
    }

    public void LocalizeName()
    {
        Name = Localization.Localize(_localization);
    }

    public virtual void Draw()
    {
    }
}