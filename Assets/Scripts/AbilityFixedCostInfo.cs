using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "SO/AbilityFixedCostInfo", fileName = "AbilityFixedCostInfo")]
public class AbilityFixedCostInfo : ScriptableObject
{
    public int level;
    public int cost;
}
