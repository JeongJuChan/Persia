using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct PercentagePerRank
{
    public Rank rank;
    public int percentage;

    public PercentagePerRank(Rank rank, int percentage)
    {
        this.rank = rank;
        this.percentage = percentage;
    }
}
