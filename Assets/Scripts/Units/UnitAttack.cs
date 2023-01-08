using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class UnitAttack : UnitBase
{
    public LayerMask ObstacleMask;
    public int Damage;
    public float KnockbackForce;
    public float AttackDistance;
    public float AttackTime;
    private float LastAttackTime = 0.0f;
    public float WindUpTime;
    public float WindDownTime;

    [HideInInspector]
    public bool TimeToStrike;

    public bool CanAttack() => Time.time - LastAttackTime > AttackTime;
    public bool IsEnemyTargetable(Entity Other) => Entity.Team != Other.Team && (Entity.Team == Team.Enemy || Other.isSeenByPlayer);

    void Update()
    {
        if (!CanAttack())
            return;

        if (TimeToStrike)
        {
            return;
        }

        List<Entity> Enemies = GameManager.Instance.EntitiesInGame
            .Where(e => IsEnemyTargetable(e) && e.Has<UnitHealth>())
            .OrderBy(e => transform.position.Dist2D(e.transform.position))
            .ToList();

        foreach (Entity Enemy in Enemies)
        {
            float Distance = transform.position.Dist2D(Enemy.transform.position);
            if (Distance > AttackDistance)
                return;

            RaycastHit Hit;
            if (!Physics.SphereCast(
                transform.position,
                0.6f,
                (Enemy.transform.position - transform.position).normalized,
                out Hit,
                Mathf.Min(AttackDistance, Vector3.Distance(Enemy.transform.position, transform.position)),
                ObstacleMask))
            {
                StartCoroutine(AttackActionStart(Enemy));
                break;
            }
        }
    }

    private IEnumerator AttackActionStart(Entity Enemy)
    {
        TimeToStrike = true;
        Entity.Get<Movement>().CanMove = false;
        yield return new WaitForSeconds(WindUpTime);
        if (Enemy == null)
        {
            Entity.Get<Movement>().CanMove = true;
            TimeToStrike = false;
            yield break;
        }
        Attack(Enemy);
        yield return new WaitForSeconds(WindDownTime);
        TimeToStrike = false;
        LastAttackTime = Time.time;
        Entity.Get<Movement>().CanMove = true;
        yield return 0;
    }

    protected virtual void Attack(Entity Entity) { }
}
