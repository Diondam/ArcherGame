using System.Collections;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopPerma : MonoBehaviour
{
    [FoldoutGroup("Price")]
    public int UPGRADE_COST = 100;
    [FoldoutGroup("Price")]
    public float REFUND_RATE = 0.5f;

    [FoldoutGroup("Loaded Stats")]
    public int playerGold;
    [FoldoutGroup("Loaded Stats")]
    [ReadOnly] public int permaHPUpgrades, permaSpeedUpgrades, permaDamageUpgrades;

    // Baseline values to track confirmed upgrades
    private int confirmedHP, confirmedSpeed, confirmedDamage;

    [FoldoutGroup("UI")]
    public TMP_Text goldText;
    [FoldoutGroup("UI")]
    public Button exitStatsButton, confirmModifierPermanentButton;

    #region HP Buttons
    [FoldoutGroup("UI/Health")]
    public TMP_Text healthStat;
    [FoldoutGroup("UI/Health")]
    public Button hpPlusButton, hpMinusButton;
    #endregion

    #region Speed Buttons
    [FoldoutGroup("UI/Speed")]
    public TMP_Text speedStat;
    [FoldoutGroup("UI/Speed")]
    public Button speedPlusButton, speedMinusButton;
    #endregion

    #region Damage Buttons
    [FoldoutGroup("UI/Damage")]
    public TMP_Text damageStat;
    [FoldoutGroup("UI/Damage")]
    public Button damagePlusButton, damageMinusButton;
    #endregion

    [FoldoutGroup("Debug")]
    private int receiptHP, receiptSpeed, receiptDamage;
    private PlayerStats _stats;

    private void Start()
    {
        ButtonSetup();
        LoadPlayerSave();
        UpdateUI();
    }

    private void ButtonSetup()
    {
        hpPlusButton.onClick.AddListener(() => ModifyPermaStat("HP", true));
        hpMinusButton.onClick.AddListener(() => ModifyPermaStat("HP", false));
        speedPlusButton.onClick.AddListener(() => ModifyPermaStat("Speed", true));
        speedMinusButton.onClick.AddListener(() => ModifyPermaStat("Speed", false));
        damagePlusButton.onClick.AddListener(() => ModifyPermaStat("Damage", true));
        damageMinusButton.onClick.AddListener(() => ModifyPermaStat("Damage", false));

        confirmModifierPermanentButton.onClick.AddListener(ConfirmUpdateStats);
        exitStatsButton.onClick.AddListener(() => ToggleShopUI(false));
    }

    public void LoadPlayerSave()
    {
        _stats = PlayerController.Instance._stats;

        permaHPUpgrades = _stats.permaHP_UpAmount;
        permaSpeedUpgrades = _stats.permaSpeed_UpAmount;
        permaDamageUpgrades = _stats.permaDamage_UpAmount;

        receiptHP = permaHPUpgrades;
        receiptSpeed = permaSpeedUpgrades;
        receiptDamage = permaDamageUpgrades;

        confirmedHP = permaHPUpgrades;
        confirmedSpeed = permaSpeedUpgrades;
        confirmedDamage = permaDamageUpgrades;

        playerGold = _stats.playerGold;
    }

    public void ModifyPermaStat(string statType, bool increase)
    {
        int change = increase ? 1 : -1;
        bool canModify = true;

        if (increase && playerGold < UPGRADE_COST) return;

        switch (statType)
        {
            case "HP":
                if (CanUpdateReceipt(ref receiptHP, change, confirmedHP))
                    playerGold -= increase ? UPGRADE_COST : -(int)(UPGRADE_COST * REFUND_RATE);
                break;

            case "Speed":
                if (CanUpdateReceipt(ref receiptSpeed, change, confirmedSpeed))
                    playerGold -= increase ? UPGRADE_COST : -(int)(UPGRADE_COST * REFUND_RATE);
                break;

            case "Damage":
                if (CanUpdateReceipt(ref receiptDamage, change, confirmedDamage))
                    playerGold -= increase ? UPGRADE_COST : -(int)(UPGRADE_COST * REFUND_RATE);
                break;

            default:
                canModify = false;
                break;
        }

        if (canModify)
            UpdateUI();
        else
            Debug.Log("Cannot decrease stat below 0!");
    }

    public void ConfirmUpdateStats()
    {
        permaHPUpgrades = receiptHP;
        permaSpeedUpgrades = receiptSpeed;
        permaDamageUpgrades = receiptDamage;

        confirmedHP = permaHPUpgrades;
        confirmedSpeed = permaSpeedUpgrades;
        confirmedDamage = permaDamageUpgrades;

        var data = new PermaStatsData
        {
            Gold = playerGold,
            HPUpgradesData = permaHPUpgrades,
            SpeedUpgradesData = permaSpeedUpgrades,
            DamageUpgradesData = permaDamageUpgrades,
        };

        PermanCRUD.SavePermanentStats(data);

        _stats.LoadSave();
        Debug.Log("Permanent upgrades confirmed and saved.");
    }

    private bool CanUpdateReceipt(ref int receiptValue, int change, int confirmedValue)
    {
        int newValue = receiptValue + change;
        if (newValue >= 0)
        {
            if (change < 0 && receiptValue > confirmedValue) // Refund part of the upgrade cost only if above confirmed baseline
                playerGold += (int)(UPGRADE_COST * REFUND_RATE);

            receiptValue = newValue;
            return true;
        }
        return false;
    }

    public void UpdateUI()
    {
        healthStat.text = $"HP: {CalculatePercent(receiptHP).ToString("F2")}%";
        speedStat.text = $"SPD: {CalculatePercent(receiptSpeed).ToString("F2")}%";
        damageStat.text = $"ATK: {CalculatePercent(receiptDamage).ToString("F2")}%";
        goldText.text = $"Soul Well: {playerGold}";
    }

    public void ToggleShopUI(bool toggle)
    {
        gameObject.SetActive(toggle);
    }

    public float CalculatePercent(int amount)
    {
        return (Mathf.Pow(amount, 0.6f));
    }
}
