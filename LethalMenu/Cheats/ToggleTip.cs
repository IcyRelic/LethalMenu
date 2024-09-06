using System.Linq;
using LethalMenu.Menu.Popup;
using TMPro;

namespace LethalMenu.Cheats
{
    internal class ToggleTip : Cheat
    {
        public override void Update()
        {
            if (HUDManager.Instance == null) return;
            string ToggleKey = $"Toggle Menu : {FirstSetupManagerWindow.kb}";
            TextMeshProUGUI ToggleTipLine = HUDManager.Instance.controlTipLines.FirstOrDefault(l => l.text.Contains(ToggleKey));
            if (!Hack.ToggleTip.IsEnabled())
            {
                if (ToggleTipLine != null) ToggleTipLine.text = ""; 
                return;
            }
            if (ToggleTipLine != null) ToggleTipLine.text = "";
            TextMeshProUGUI FirstEmptyLine = HUDManager.Instance.controlTipLines.FirstOrDefault(l => string.IsNullOrEmpty(l.text));
            if (FirstEmptyLine != null) FirstEmptyLine.text = ToggleKey;
        }
    }
}
