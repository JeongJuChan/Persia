using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilityData
{
    public List<AbilityUpgradeInfo> abilityUpgradeInfos { get; private set; } = new List<AbilityUpgradeInfo>();
    [SerializeField] private AbilityFixedCostInfo[] fixedCostInfos;

    public void Init()
    {
        foreach (var costInfo in fixedCostInfos)
        {
            AbilityUpgradeInfo info = new AbilityUpgradeInfo();

            info.SetCostInfo(costInfo);
            info.SetCurrency(ECurrencyType.AbilityStone);

            abilityUpgradeInfos.Add(info);
        }
    }
}