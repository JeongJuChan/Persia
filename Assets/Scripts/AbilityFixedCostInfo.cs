using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[CreateAssetMenu(menuName = "SO/AbilityFixedCostInfo", fileName = "AbilityFixedCostInfo")]
public class AbilityFixedCostInfo : ScriptableObject
{
    public int level;
    public int cost;
}
