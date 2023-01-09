using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSoul : UnitBase
{
    public float SoulAmount;
    private float base_amount;
    public float BaseAmount { get { return base_amount; } }

    new void Awake()
    {
        base.Awake();
        base_amount = SoulAmount;
    }
}
