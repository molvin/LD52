using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitKamikazeAttack : UnitAttack
{
    public float ExplosionActivationRange, ExplosionRadius;

    protected override void Attack(Entity Target)
    {
        for(int i = 0; i<GameManager.Instance.playerUnits.Count; i++)
        {
            if(Vector2.Distance(Entity.gameObject.transform.position, Target.gameObject.transform.position) <= ExplosionRadius)
            {
                RaycastHit Hit;
                if (Physics.Raycast(transform.position,(GameManager.Instance.playerUnits[i].gameObject.transform.position - Entity.gameObject.transform.position).normalized, out Hit, AttackDistance) && Hit.transform == GameManager.Instance.playerUnits[i].gameObject.transform)
                {
                    GameManager.Instance.playerUnits[i].GetComponent<UnitHealth>().TakeDamage(Damage);
                }
            }
        }

        //Target.Get<UnitHealth>().TakeDamage(Damage);

        
        Entity.Get<UnitHealth>().TakeDamage(100);


    }
}
