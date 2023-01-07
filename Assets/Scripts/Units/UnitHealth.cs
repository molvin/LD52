using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth : UnitBase
{
    public int Max;
    public int Current;

    public void TakeDamage(int dmg)
    {
        if (Current == 0)
            return;
        Current = Mathf.Max(Current - dmg, 0);
        if(Current == 0)
        {
            GameManager.Instance.KillUnit(Entity);
        }
    }
}
