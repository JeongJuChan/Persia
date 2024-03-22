using Defines;
using System;
using UnityEngine;

// TODO : 점검
[Serializable]
[CreateAssetMenu(menuName = "SO/AbilityUpgradeFixedInfo", fileName = "AbilityUpgradeFixedInfo")]
public class AbilityUpgradeFixedInfo : ScriptableObject
{
    public string title;

    // 업글 관련
    public EStatusType statusType;
    public RankUpgradeRange[] rankUpgradeRangeArray = new RankUpgradeRange[Enum.GetValues(typeof(Rank)).Length - 1];

    // 비용 관련
    public ECurrencyType currencyType;
    public int baseCost;
    public int increaseCostPerLevel;

    // 꾸미기 관련
    public Sprite image;
}

[Serializable]
public struct RankUpgradeRange
{
    public Rank rank;
    public int upgradeMinInt;
    public int upgradeMaxInt;
}