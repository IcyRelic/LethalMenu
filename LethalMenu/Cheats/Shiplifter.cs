using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch(typeof(Terminal))]
    internal class Shoplifter : Cheat
    {
        private static float[] originalCreditsWorths;

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static void CostPatch(ref Item[] ___buyableItemsList)
        {
            if (Hack.Shoplifter.IsEnabled())
            {
                if (___buyableItemsList == null)
                    return;

                if (originalCreditsWorths == null || originalCreditsWorths.Length != ___buyableItemsList.Length)
                {
                    originalCreditsWorths = new float[___buyableItemsList.Length];
                    for (int i = 0; i < ___buyableItemsList.Length; i++)
                    {
                        if (___buyableItemsList[i] != null)
                        {
                            originalCreditsWorths[i] = ___buyableItemsList[i].creditsWorth;
                        }
                    }
                }
                for (int index = 0; index < ___buyableItemsList.Length; ++index)
                {
                    if (___buyableItemsList[index] != null)
                    {
                        ___buyableItemsList[index].creditsWorth = 0;
                    }
                }
            }
            else
            {
                if (___buyableItemsList == null)
                    return;

                for (int index = 0; index < ___buyableItemsList.Length; ++index)
                {
                    if (___buyableItemsList[index] != null && originalCreditsWorths != null && originalCreditsWorths.Length > index)
                    {
                        ___buyableItemsList[index].creditsWorth = (int)originalCreditsWorths[index];
                    }
                }
                originalCreditsWorths = null;
            }
        }
    }
}
