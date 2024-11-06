using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [SerializeField, ReadOnly]
    PlayerStats playerStat;
    [SerializeField, ReadOnly]
    SkillHolder skillHolder;
    public List<GameObject> skillList = new List<GameObject>();
    public List<Image> skillHolderObj;
    private float baseHealth, bonusHealth, baseSpeed, bonusSpeed, baseStamina, bonusStamina, baseStaRegen, bonusStaRegen, baseAtk, bonusATK;
    public TMP_Text baseHealthText, bonusHealthText, baseSpeedText, bonusSpeedText, baseStaminaText, bonusStaminaText, baseStaRegenText, bonusStaRegenText, baseAtkText, bonusATKText;
    // Start is called before the first frame update
    [Button]
    public void UpdateInfo()
    {
        playerStat = PlayerController.Instance._stats;
        skillHolder = SkillHolder.Instance;
        skillList = skillHolder.skillList;
        for (int i = 0; i < skillList.Count; i++)
        {
            skillHolderObj[i].sprite = skillList[i].GetComponent<ISkill>().Icon;
        }
        ShowStat();
    }

    public void ShowStat()
    {
        baseHealth = (playerStat.totalMaxHealth - playerStat.bonusHealth);
        bonusHealth = (playerStat.bonusHealth);
        baseSpeed = playerStat.speed - playerStat.bonusSpeed;
        bonusSpeed = playerStat.bonusSpeed;
        baseStamina = (int)(playerStat.maxStamina - playerStat.bonusMaxStamina);
        bonusStamina = (playerStat.bonusMaxStamina);
        baseStaRegen = (playerStat.regenRate - playerStat.bonusRegenRate);
        bonusStaRegen = (playerStat.bonusRegenRate);
        baseAtk = (playerStat.defaultDamage * playerStat.PermaDamage_Percent);
        bonusATK = (playerStat.bonusDamage);

        baseHealthText.text = "" + baseHealth.ToString("0.0"); 
        bonusHealthText.text = bonusHealth > 0 ? "+" + bonusHealth.ToString("0.0") : ""; 
        baseSpeedText.text = "" + baseSpeed.ToString("0.0"); 
        bonusSpeedText.text = bonusSpeed > 0 ? "+" + bonusSpeed.ToString("0.0") : "";
        baseStaminaText.text = "" + baseStamina;
        bonusStaminaText.text = bonusStamina > 0 ? "+" + bonusStamina.ToString("0.0") : ""; 
        baseStaRegenText.text = "" + baseStaRegen.ToString("0.0"); 
        bonusStaRegenText.text = bonusStaRegen > 0 ? "+" + bonusStaRegen.ToString("0.0") : ""; 
        baseAtkText.text = "" + baseAtk.ToString("0.0"); 
        bonusATKText.text = bonusATK > 0 ? "+" + bonusATK.ToString("0.0") : "";
    }

}
