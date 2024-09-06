using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LethalMenu.Cheats
{
    internal class PJSpammer : Cheat
    {
        
        private float lastTime = 0;


        public override void Update()
        {
            if (!Hack.PJSpammer.IsEnabled()) return;
            if (Time.time - lastTime < Settings.f_pjSpamSpeed) return;

            LethalMenu.animatedTriggers.FindAll(c => c.transform.parent is not null && c.transform.parent.gameObject.name.StartsWith("PlushiePJManContainer")).ForEach(c => c.TriggerAnimation(LethalMenu.localPlayer));
            lastTime = Time.time;            
        }
    }
}
