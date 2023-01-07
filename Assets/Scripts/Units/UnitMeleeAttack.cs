using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMeleeAttack : UnitBase
{
    public int Damage;
    public float AttackRange;
    public float AttackFromDistance;
    public float AttackTime;

    private float LastAttackTime = 0.0f;

    public bool CanAttack() => Time.time - LastAttackTime > AttackTime;

    void Update()
    {
        if (!CanAttack())
            return;

        List<Entity> Enemies = GameDirector.GetEnemies(Entity);
    }
}
