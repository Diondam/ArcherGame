using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public enum HealthState
{
    Idle, Invincible, Absorbtion
}

public class Health : MonoBehaviour
{
    #region Variables
    
    [FoldoutGroup("Stats")]
    public HealthState healthState;
    [FoldoutGroup("Stats")]
    public int maxHealth;
    [FoldoutGroup("Stats")]
    [ReadOnly, SerializeField] private int health, overHeal;

    
    [FoldoutGroup("Setup")]
    public bool fillOnStart = true;
    [FoldoutGroup("Setup/Event")]
    public UnityEvent HpValueChange, OnDeath;
    [FoldoutGroup("Setup/Event")]
    public UnityEvent<Vector3, float> OnKnockback;

    [FoldoutGroup("Debug")] public bool isAlive = true;
    
    //Calculate
    public int currentHealth
    {
        get { return health; }
        set
        {
            if (value > maxHealth)
            {
                this.overHeal = value - maxHealth;
                value = maxHealth;
            }
            this.health = value;
            if (value <= 0) DeathEvent();
            HPUpdate();
        }
    } //use this
    
    #endregion

    #region Unity Method

    private void Start()
    {
        if(fillOnStart) currentHealth = maxHealth;
    }

    #endregion
    
    #region Event

    public void HPUpdate()
    {
        Debug.Log(this.gameObject.name + " HP Update");
        HpValueChange.Invoke();
    }
    public void DeathEvent()
    {
        Debug.Log(this.gameObject.name + " Dead");
        OnDeath.Invoke();
    }
    
    #endregion

    #region Ults
    
    [FoldoutGroup("Event Test")] [Button]
    public void Invinvicle(float time)
    {
        InvincibleTimer(time);
    }
    
    [FoldoutGroup("Event Test/Basic")]
    [Button]
    public void Hurt(int damage)
    {
        switch (healthState)
        {
            case HealthState.Idle:
                DealDamage(damage);
                break;
            case HealthState.Invincible:
                break;
            case HealthState.Absorbtion:
                Heal(damage);
                break;
            
        }
    }
    
    [FoldoutGroup("Event Test/Extend")] [Button]
    public void DamageOverTime(int damage, float time)
    {
        Debug.Log("Damage Over Time: " + damage + " in " + time + " secs");
        DoT(damage, time);
    }
    
    
    [FoldoutGroup("Event Test/Basic")] [Button]
    public void Heal(int heal)
    {
        Debug.Log("Heal " + heal);
        currentHealth += heal;
    }
    
    [FoldoutGroup("Event Test/Extend")] [Button]
    public void HealOverTime(int heal, float time)
    {
        Debug.Log("Heal Over Time: " + heal + " in " + time + " secs");
        HoT(heal, time);
    }
    
    
    [FoldoutGroup("Event Test/Basic")] [Button]
    public void Knockback(Vector3 Dir, float knockForce)
    {
        //knockback Event
        OnKnockback.Invoke(Dir, knockForce);
    }


    private async UniTaskVoid InvincibleTimer(float time)
    {
        healthState = HealthState.Invincible;
        await UniTask.Delay(TimeSpan.FromSeconds(time));
        healthState = HealthState.Idle;
    }
    void DealDamage(int damage)
    {
        Debug.Log("Hurt " + damage);
        currentHealth -= damage;
    }
    async UniTaskVoid DoT(int damage, float duration)
    {
        float timePerTick = duration / damage; // Calculate the time between each damage tick
        int ticks = damage; // The number of times to apply damage
        for (int i = 0; i < ticks; i++)
        {
            Hurt(1); // Call Hurt with 1 damage for each tick
            if (currentHealth <= 0) break; // Exit the loop if the character is dead
            await UniTask.Delay(TimeSpan.FromSeconds(timePerTick)); // Wait before the next tick
        }
    }
    async UniTaskVoid HoT(int heal, float duration)
    {
        float timePerTick = duration / heal; // Calculate the time between each damage tick
        int ticks = heal; // The number of times to apply damage
        for (int i = 0; i < ticks; i++)
        {
            Heal(1); // Call Hurt with 1 damage for each tick
            if (currentHealth <= 0) break; // Exit the loop if the character is dead
            await UniTask.Delay(TimeSpan.FromSeconds(timePerTick)); // Wait before the next tick
        }
    }
    
    #endregion
}
