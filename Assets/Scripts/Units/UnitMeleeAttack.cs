using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class UnitMeleeAttack : UnitAttack
{
    protected override void Attack(Entity Entity)
    {
        AudioManager.Instance.PlayAudio(AttackSound, transform.position);
        Entity.Get<UnitHealth>().TakeDamage(Damage);
        if (Entity.TryGet(out Movement MovementComp))
        {
            MovementComp.AddForce(Vector3.zero);
        }
    }
}
