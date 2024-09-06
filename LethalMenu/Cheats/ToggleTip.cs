using System.Linq;
using LethalMenu.Menu.Popup;
using TMPro;

namespace LethalMenu.Cheats
{
    internal class ToggleTip : Cheat
    {
        public override void Update()
        {
            if (HUDManager.Instance == null || !Hack.ToggleTip.IsEnabled()) return;
            string ToggleKey = $"Toggle Menu : {FirstSetupManagerWindow.kb}";
            TextMeshProUGUI ToggleTipLine = HUDManager.Instance.controlTipLines.FirstOrDefault(l => l.text.Contains(ToggleKey));
            if (ToggleTipLine != null || !Hack.ToggleTip.IsEnabled()) ToggleTipLine.text = "";
            TextMeshProUGUI FirstEmptyLine = HUDManager.Instance.controlTipLines.FirstOrDefault(l => string.IsNullOrEmpty(l.text));
            if (FirstEmptyLine != null) FirstEmptyLine.text = ToggleKey;
        }
    }
}