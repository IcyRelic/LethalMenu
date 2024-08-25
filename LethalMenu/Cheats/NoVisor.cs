using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class NoVisor : Cheat
    {
        public override void Update()
        {
            GameObject.Find("Systems/Rendering/PlayerHUDHelmetModel/").SetActive(!Hack.NoVisor.IsEnabled());
        }
    }
}
