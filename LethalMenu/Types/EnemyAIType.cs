using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LethalMenu.Types
{
    public enum EnemyAIType
    {
        Unknown,
        Centipede,
        SandSpider,
        HoarderBug,
        Flowerman,
        Crawler,
        Blob,
        DressGirl,
        Puffer,
        Nutcracker,
        RedLocustBees,
        Doublewing,
        DocileLocustBees,
        MouthDog,
        ForestGiant,
        SandWorm,
        BaboonHawk,
        SpringMan,
        Jester,
        LassoMan,
        MaskedPlayerEnemy,
        Butler,
        ButlerBees,
        RadMech,
        FlowerSnake,
    }

    public static class EnemyAITypeExtensions
    {
        public static readonly Dictionary<EnemyAIType, bool> EnemyFilter = new Dictionary<EnemyAIType, bool>()
        {
            { EnemyAIType.Unknown, true },
            { EnemyAIType.Centipede, true },
            { EnemyAIType.SandSpider, true },
            { EnemyAIType.HoarderBug, true },
            { EnemyAIType.Flowerman, true },
            { EnemyAIType.Crawler, true },
            { EnemyAIType.Blob, true },
            { EnemyAIType.DressGirl, true },
            { EnemyAIType.Puffer, true },
            { EnemyAIType.Nutcracker, true },
            { EnemyAIType.RedLocustBees, true },
            { EnemyAIType.Doublewing, false },
            { EnemyAIType.DocileLocustBees, false },
            { EnemyAIType.MouthDog, true },
            { EnemyAIType.ForestGiant, true },
            { EnemyAIType.SandWorm, true },
            { EnemyAIType.BaboonHawk, true },
            { EnemyAIType.SpringMan, true },
            { EnemyAIType.Jester, true },
            { EnemyAIType.LassoMan, true },
            { EnemyAIType.MaskedPlayerEnemy, true },
            { EnemyAIType.Butler, true },
            { EnemyAIType.ButlerBees, true },
            { EnemyAIType.RadMech, true },
            { EnemyAIType.FlowerSnake, true }
        };

        public static void ToggleESP(this EnemyAIType type)
        {
            EnemyFilter[type] = !EnemyFilter[type];
        }

        public static bool IsESPEnabled(this EnemyAIType type)
        {
            return EnemyFilter[type];
        }

        public static EnemyAIType GetEnemyAIType(this EnemyAI enemy)
        {
            bool hasType = Enum.TryParse<EnemyAIType>(enemy.enemyType.name, out EnemyAIType type);

            if (hasType) return type;

            return EnemyAIType.Unknown;
        }
    }
}