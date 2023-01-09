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
    [Header("Audio")]
    public AudioClip AttackSound;

    [HideInInspector]
    public bool TimeToStrike;

    public bool CanAttack() => Time.time - LastAttackTime > AttackTime;
    public bool IsEnemyTargetable(Entity Other) => Entity.Team != Other.Team && (Entity.Team == Team.Enemy || Other.isSeenByPlayer);

    public Entity Target;

    protected virtual void Update()
    {
        List<Entity> Enemies = GameManager.Instance.EntitiesInGame
        .Where(e => IsEnemyTargetable(e) && e.Has<UnitHealth>())
        .OrderBy(e => transform.position.Dist2D(e.transform.position))
        .ToList();

        Target = null;
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
                Target = Enemy;
                break;
            }
        }

        if (!CanAttack())
            return;

        if (TimeToStrike)
        {
            return;
        }

        if(Target)
            StartCoroutine(AttackActionStart(Target));
    }

    protected IEnumerator AttackActionStart(Entity Enemy)
    {
        TimeToStrike = true;
        bool canMove = Entity.TryGet(out Movement movement);
        if(canMove)
            movement.CanMove = false;
        yield return new WaitForSeconds(WindUpTime);
        if (Enemy == null)
        {
            if (canMove)
                movement.CanMove = true;
            TimeToStrike = false;
            yield break;
        }
        Attack(Enemy);
        yield return new WaitForSeconds(WindDownTime);
        TimeToStrike = false;
        LastAttackTime = Time.time;
        if (canMove)
            movement.CanMove = true;
        yield return 0;
    }

    protected virtual void Attack(Entity Entity) { }
}
