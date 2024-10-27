using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;

public class BotMain : MonoBehaviour
{
    //Data
    public UnitType unitType;
    //public int Team = 0;
    
    [FoldoutGroup("Movement Settings")]
    public float minRange, maxRange, MoveAngle = 80f;
    [FoldoutGroup("Movement Settings")]
    public float cooldown = 2f, countdown;
    [FoldoutGroup("Movement Settings")]
    public float attackRange = 1.5f;
    [FoldoutGroup("Movement Settings/Prediction")]
    public bool UseMovementPrediction;
    [FoldoutGroup("Movement Settings/Prediction")]
    [Range(-1, 1)] public float MovementPredictionThreshold = 0;
    [FoldoutGroup("Movement Settings/Prediction")]
    [Range(0.25f, 2f)] public float MovementPredictionTime = 1f;

    [FoldoutGroup("Setup")]
    public Rigidbody rg;
    [FoldoutGroup("Setup")]
    public Health Health;
    [FoldoutGroup("Setup")]
    public List<BotGun> gun;
    [FoldoutGroup("Setup")]
    [CanBeNull] public BotAnimController _animController;

    public void ResetCooldown()
    {
        countdown = cooldown;
    }
    public void Dead()
    {
        Destroy(this.gameObject);
    }

    public void Shoot(int gunSlot)
    {
        if(gun.Count <= 0) return;
        if(gun[gunSlot] != null && gun[gunSlot].target != null) gun[gunSlot].Fire();
    }
}
