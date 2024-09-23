using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StaminaSystem : MonoBehaviour
{
    public int MaxStamina;      // Maximum stamina
    public int RegenRate;       // Stamina regeneration rate (how fast it regenerates per second)
    public int currentValue;    // Current stamina value
    public bool fulled, isRegen; // Flags for full stamina and ongoing regeneration

    public delegate void ValueChangeHandler(object sender);
    public event ValueChangeHandler OnValueChange;

    public int CurrentValue
    {
        get { return currentValue; }
        set
        {
            if (this.currentValue != value)
            {
                this.currentValue = value;
                fulled = (currentValue >= MaxStamina); // Check if stamina is full
                this.currentValue = Mathf.Clamp(this.currentValue, 0, MaxStamina); // Clamp value between 0 and MaxStamina

                OnValueChanged(); // Trigger value change event

                // If stamina is full, stop regenerating
                if (fulled)
                {
                    isRegen = false;
                }
            }
        }
    }

    public bool HasEnoughStamina(int amount)
    {
        return CurrentValue >= amount;
    }

    protected virtual void OnValueChanged()
    {
        OnValueChange?.Invoke(this);
    }

    private async UniTaskVoid StartStaminaRegeneration()
    {
        if (isRegen) // Prevent multiple regeneration processes from starting
            return;

        isRegen = true;

        while (!fulled && isRegen) // Keep regenerating until stamina is full
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1f / RegenRate)); // Wait based on RegenRate

            RegenerateStamina(1); // Regenerate stamina 1 point at a time

            if (CurrentValue >= MaxStamina)
            {
                fulled = true;  // Mark stamina as full
                isRegen = false; // Stop regeneration when full
            }
        }
    }

    private void Start()
    {
        currentValue = MaxStamina; // Initialize stamina as full at the start
        fulled = true;             // Stamina starts full
    }

    public void Consume(int amount)
    {
        CurrentValue -= amount;  // Reduce stamina by the consumed amount

        if (fulled)
        {
            fulled = false;       // Mark stamina as no longer full
        }

        if (!isRegen)             // If not already regenerating, start the regeneration process
        {
            StartStaminaRegeneration();
        }
    }

    public void RegenerateStamina(int regenValue)
    {
        CurrentValue += regenValue;  // Regenerate stamina by the specified value
    }
}
