using Defines;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UIAbilityBar : UIBase
{
    [SerializeField] private HoldCheckerButton upgradeBtn;

    [SerializeField] private Image costImage;

    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text upgradeStatText;
    [SerializeField] private TMP_Text costText;

    [SerializeField] private Color[] colorOfRankArray;

    private Dictionary<Rank, Color> colorOfRankDict = new Dictionary<Rank, Color>();
    
    private AbilityUpgradeInfo upgradeInfo;

    [SerializeField] private RectTransform effectRect;

    public Transform GetButtonRect()
    {
        return upgradeBtn.transform;
    }
    public void ShowUI(AbilityUpgradeInfo info)
    {
        upgradeInfo = info;
        InitializeUI();

        CurrencyManager.instance.onCurrencyChanged += OnCurrencyUpdate;
    }

    public override void CloseUI()
    {
        base.CloseUI();

        CurrencyManager.instance.onCurrencyChanged -= OnCurrencyUpdate;
    }

    private void OnCurrencyUpdate(ECurrencyType type, string amount)
    {
        if (type == upgradeInfo.currencyType)
        {
            if (upgradeInfo.CheckUpgradeCondition())
            {
                // TODO 글씨 색 회색
                upgradeBtn.interactable = true;
                costText.color = Color.white;
            }
            else
            {
                // TODO 글씨 색 흰색 
                upgradeBtn.interactable = false;
                costText.color = Color.red;
            }
        }
    }

    protected void Awake()
    {
        Init();
        InitializeBtn();
        InitColorOfRank();
    }

    private void Init()
    {
        
    }

    private void InitColorOfRank()
    {
        Rank[] ranks = Enum.GetValues(typeof(Rank)) as Rank[];
        for (int i = 0; i < ranks.Length - 1; i++)
        {
            colorOfRankDict.Add(ranks[i], colorOfRankArray[i]);
        }
    }

    private void InitializeBtn()
    {
        upgradeBtn.onClick.AddListener(() => UpgradeBtn(upgradeInfo.statusType));
        upgradeBtn.onExit.AddListener(CurrencyManager.instance.SaveCurrencies);
    }

    private void UpgradeBtn(EStatusType type)
    {
        // TODO currency manager를 통해서 돈 빼기!
        if (TryUpgrade(type))
        {
            UpdateUI();
        }
        else
        {
            MessageUIManager.instance.ShowCenterMessage(CustomText.SetColor("어빌리티 스톤", Color.yellow) + "가 부족합니다.");
        }
    }

    private bool TryUpgrade(EStatusType type)
    {
        if (CurrencyManager.instance.SubtractCurrency(upgradeInfo.currencyType, upgradeInfo.cost))
        {
            UpgradeManager.instance.UpgradePercentStatus(upgradeInfo);

            // Show Effect
            //UIEffectManager.instance.ShowUpgradeEffect(image.transform);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateUI()
    {
        levelText.text = $"Lv.{upgradeInfo.level}";

        rankText.text = $"[{upgradeInfo.rank}]";
        rankText.color = colorOfRankDict[upgradeInfo.rank];
        UpdateDescription();

        costText.text = upgradeInfo.cost.ToString();

        upgradeBtn.interactable = upgradeInfo.CheckUpgradeCondition();
    }

    private void InitializeUI()
    {
        costImage.sprite = CurrencyManager.instance.GetIcon(upgradeInfo.currencyType);

        UpdateDescription();

        UpdateUI();
    }

    private void UpdateDescription()
    {
        upgradeStatText.text = upgradeInfo.percent == 0 ?
                    ": 없음" :
                    $": {upgradeInfo.title} + {upgradeInfo.percent}%";
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }
}