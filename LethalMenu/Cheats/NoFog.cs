using UnityEngine;

namespace LethalMenu.Cheats;

internal class NoFog : Cheat
{
    public override void Update()
    {
        var systems = GameObject.Find("Systems");

        if (!systems) return;

        systems.transform.Find("Rendering").Find("VolumeMain").gameObject.SetActive(!Hack.NoFog.IsEnabled());
    }
}