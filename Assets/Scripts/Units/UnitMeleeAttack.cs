using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class UnitMeleeAttack : UnitAttack
{
    protected override void Attack(Entity Entity)
    {
        Entity.Get<UnitHealth>().TakeDamage(Damage);
    }
}
