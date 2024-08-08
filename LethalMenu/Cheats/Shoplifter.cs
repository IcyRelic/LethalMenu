using HarmonyLib;

namespace LethalMenu.Cheats
{
    [HarmonyPatch(typeof(Terminal), "Update")]
    internal class Shoplifter : Cheat
    {
        public static float[] originalCreditsWorths;
        [HarmonyPrefix]
        public static void Prefix(ref Item[] ___buyableItemsList)
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
