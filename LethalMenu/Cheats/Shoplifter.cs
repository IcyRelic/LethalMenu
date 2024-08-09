using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LethalMenu.Cheats
{
    [HarmonyPatch]
    internal class Shoplifter : Cheat
    {
        public static Dictionary<string, int> nodeCosts = new Dictionary<string, int>();
        public static int[] itemCosts;
        public static int[] vehicleCosts;


        public override void Update()
        {
            Terminal term = Object.FindAnyObjectByType<Terminal>();
            TerminalKeyword buy = term.terminalNodes.allKeywords.ToList().Find(x => x.word == "buy");

            

            if(Hack.Shoplifter.IsEnabled())
            {
                buy.compatibleNouns.ToList().ForEach(n =>
                {
                    if (!nodeCosts.ContainsKey(n.noun.name)) nodeCosts.Add(n.noun.name, n.result.itemCost);

                    n.result.itemCost = 0;
                });

                if (itemCosts is null)
                {
                    itemCosts = new int[term.buyableItemsList.Length];
                    for (int i = 0; i < term.buyableItemsList.Length; i++)
                    {
                        itemCosts[i] = term.buyableItemsList[i].creditsWorth;
                    }
                }

                if (vehicleCosts is null)
                {
                    vehicleCosts = new int[term.buyableVehicles.Length];
                    for (int i = 0; i < term.buyableVehicles.Length; i++)
                    {
                        vehicleCosts[i] = term.buyableVehicles[i].creditsWorth;
                    }
                }

                term.buyableVehicles.ToList().ForEach(i => i.creditsWorth = 0);
                term.buyableItemsList.ToList().ForEach(i => i.creditsWorth = 0);
            }
            else if(nodeCosts.Count > 0)
            {
                buy.compatibleNouns.ToList().FindAll(n => nodeCosts.ContainsKey(n.noun.name)).ForEach(n => n.result.itemCost = nodeCosts[n.noun.name]);

                for (int i = 0; i < term.buyableItemsList.Length; i++)
                {
                    term.buyableItemsList[i].creditsWorth = itemCosts[i];
                }

                for (int i = 0; i < term.buyableVehicles.Length; i++)
                {
                    term.buyableVehicles[i].creditsWorth = vehicleCosts[i];
                }

                Clear();
            }

            
        }

        public static void Clear()
        {
            nodeCosts.Clear();
            itemCosts = null;
            vehicleCosts = null;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Terminal), "LoadNewNodeIfAffordable")]
        public static void LoadNode(TerminalNode node)
        {
            
            if (Hack.Shoplifter.IsEnabled())
            {
                Debug.Log($"Making Item Free {node.name}");
                node.itemCost = 0;
            }
        }

    }
        
}
