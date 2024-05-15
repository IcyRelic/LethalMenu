using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class NoVisor : Cheat
    {
        public override void Update()
        {
            GameObject localVisor = GameObject.Find("Systems/Rendering/PlayerHUDHelmetModel/");

            if (Hack.NoVisor.IsEnabled())
            {
                localVisor?.SetActive(false);
            }
            else
            {
                localVisor?.SetActive(true);
            }
        }
    }
}
