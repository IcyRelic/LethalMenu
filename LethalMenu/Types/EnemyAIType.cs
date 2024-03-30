using System;
using System.Collections.Generic;

namespace LethalMenu.Types;

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
    MaskedPlayerEnemy
}

public static class EnemyAITypeExtensions
{
    public static readonly Dictionary<EnemyAIType, bool> EnemyFilter = new()
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
        { EnemyAIType.MaskedPlayerEnemy, true }
    };

    public static void ToggleEsp(this EnemyAIType type)
    {
        EnemyFilter[type] = !EnemyFilter[type];
    }

    public static bool IsEspEnabled(this EnemyAIType type)
    {
        return EnemyFilter[type];
    }

    public static EnemyAIType GetEnemyAIType(this EnemyAI enemy)
    {
        var hasType = Enum.TryParse(enemy.enemyType.name, out EnemyAIType type);

        return hasType ? type : EnemyAIType.Unknown;
    }
}