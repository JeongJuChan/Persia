using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "SO/RankSO", fileName = "RankSO")]
public class RankSO : ScriptableObject
{
    public PercentagePerRank[] percentagePerRankArray = new PercentagePerRank[Enum.GetValues(typeof(Rank)).Length - 1];
}

[Serializable]
public struct PercentagePerRank
{
    public Rank rank;
    public int percentage;
}
