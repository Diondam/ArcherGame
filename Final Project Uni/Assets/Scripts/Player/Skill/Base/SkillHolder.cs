using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SkillHolder : MonoBehaviour
{
    [ReadOnly] public float currentCD;
    [ReadOnly] public float coolDownUI; // This will store the cooldown progress (0 to 1)

    [FoldoutGroup("Skill List")]
    public List<GameObject> skillList = new List<GameObject>();
    [FoldoutGroup("Skill List")]
    public List<GameObject> passiveSkillList = new List<GameObject>();
    [FoldoutGroup("Skill List")]
    public List<GameObject> activeSkillList = new List<GameObject>();
    [FoldoutGroup("Skill List")]
    public List<string> SkillIDList;

    [FoldoutGroup("Current Active Skill")]
    public int currentActiveSkill = 0;
    [FoldoutGroup("Current Active Skill")]
    [ReadOnly] public GameObject activeSkill;
    [FoldoutGroup("Current Active Skill")]
    [ReadOnly] public ISkill currentSkill;

    [FoldoutGroup("Setup")] 
    public float SoulRecover = 20;
    [FoldoutGroup("Setup")]
    public PlayerController _pc;
    [FoldoutGroup("Setup")]
    public List<GameObject> StartSkill;
    [FoldoutGroup("Setup")] 
    public Image currentSkillUISprite, CDOverlay;
    [FoldoutGroup("Setup")] 
    [CanBeNull] public Button SwitchButton;

    public static SkillHolder Instance;

    void Start()
    {
        Instance = this;
        _pc = PlayerController.Instance;

        InitStartSkill();
    }

    async UniTaskVoid InitStartSkill()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(0.1f));
        // Instantiate and categorize skills based on their type
        foreach (GameObject skillPrefab in StartSkill)
        {
            AddSkill(skillPrefab);
        }

        // Set the initial active skill from the ActiveSkillList
        SetActiveSkill(currentActiveSkill);
    }
    
    void Update()
    {
        UpdateCooldownUI();
    }

    [Button]
    void SetActiveSkill(int slot)
    {
        if (slot >= 0 && slot < activeSkillList.Count)
        {
            currentActiveSkill = slot;
            activeSkill = activeSkillList[slot];
            currentSkill = activeSkill.GetComponent<ISkill>();
            currentSkillUISprite.sprite = currentSkill.Icon;
            
            // Set initial cooldown values
            currentCD = currentSkill.currentCD;
            coolDownUI = (currentSkill.Cooldown > 0) ? Mathf.Clamp01(currentSkill.currentCD / currentSkill.Cooldown) : 0;
        }
    }

    #region Util
    // Utility method for switching to the next active skill
    public void NextSkill()
    {
        if (_pc.blockInput) return;
        Debug.Log("next");
        
        if (currentActiveSkill + 1 < activeSkillList.Count)
            currentActiveSkill += 1;
        else
            currentActiveSkill = 0;

        SetActiveSkill(currentActiveSkill);
    }
    

    [Button]
    public void AddSkill(GameObject skillPrefab)
    {
        ISkill skillComponent = skillPrefab.GetComponent<ISkill>();
        string skillName = skillComponent.name;

        // Check if the skill already exists
        if (SkillIDList.Contains(skillName))
        {
            PlayerController.Instance._playerData.SoulCollected += SoulRecover;
            Debug.Log("Skill already exists: " + skillName);
            return;
        }

        // Instantiate the skill and set it up
        GameObject skillInstance = Instantiate(skillPrefab);
        skillInstance.transform.SetParent(this.transform);
        skillInstance.transform.localPosition = Vector3.zero;
        skillInstance.transform.localRotation = Quaternion.identity;

        skillComponent.Assign();
        SkillIDList.Add(skillName);  // Add to skill ID list to track uniqueness

        // Add the skill to the appropriate list based on its type
        if (skillComponent.type == SkillType.ACTIVE)
        {
            activeSkillList.Add(skillInstance);  // Add to active skill list
        }
        else if (skillComponent.type == SkillType.PASSIVE)
        {
            passiveSkillList.Add(skillInstance);  // Add to passive skill list
            skillComponent.Activate();  // Auto-activate passive skills
        }

        skillList.Add(skillInstance);  // Add all skills to the master list

        // Update the switch button's active state if it exists
        if (SwitchButton != null)
        {
            SwitchButton.gameObject.SetActive(activeSkillList.Count > 1);
            if (activeSkillList.Count == 1) SetActiveSkill(0);
        }
    }

    
    #endregion

    #region Input
    public void ActivateSkill(InputAction.CallbackContext ctx)
    {
        ActivateSkill();
    }

    public void DeactivateSkill(InputAction.CallbackContext ctx)
    {
        DeactivateSkill();
    }

    public void ActivateSkill()
    {
        if (activeSkill == null) return;
        currentSkill.Activate();
    }

    public void DeactivateSkill()
    {
        if (activeSkill == null) return;
        currentSkill.Deactivate();
    }
    #endregion

    #region Timer
    public void Timer()
    {
        if (currentCD >= 0) currentCD -= Time.deltaTime;
    }

    public void TimerAdd(float addTime)
    {
        currentCD += addTime;
    }
    
    private void UpdateCooldownUI()
    {
        if (currentSkill == null) return;

        // Update current cooldown and calculate cooldown percentage (0 to 1)
        currentCD = currentSkill.currentCD;
        coolDownUI = (currentSkill.Cooldown > 0) ? Mathf.Clamp01(currentSkill.currentCD / currentSkill.Cooldown) : 0;

        // update the UI image or any other visual indicator using coolDownUI here
        if(CDOverlay != null)
        CDOverlay.fillAmount = coolDownUI;
    }
    #endregion
}
