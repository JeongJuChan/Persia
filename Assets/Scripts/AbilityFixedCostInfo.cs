using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct AbilityFixedCostInfo
{
    public AbilityFixedCostInfo(int level, int cost)
    {
        this.level = level;
        this.cost = cost;
    }

    public int level;
    public int cost;
}
