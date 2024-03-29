using System;
using System.Collections;
using System.Collections.Generic;
using Defines;
using Keiwando.BigInteger;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    private void Awake()
    {
        instance = this;
    }

    public event Action<EStatusType, int> onTrainingTypeAndCurrentLevel;
    public event Action<EStatusType, int> onAwakenUpgrade;
    public event Action<int> onBaseAttackUpgrade;
    public event Action<int> onBaseHealthUpgrade;
    public event Action<float> onBaseDamageReductionUpgrade;
    public event Action<int> onBaseManaUpgrade;
    public event Action<int> onBaseRecoveryUpgrade;
    public event Action<float> onBaseCriticalChanceUpgrade;
    public event Action<int> onBaseCriticalDamageUpgrade;

    public event Action<float> onBaseAttackSpeedUpgrade;

    // public event Action<float> onBaseAttackRangeUpgrade;
    public event Action<float> onBaseMovementSpeedUpgrade;

    public event Action<int> onAwakenAttack;
    public event Action<float> onAwakenDamageReduction;
    public event Action<float> onAwakenCriticalChance;
    public event Action<int> onAwakenCriticalDamage;
    public event Action<float> onAwakenAttackSpeed;
    public event Action<int> onAwakenSkillMultiplier;

    public event Action<int> onAbilityAttack;
    public event Action<int> onAbilityHealth;
    public event Action<int> onAbilityCriticalDamage;
    public event Action<int> onAbilitySkillDamage;

    [field: SerializeField] public StatUpgradeInfo[] statUpgradeInfo { get; protected set; }

    [field: SerializeField] public AwakenUpgradeInfo[] awakenUpgradeInfo { get; protected set; }

    [field: SerializeField] public AbilityCalculator abilityCalculator { get; private set; } = new AbilityCalculator();
    [field: SerializeField] public List<AbilityUpgradeFixedInfo> abilityUpgradeFixedInfo { get; protected set; } = new List<AbilityUpgradeFixedInfo>();
    
    [field: SerializeField] public AbilityData abilitydata { get; private set; } = new AbilityData();
    private AbilityUpgradeInfo preUpgradeInfo = new AbilityUpgradeInfo();

    // [field: SerializeField] public SpecialityUpgradeInfo[] specialityUpgradeInfo { get; protected set; }
    // [field: SerializeField] public RelicUpgradeInfo[] relicUpgradeInfo { get; protected set; }
    public void InitStatus(EStatusType type, BigInteger value)
    {
        PlayerManager.instance.status.ChangeBaseStat(type, value);
    }

    public void InitStatus(EStatusType type, float value)
    {
        PlayerManager.instance.status.ChangeBaseStat(type, value);
    }

    public void InitAwaken(EStatusType type, BigInteger value)
    {
        PlayerManager.instance.status.ChangePercentStat(type, value);
    }

    public void InitAwaken(EStatusType type, float value)
    {
        PlayerManager.instance.status.ChangePercentStat(type, value);
    }

    public void UpgradeBaseStatus(StatUpgradeInfo info)
    {
        var status = PlayerManager.instance.status;
        var score = new BigInteger(status.BattleScore.ToString());
        
        if (info.upgradePerLevelInt != 0)
            PlayerManager.instance.status.ChangeBaseStat(info.statusType, info.upgradePerLevelInt);
        else
            PlayerManager.instance.status.ChangeBaseStat(info.statusType, info.upgradePerLevelFloat);
        
        switch (info.statusType)
        {
            case EStatusType.ATK:
                onBaseAttackUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.HP:
                onBaseHealthUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.MP:
                onBaseManaUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.MP_RECO:
                onBaseRecoveryUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.CRIT_DMG:
                onBaseCriticalDamageUpgrade?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.DMG_REDU:
                onBaseDamageReductionUpgrade?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.CRIT_CH:
                onBaseCriticalChanceUpgrade?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.ATK_SPD:
                onBaseAttackSpeedUpgrade?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.MOV_SPD:
                onBaseMovementSpeedUpgrade?.Invoke(info.upgradePerLevelFloat);
                break;
        }

        PlayerManager.instance.status.InitBattleScore();
        MessageUIManager.instance.ShowPower(status.BattleScore, status.BattleScore - score);
        
        info.LevelUp();

        onTrainingTypeAndCurrentLevel?.Invoke(info.statusType, info.level);
    }

    public void UpgradePercentStatus(AwakenUpgradeInfo info)
    {
        var status = PlayerManager.instance.status;
        var score = new BigInteger(status.BattleScore.ToString());
        
        if (info.upgradePerLevelInt != 0)
            PlayerManager.instance.status.ChangePercentStat(info.statusType, new BigInteger(info.upgradePerLevelInt));
        else
            PlayerManager.instance.status.ChangePercentStat(info.statusType, info.upgradePerLevelFloat);
        
        switch (info.statusType)
        {
            case EStatusType.ATK:
                onAwakenAttack?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.CRIT_DMG:
                onAwakenCriticalDamage?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.SKILL_DMG:
                onAwakenSkillMultiplier?.Invoke(info.upgradePerLevelInt);
                break;
            case EStatusType.DMG_REDU:
                onAwakenDamageReduction?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.CRIT_CH:
                onAwakenCriticalChance?.Invoke(info.upgradePerLevelFloat);
                break;
            case EStatusType.ATK_SPD:
                onAwakenAttackSpeed?.Invoke(info.upgradePerLevelFloat);
                break;
        }
        
        PlayerManager.instance.status.InitBattleScore();
        MessageUIManager.instance.ShowPower(status.BattleScore, status.BattleScore - score);
        
        info.LevelUP();
        
        onAwakenUpgrade?.Invoke(info.statusType, info.level);
    }

    public void UpgradePercentStatus(AbilityUpgradeInfo info)
    {
        var status = PlayerManager.instance.status;
        PlayerManager.instance.status.InitBattleScore();
        var score = new BigInteger(status.BattleScore.ToString());

        preUpgradeInfo.SetType(info.statusType);
        preUpgradeInfo.SetUpgradeInfo(info.rank, info.percent);
        
        AbilityUpgradeFixedInfo fixedInfo = abilityCalculator.GetRandomFixedInfo(abilityUpgradeFixedInfo);
        int percent = abilityCalculator.GetRandomPercent(fixedInfo.abilityRangePerRanks, out Rank rank);

        info.SetType(fixedInfo.statusType);
        info.SetTitle(fixedInfo.title);
        info.SetUpgradeInfo(rank, percent);

        PlayerManager.instance.status.ChangePercentStat(preUpgradeInfo.statusType, new BigInteger(-preUpgradeInfo.percent));
        PlayerManager.instance.status.ChangePercentStat(info.statusType, new BigInteger(percent));

        info.Save();


        switch (info.statusType)
        {
            case EStatusType.ATK:
                onAbilityAttack?.Invoke(percent);
                break;
            case EStatusType.HP:
                onAbilityHealth?.Invoke(percent);
                break;
            case EStatusType.CRIT_DMG:
                onAbilityCriticalDamage?.Invoke(percent);
                break;
            case EStatusType.SKILL_DMG:
                onAbilitySkillDamage?.Invoke(percent);
                break;
        }

        PlayerManager.instance.status.InitBattleScore();
        MessageUIManager.instance.ShowPower(status.BattleScore, status.BattleScore - score);

        onAwakenUpgrade?.Invoke(info.statusType, info.level);
    }

    public void InitUpgradeManager()
    {
        // TODO Save & Load Upgrade Information
        if (ES3.KeyExists("Init_Game"))
        {
            LoadUpgradeInfo();
        }
        else
        {
            InitUpgradeInfo();
        }
    }

    private void InitUpgradeInfo()
    {
        foreach (var upgradeInfo in statUpgradeInfo)
        {
            upgradeInfo.Init();
        }

        foreach (var upgradeInfo in awakenUpgradeInfo)
        {
            upgradeInfo.Init();
        }

        
    }

    public void LoadUpgradeInfo()
    {
        foreach (var upgradeInfo in statUpgradeInfo)
        {
            upgradeInfo.Load();
        }

        foreach (var upgradeInfo in awakenUpgradeInfo)
        {
            upgradeInfo.Load();
        }

        foreach (var upgradeInfo in abilitydata.abilityUpgradeInfos)
        {
            upgradeInfo.Load();
        }
    }

    public void SaveUpgradeInfo()
    {
        foreach (var upgradeInfo in statUpgradeInfo)
        {
            upgradeInfo.Save();
        }

        foreach (var upgradeInfo in awakenUpgradeInfo)
        {
            upgradeInfo.Save();
        }
    }
}

[Serializable]
public class AwakenUpgradeInfo
{
    public string gemName => info.gemName;
    public string title => info.title;
    public int level;
    public int maxLevel => info.maxLevel;

    // 업글 관련
    public EStatusType statusType => info.statusType;
    
    public int upgradePerLevelInt => info.upgradePerLevelInt;
    public float upgradePerLevelFloat => info.upgradePerLevelFloat;

    // 비용 관련 
    public ECurrencyType currencyType => info.currencyType;
    public int baseCost => info.baseCost;
    public int increaseCostPerLevel => info.increaseCostPerLevel;

    public BigInteger cost;

    // 꾸미기 관련
    public Sprite image => info.image;

    [SerializeField] private AwakenUpgradeFixedInfo info;

    public void LevelUP()
    {
        ++level;
        cost += (cost * increaseCostPerLevel) / 100;
        Save();
    }

    public void Save()
    {
        DataManager.Instance.Save($"{nameof(AwakenUpgradeInfo)}_{statusType.ToString()}_{nameof(level)}", level);
        DataManager.Instance.Save($"{nameof(AwakenUpgradeInfo)}_{statusType.ToString()}_{nameof(cost)}",
            cost.ToString());
    }

    public void Load()
    {
        level = DataManager.Instance.Load($"{nameof(AwakenUpgradeInfo)}_{statusType.ToString()}_{nameof(level)}",
            level);
        cost = new BigInteger(DataManager.Instance.Load<string>(
            $"{nameof(AwakenUpgradeInfo)}_{statusType.ToString()}_{nameof(cost)}", baseCost.ToString()));

        if (upgradePerLevelInt != 0)
            UpgradeManager.instance.InitAwaken(statusType, (new BigInteger(upgradePerLevelInt)) * level);
        else
            UpgradeManager.instance.InitAwaken(statusType, (upgradePerLevelFloat) * level);
    }

    public bool CheckUpgradeCondition()
    {
        if (level >= maxLevel || cost > CurrencyManager.instance.GetCurrency(currencyType))
            return false;
        return true;
    }


    public void Init()
    {
        level = 0;
        cost = baseCost;
    }
}


[Serializable]
public class StatUpgradeInfo
{
    public string title => info.title;
    public int level;
    public int maxLevel => info.maxLevel;

    // 업글 관련
    public EStatusType statusType => info.statusType;
    
    public int upgradePerLevelInt => info.upgradePerLevelInt;
    
    public float upgradePerLevelFloat => info.upgradePerLevelFloat;

    // 비용 관련
    public ECurrencyType currencyType => info.currencyType;
    public int baseCost => info.baseCost;
    public int increaseCostPerLevel => info.increaseCostPerLevel;

    public BigInteger cost;

    // 꾸미기 관련
    public Sprite image => info.image;

    [SerializeField] private StatUpgradeFixedInfo info;
    
    public void LevelUp()
    {
        ++level;
        cost += (cost * increaseCostPerLevel) / 100;
        Save();
    }

    public void Save()
    {
        DataManager.Instance.Save($"{nameof(StatUpgradeInfo)}_{statusType.ToString()}_{nameof(level)}", level);
        DataManager.Instance.Save($"{nameof(StatUpgradeInfo)}_{statusType.ToString()}_{nameof(cost)}", cost.ToString());
    }

    public void Load()
    {
        level = DataManager.Instance.Load($"{nameof(StatUpgradeInfo)}_{statusType.ToString()}_{nameof(level)}", level);
        cost = new BigInteger(DataManager.Instance.Load<string>(
            $"{nameof(StatUpgradeInfo)}_{statusType.ToString()}_{nameof(cost)}", baseCost.ToString()));

        if (upgradePerLevelInt != 0)
            UpgradeManager.instance.InitStatus(statusType, (new BigInteger(upgradePerLevelInt)) * level);
        else
            UpgradeManager.instance.InitStatus(statusType, (upgradePerLevelFloat) * level);
    }

    public bool CheckUpgradeCondition()
    {
        if (level >= maxLevel || cost > CurrencyManager.instance.GetCurrency(currencyType))
            return false;
        return true;
    }

    public void Init()
    {
        level = 0;
        cost = baseCost;
    }
}

public class AbilityUpgradeInfo
{
    public string title;
    public int level;
    public int percent;
    public Rank rank = Rank.C;

    // 업글 관련
    public EStatusType statusType;

    // 비용 관련
    public ECurrencyType currencyType;
    public int cost;

    public void SetCurrency(ECurrencyType type)
    {
        currencyType = type;
    }

    public void SetUpgradeInfo(Rank rank, int percent)
    {
        this.rank = rank;
        this.percent = percent;
    }

    public void SetTitle(string title)
    {
        this.title = title;
    }

    public void SetType(EStatusType type)
    {
        statusType = type;
    }

    public void SetCostInfo(AbilityFixedCostInfo info)
    {
        level = info.level;
        cost = info.cost;
    }

    public void Save()
    {
        DataManager.Instance.Save($"{level}_{nameof(title)}", title);
        DataManager.Instance.Save($"{level}_{nameof(rank)}", rank);
        DataManager.Instance.Save($"{level}_{nameof(percent)}", percent);
    }
     
    public void Load()
    {
        title = DataManager.Instance.Load($"{level}_{nameof(title)}", title);
        rank = DataManager.Instance.Load($"{level}_{nameof(rank)}", rank);
        percent = DataManager.Instance.Load($"{level}_{nameof(percent)}", percent);

        UpgradeManager.instance.InitStatus(statusType, new BigInteger(percent));
    }

    public bool CheckUpgradeCondition()
    {
        if (cost > CurrencyManager.instance.GetCurrency(currencyType))
            return false;
        return true;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(UpgradeManager))]
public class UpgradeManagerEditor : Editor
{
    private TextAsset abilityProbability;
    private TextAsset abilityCost;
    private TextAsset abilityEffect;

    private UpgradeManager upgrade;

    private void OnEnable()
    {
        upgrade = (UpgradeManager)target;
    }


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        abilityProbability = EditorGUILayout.ObjectField("어빌리티_확률", abilityProbability, typeof(TextAsset), true) as TextAsset;
        if (GUILayout.Button("Load From CSV File"))
        {
            LoadAbilityRank(abilityProbability);
        }
        abilityCost = EditorGUILayout.ObjectField("어빌리티_소모량", abilityCost, typeof(TextAsset), true) as TextAsset;
        if (GUILayout.Button("Load From CSV File"))
        {
            LoadAbilityCost(abilityCost);
        }
        abilityEffect = EditorGUILayout.ObjectField("어빌리티_효과", abilityEffect, typeof(TextAsset), true) as TextAsset;
        if (GUILayout.Button("Load From CSV File"))
        {
            LoadAbilityEffect(abilityEffect);
        }
    }

    private void LoadAbilityRank(TextAsset textAsset)
    {
        if (textAsset == null)
        {
            Debug.LogError("필드에 어빌리티_확률 CSV 파일을 넣어주십시오");
            return;
        }

        string[] rows = textAsset.text.Split('\n');

        Debug.Log(rows[0]);

        int length = rows.Length - 1;
        int totalPercent = 0;

        upgrade.abilityCalculator.SetPercentageLength(length - 1);

        for (int i = 1; i < length; i++)
        {
            string[] elements = rows[i].Split(',');

            Rank rank = GetEnumByComparingName<Rank>(elements[0]);

            int.TryParse(elements[1], out int percent);
            percent = percent < 0 ? 0 : percent;
            totalPercent += percent;

            PercentagePerRank percentPerRank = new PercentagePerRank(rank, percent);
            upgrade.abilityCalculator.percentagePerRankArray[i - 1] = percentPerRank;
        }

        if (totalPercent != 100)
        {
            Debug.LogError("어빌리티_확률 CSV 파일 값이 잘못되었습니다.");
            return;
        }

        EditorUtility.SetDirty(target);
    }

    

    private void LoadAbilityCost(TextAsset textAsset)
    {
        if (textAsset == null)
        {
            Debug.LogError("필드에 어빌리티_소모량 CSV 파일을 넣어주십시오");
            return;
        }

        string[] rows = textAsset.text.Split('\n');

        Debug.Log(rows[0]);

        int length = rows.Length - 1;

        upgrade.abilitydata.SetFixedCostInfoLength(length - 1);

        for (int i = 1; i < length; i++)
        {
            string[] elements = rows[i].Split(',');

            int.TryParse(elements[0], out int level);

            int.TryParse(elements[1], out int cost);

            AbilityFixedCostInfo abilityFixedCostInfo = new AbilityFixedCostInfo(level, cost);
            upgrade.abilitydata.fixedCostInfos[i - 1] = abilityFixedCostInfo;

            if (level < 0 || cost < 0)
            {
                Debug.LogError("어빌리티_소모량 CSV 파일 값이 잘못되었습니다.");
                return;
            }
        }

        EditorUtility.SetDirty(target);
    }

    private void LoadAbilityEffect(TextAsset textAsset)
    {
        if (textAsset == null)
        {
            Debug.LogError("필드에 어빌리티_효과 CSV 파일을 넣어주십시오");
            return;
        }



        string[] rows = textAsset.text.Split('\n');

        Debug.Log(rows[0]);

        int length = rows.Length - 1;

        Dictionary<string, Dictionary<Rank, (int, int)>> abilityEffectDict = 
            new Dictionary<string, Dictionary<Rank, (int, int)>>();

        // None을 포함하기 때문에 -1
        int rankLength = Enum.GetValues(typeof(Rank)).Length - 1;

        upgrade.abilityUpgradeFixedInfo.Clear();

        for (int i = 1; i < length; i++)
        {
            string[] elements = rows[i].Split(',');
            //ATK
            //HP
            //CRIT_DMG
            //SKILL_DMG
            string title = elements[0];
            if (!abilityEffectDict.ContainsKey(title))
                abilityEffectDict.Add(title, new Dictionary<Rank, (int, int)>());

            Rank rank = GetEnumByComparingName<Rank>(elements[2]);
            if (!abilityEffectDict[title].ContainsKey(rank))
            {
                int.TryParse(elements[3], out int min);
                int.TryParse(elements[4], out int max);

                if (min < 0 || max < 0)
                {
                    Debug.LogError("어빌리티_효과 CSV 파일의 최댓값 또는 최솟값이 잘못되었습니다.");
                    return;
                }

                abilityEffectDict[title].Add(rank, (min, max));
            }

            if (abilityEffectDict[title].Keys.Count == rankLength)
            {
                EStatusType statusType = GetEnumByComparingName<EStatusType>(elements[1]);
                ECurrencyType currencyType = GetEnumByComparingName<ECurrencyType>(elements[5]);

                AbilityUpgradeFixedInfo abilityUpgradeFixedInfo = 
                    new AbilityUpgradeFixedInfo(title, abilityEffectDict[title], statusType, currencyType);

                upgrade.abilityUpgradeFixedInfo.Add(abilityUpgradeFixedInfo);
            }
        }

        EditorUtility.SetDirty(target);
    }

    private T GetEnumByComparingName<T>(string element)
    {
        foreach (T comparativeValue in Enum.GetValues(typeof(T)))
        {
            if (string.Compare(element, comparativeValue.ToString()) == 0)
            {
                return comparativeValue;
            }
        }

        return default;
    }
}
#endif