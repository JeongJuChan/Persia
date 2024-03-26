using Defines;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilityUpgradeFixedInfo
{
    public string title;

    [field: SerializeField] public List<AbilityRangePerRank> abilityRangePerRanks { get; private set; } = new List<AbilityRangePerRank>();

    // 업글 관련
    public EStatusType statusType;

    // 비용 관련
    public ECurrencyType currencyType;


    public AbilityUpgradeFixedInfo(string title, Dictionary<Rank, (int, int)> abilityDict, EStatusType statusType, ECurrencyType currencyType)
    {
        this.title = title;

        foreach (var pair in abilityDict)
            abilityRangePerRanks.Add(new AbilityRangePerRank(pair.Key, pair.Value));

        this.statusType = statusType;
        this.currencyType = currencyType;
    }
}

[Serializable]
public struct AbilityRangePerRank
{
    public AbilityRangePerRank(Rank rank, (int, int) range)
    {
        this.rank = rank;
        minRange = range.Item1;
        maxRange = range.Item2;
    }

    public Rank rank;
    public int minRange;
    public int maxRange;
}
