using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using LethalMenu.Util;
using UnityEngine;

namespace LethalMenu.Cheats;

[HarmonyPatch]
internal class NoCooldownInteractions : Cheat
{
    private static readonly Dictionary<int, float> Triggers = new();

    public NoCooldownInteractions()
    {
        LethalMenu.Instance.StartCoroutine(TimeToHold());
        LethalMenu.Instance.StartCoroutine(CleanUpTriggers());
    }

    public override void Update()
    {
        //LethalMenu.debugMessage = FindObjectOfType<StartMatchLever>().triggerScript.timeToHold + "";

        if (Hack.NoCooldown.IsEnabled())
            LethalMenu.Teleporters.FindAll(t => t).ForEach(t =>
            {
                t.Reflect().SetValue("cooldownTime", 0.0f);
                t.buttonTrigger.enabled = true;
            });
    }


    private static IEnumerator TimeToHold()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            LethalMenu.InteractTriggers.FindAll(t => t).ForEach(t =>
            {
                if (!Triggers.ContainsKey(t.GetInstanceID())) Triggers.Add(t.GetInstanceID(), t.timeToHold);

                var original = Triggers.GetValueOrDefault(t.GetInstanceID(), 0.5f);

                if (t.name.Equals("StartGameLever"))
                {
                    var leverHoldTime = TimeOfDay.Instance.daysUntilDeadline > 0
                        ? t.timeToHold = 0.7f
                        : t.timeToHold = 4f;

                    if (Mathf.Approximately(leverHoldTime, original))
                    {
                        Triggers[t.GetInstanceID()] = leverHoldTime;
                        original = leverHoldTime;
                    }
                }

                var timeToHold = Hack.InstantInteract.IsEnabled()
                    ? t.name.StartsWith("EntranceTeleport") ? 0.3f : 0.0f
                    : original;

                t.timeToHold = timeToHold;
            });
        }
    }

    private IEnumerator CleanUpTriggers()
    {
        while (true)
        {
            yield return new WaitForSeconds(15f);

            var keep = new List<int>();

            LethalMenu.InteractTriggers.ForEach(t => keep.Add(t.GetInstanceID()));
            Triggers.Keys.ToList().FindAll(k => !keep.Contains(k)).ForEach(k => Triggers.Remove(k));
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(GrabbableObject), "RequireCooldown")]
    public static bool RequireCd(ref bool __result)
    {
        if (!Hack.NoCooldown.IsEnabled()) return true;

        __result = false;
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(InteractTrigger), "Interact")]
    public static bool Interact(InteractTrigger __instance)
    {
        var isDoorLock = __instance.GetComponentInParent<DoorLock>() != null;

        if (isDoorLock)
        {
            __instance.interactCooldown = true;
            var door = __instance.GetComponentInParent<DoorLock>();
            var isOpen = door.Reflect().GetValue<bool>("isDoorOpened");
            if (isOpen) __instance.timeToHold = 0.0f;
            else __instance.timeToHold = Hack.InstantInteract.IsEnabled() ? 0.0f : 0.3f;
        }
        else
        {
            __instance.interactCooldown = !Hack.NoCooldown.IsEnabled();
        }

        return true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartMatchLever), nameof(StartMatchLever.BeginHoldingInteractOnLever))]
    public static void BeginHoldPostfix(StartMatchLever __instance)
    {
        if (TimeOfDay.Instance.daysUntilDeadline > 0) return;

        var normal = TimeOfDay.Instance.daysUntilDeadline > 0 ? 0.5f : 4f;


        if (Triggers.TryGetValue(__instance.triggerScript.GetInstanceID(), out var holdTime) &&
            Mathf.Approximately(holdTime, normal))
            Triggers[__instance.triggerScript.GetInstanceID()] = normal;

        if (Hack.InstantInteract.IsEnabled())
            __instance.triggerScript.timeToHold = 0.0f;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DoorLock), nameof(DoorLock.UnlockDoor))]
    public static void UnlockDoorPostfix(StartMatchLever __instance)
    {
        if (Triggers.TryGetValue(__instance.triggerScript.GetInstanceID(), out var holdTime) &&
            Mathf.Approximately(holdTime, 0.3f))
            Triggers[__instance.triggerScript.GetInstanceID()] = 0.3f;

        if (Hack.InstantInteract.IsEnabled())
            __instance.triggerScript.timeToHold = 0.0f;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Shovel), "reelUpShovel")]
    public static IEnumerator ReelUpShovelPostfix(IEnumerator reelUpShovel)
    {
        while (reelUpShovel.MoveNext())
        {
            if (reelUpShovel.Current is WaitForSeconds && Hack.NoCooldown.IsEnabled()) continue;

            yield return reelUpShovel.Current;
        }
    }
}