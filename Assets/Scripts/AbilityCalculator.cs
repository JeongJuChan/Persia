﻿using System;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class AbilityCalculator
{
    [SerializeField] RankSO rankSO;
    private int[] actualPercentageArray;

    private const int MIN_PERCENT_INT = 1;
    private const int MAX_PERCENT_INT_EXCLUSIVE = 101;

    public int GetRandomPercent(RankUpgradeRange[] upgradeRangeArray)
    {
        int index = GetRankRandomIndex();
        RankUpgradeRange range = upgradeRangeArray[index];
        int random = UnityEngine.Random.Range(range.upgradeMinInt, range.upgradeMaxInt);
        return random;
    }

    private int GetRankRandomIndex()
    {
        int length = actualPercentageArray.Length;
        if (length == 0)
            Init();

        int random = UnityEngine.Random.Range(MIN_PERCENT_INT, MAX_PERCENT_INT_EXCLUSIVE);

        for (int i = 0; i < length; i++)
        {
            bool isUpperZero = random - actualPercentageArray[i] > 0;
            if (!isUpperZero)
                return i;
        }

        return length;
    }

    private void Init()
    {
        int length = rankSO.percentagePerRankArray.Length;
        actualPercentageArray = new int[length];
        // 1 9 20 30 40
        // 이전 값보다는 크고 현재 값보다는 작거나 같음
        int total = 0;
        for (int i = 0; i < length; i++)
        {
            total += rankSO.percentagePerRankArray[length - i - 1].percentage;
            actualPercentageArray[i] = total;
        }

#if UNITY_EDITOR
        if (total > MAX_PERCENT_INT_EXCLUSIVE - 1)
            Debug.LogError($"{rankSO} 숫자를 잘못 입력하였습니다.");
#endif
    }

    
}