using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class AbilityCalculator
{
    [field: SerializeField] public PercentagePerRank[] percentagePerRankArray { get; private set; }

    private int[] actualPercentageArray;

    private const int MIN_PERCENT_INT = 1;
    private const int MAX_PERCENT_INT_EXCLUSIVE = 101;

    public void SetPercentageLength(int length)
    {
        percentagePerRankArray = new PercentagePerRank[length];
    }

    public int GetRandomPercent(List<AbilityRangePerRank> abilityRangePerRanks, out Rank rank)
    {
        int index = GetRankRandomIndex();

        AbilityRangePerRank rangePerRank = abilityRangePerRanks[index];
        rank = rangePerRank.rank;
        
        int percent = UnityEngine.Random.Range(rangePerRank.minRange, rangePerRank.maxRange);
        return percent;
    }

    private int GetRankRandomIndex()
    {
        if (actualPercentageArray == null)
            Init();

        int length = actualPercentageArray.Length;

        int random = UnityEngine.Random.Range(MIN_PERCENT_INT, MAX_PERCENT_INT_EXCLUSIVE);

        for (int i = 0; i < length; i++)
        {
            bool isUnderZero = random - actualPercentageArray[i] <= 0;
            if (isUnderZero)
                return length - i - 1;
        }

        return -1;
    }

    private void Init()
    {
        int length = percentagePerRankArray.Length;
        actualPercentageArray = new int[length];
        // 1 9 20 30 40
        // 이전 값보다는 크고 현재 값보다는 작거나 같음
        int total = 0;
        for (int i = 0; i < length; i++)
        {
            total += percentagePerRankArray[length - i - 1].percentage;
            actualPercentageArray[i] = total;
        }
    }

    public AbilityUpgradeFixedInfo GetRandomFixedInfo(List<AbilityUpgradeFixedInfo> abilityUpgradeFixedInfo)
    {
        int index = UnityEngine.Random.Range(0, abilityUpgradeFixedInfo.Count);
        return abilityUpgradeFixedInfo[index];
    }
}