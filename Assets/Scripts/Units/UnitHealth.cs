using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth : UnitBase
{
    public int Max;
    public int Current;
    public HealthBar healthBar;
    private UnitRagdoll ragdoll;

    public void Start()
    {
        ragdoll = transform.GetComponentInChildren<UnitRagdoll>();
    }
    public void TakeDamage(int dmg)
    {
        if (Current == 0)
            return;
        Current = Mathf.Max(Current - dmg, 0);
        healthBar?.UnitTakeDamage(dmg);
        if (Current == 0)
        {
            ragdoll.Explode(dmg, transform.position);
            GameManager.Instance?.KillUnit(Entity);
        }
    }
}
