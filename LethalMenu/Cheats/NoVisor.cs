using UnityEngine;

namespace LethalMenu.Cheats;

internal class NoVisor : Cheat
{
    public override void Update()
    {
        RemoveVisor();
    }

    private static void RemoveVisor()
    {
        var localVisor = GameObject.Find("Systems/Rendering/PlayerHUDHelmetModel/");

        localVisor?.SetActive(!Hack.NoVisor.IsEnabled());
    }
}