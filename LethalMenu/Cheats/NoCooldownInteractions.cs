using UnityEngine;
using LethalMenu.Util;
using HarmonyLib;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Bson;
using System.Reflection.Emit;
using Mono.Cecil.Cil;
using System;
using System.Reflection;
using static UnityEngine.UI.Image;
using static IngamePlayerSettings;


namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class NoCooldownInteractions : Cheat
    {

        private static Dictionary<int, float> triggers = new Dictionary<int, float>();

        public NoCooldownInteractions()
        {
            LethalMenu.Instance.StartCoroutine(TimeToHold());
            LethalMenu.Instance.StartCoroutine(CleanUpTriggers());
        }

        public override void Update()
        {
            LethalMenu.debugMessage = FindObjectOfType<StartMatchLever>().triggerScript.timeToHold + "";

            if (Hack.NoCooldown.IsEnabled())
                LethalMenu.teleporters.FindAll(t => t != null).ForEach(t =>
                {
                    t.Reflect().SetValue("cooldownTime", 0.0f);
                    t.buttonTrigger.enabled = true;

                });
        }

        

        private IEnumerator TimeToHold()
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);
                LethalMenu.interactTriggers.FindAll(t => t != null).ForEach(t =>
                {
                    

                    if (!triggers.ContainsKey(t.GetInstanceID()))
                    {
                        triggers.Add(t.GetInstanceID(), t.timeToHold);
                        Debug.Log($"Added {t.name} to triggers with => {t.timeToHold}");
                    }

                    float original = triggers.TryGetValue(t.GetInstanceID(), out float value) ? value : 0.5f;

                    float timeToHold = Hack.InstantInteract.IsEnabled() ? t.name.StartsWith("EntranceTeleport") ? 0.3f : 0.0f : original;
                    
                    t.timeToHold = timeToHold;
                });
            }
        }

        private IEnumerator CleanUpTriggers()
        {
            while (true)
            {
                yield return new WaitForSeconds(15f);

                List<int> keep = new List<int>();

                LethalMenu.interactTriggers.ForEach(t => keep.Add(t.GetInstanceID()));
                triggers.Keys.ToList().FindAll(k => !keep.Contains(k)).ForEach(k => triggers.Remove(k));
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GrabbableObject), "RequireCooldown")]
        public static bool RequireCD(ref bool __result)
        {
            if(!Hack.NoCooldown.IsEnabled()) return true;

            __result = false;
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(InteractTrigger), "Interact")]
        public static bool Interact(InteractTrigger __instance)
        {
            bool isDoorLock = __instance.GetComponentInParent<DoorLock>() != null;

            if (isDoorLock)
            {
                __instance.interactCooldown = true;
                DoorLock door = __instance.GetComponentInParent<DoorLock>();
                bool isOpen = (bool) door.Reflect().GetValue("isDoorOpened");
                if (isOpen) __instance.timeToHold = 0.0f;
                else __instance.timeToHold = Hack.InstantInteract.IsEnabled()? 0.0f : 0.3f;
            }
            else __instance.interactCooldown = !Hack.NoCooldown.IsEnabled();

            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StartMatchLever), nameof(StartMatchLever.BeginHoldingInteractOnLever))]
        public static void BeginHoldPostfix(StartMatchLever __instance)
        {
            if (TimeOfDay.Instance.daysUntilDeadline > 0) return;

            float normal = TimeOfDay.Instance.daysUntilDeadline > 0 ? 0.5f : 4f;


            if (triggers.TryGetValue(__instance.triggerScript.GetInstanceID(), out float holdTime) && holdTime != normal)
                triggers[__instance.triggerScript.GetInstanceID()] = normal;

            if (Hack.InstantInteract.IsEnabled())
                __instance.triggerScript.timeToHold = 0.0f;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(DoorLock), nameof(DoorLock.UnlockDoor))]
        public static void UnlockDoorPostfix(StartMatchLever __instance)
        {
            if (triggers.TryGetValue(__instance.triggerScript.GetInstanceID(), out float holdTime) && holdTime != 0.3f)
                triggers[__instance.triggerScript.GetInstanceID()] = 0.3f;

            if (Hack.InstantInteract.IsEnabled())
                __instance.triggerScript.timeToHold = 0.0f;
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Shovel), "reelUpShovel")]
        public static IEnumerator reelUpShovelPostfix(IEnumerator reelUpShovel)
        {
            while (reelUpShovel.MoveNext())
            {
                if (reelUpShovel.Current is WaitForSeconds && Hack.NoCooldown.IsEnabled()) continue;

                yield return reelUpShovel.Current;
            }
        }
    }
}
