using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitKamikazeAttack : UnitAttack
{
    public float ExplosionRadius;
    private bool Exploding;

    protected override void Attack(Entity Target)
    {
        if (!Exploding)
        {
            StartCoroutine(GoKaploow());
        }
    }

     private IEnumerator GoKaploow()
    {
        Exploding = true;
        Entity.Get<Movement>().enabled = false;
        yield return new WaitForSeconds(WindUpTime);

        List<Entity> Enemies = GameManager.Instance.EntitiesInGame
            .Where(e => e.Team != Entity.Team)
            .ToList();
        foreach (Entity enemy in Enemies)
        {
            if (Vector2.Distance(Entity.transform.position, enemy.transform.position) <= ExplosionRadius)
            {
                RaycastHit Hit;
                if (Physics.Raycast(transform.position, (enemy.transform.position - Entity.transform.position).normalized, out Hit, ExplosionRadius) && Hit.transform == enemy.transform)
                {
                    enemy.Get<UnitHealth>().TakeDamage(Damage, transform.position);
                }
            }
        }
        AudioManager.Instance.PlayAudio(AttackSound, transform.position);
        Entity.Get<UnitHealth>().TakeDamage(500000000, transform.position);
        yield return 0;
    }
    
}
