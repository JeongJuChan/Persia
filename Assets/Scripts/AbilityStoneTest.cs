using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Keiwando.BigInteger;

public class AbilityStoneTest : MonoBehaviour
{
    public int amount = 100;
}
#if UNITY_EDITOR
[CustomEditor(typeof(AbilityStoneTest))]
public class AbilityStoneTestEditor : Editor
{
    private AbilityStoneTest ability;
    private void OnEnable()
    {
        ability = target as AbilityStoneTest;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("¾îºô¸®Æ¼ ½ºÅæ Áõ°¡"))
        {
            CurrencyManager.instance.AddCurrency(ECurrencyType.AbilityStone, new BigInteger(ability.amount));
        }
    }
}
#endif