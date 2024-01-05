using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class NoFog : Cheat
    {

        public override void Update()
        {
            GameObject gameObject = GameObject.Find("Systems");

            if (gameObject == null) return;

            gameObject.transform.Find("Rendering").Find("VolumeMain").gameObject.SetActive(!Hack.NoFog.IsEnabled());
        }

    }
}
