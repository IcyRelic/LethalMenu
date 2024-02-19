using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LethalMenu.Cheats
{
    internal class FOV : Cheat
    {

        public override void Update()
        {
            LethalMenu.localPlayer.gameplayCamera.fieldOfView = Hack.FOV.IsEnabled() ? Settings.f_fov : Settings.f_defaultFOV;
        }

    }
}
