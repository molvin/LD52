using System.Collections;
using System.Collections.Generic;
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
        yield return new WaitForSeconds(1);
        List<Entity> Players = GameManager.Instance.playerUnits;
        foreach (Entity player in Players)
        {
            if (Vector2.Distance(Entity.transform.position, player.transform.position) <= ExplosionRadius)
            {
                RaycastHit Hit;
                if (Physics.Raycast(transform.position, (player.transform.position - Entity.transform.position).normalized, out Hit, ExplosionRadius) && Hit.transform == player.transform)
                {
                    player.Get<UnitHealth>().TakeDamage(Damage);
                }
            }
        }
        AudioManager.Instance.PlayAudio(AttackSound, transform.position);
        Entity.Get<UnitHealth>().TakeDamage(500000000);
        yield return 0;
    }
    
}
