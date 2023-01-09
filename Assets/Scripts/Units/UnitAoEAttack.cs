using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitAoEAttack : UnitAttack
{
    public float BlastRadius;
    protected override void Update()
    {
        List<Entity> Enemies = GameManager.Instance.EntitiesInGame
            .Where(e => {
                bool filter = IsEnemyTargetable(e) && e.Has<UnitHealth>() && transform.position.Dist2D(e.transform.position) <= AttackDistance;

                return filter && (!Physics.SphereCast(
                    transform.position,
                    0.6f,
                    (e.transform.position - transform.position).normalized,
                    out RaycastHit Hit,
                    Mathf.Min(AttackDistance, Vector3.Distance(e.transform.position, transform.position)),
                    ObstacleMask));
            })
            .OrderBy(e => transform.position.Dist2D(e.transform.position))
            .ToList();

        Target = null;

        if (Enemies.Count == 0)
            return;
        else if (Enemies.Count <= 2)
        {
            Target = Enemies[0];
        }
        else
        {

        }



        foreach (Entity Enemy in Enemies)
        {
            float Distance = transform.position.Dist2D(Enemy.transform.position);
            if (Distance > AttackDistance)
                return;

        }

        if (!CanAttack())
            return;

        if (TimeToStrike)
        {
            return;
        }

        //if(Target)
            //StartCoroutine(AttackActionStart(Target));
    }
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
