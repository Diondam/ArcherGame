using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_Buff_PreciseArcher : ISkill
{
    void OnEnable()
    {
        SetToggle(true);
    }
    
    void SetToggle(bool toggle)
    {
        if(_pc == null) _pc = PlayerController.Instance;
        _pc._arrowController.IsPreciseArcher = toggle;
    }
}
