using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAoEAttack : UnitAttack
{
    protected override void Attack(Entity Entity)
    {
        AudioManager.Instance.PlayAudio(AttackSound, transform.position);
        Entity.Get<UnitHealth>().TakeDamage(Damage, transform.position);
        if (Entity.TryGet(out Movement MovementComp))
        {
            MovementComp.AddForce(Vector3.zero);
        }
    }
}
