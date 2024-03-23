using System;
using System.Collections.Generic;
using UnityEngine;

public class AbilityData
{
    public List<AbilityUpgradeInfo> abilityUpgradeInfos { get; private set; } = new List<AbilityUpgradeInfo>();
    [SerializeField] private AbilityFixedCostInfo[] fixedCostInfos;

    public void Init()
    {
        foreach (var costInfo in fixedCostInfos)
        {
            AbilityUpgradeInfo info = new AbilityUpgradeInfo();
            info.level = costInfo.level;

            info.cost = costInfo.cost;

            info.Load();
            abilityUpgradeInfos.Add(info);
        }
    }
}