using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class UnitAttack : UnitBase
{
    public int Damage;
    public float AttackDistance;
    public float AttackTime;
    private float LastAttackTime = 0.0f;
    public float WindUpTime;
    public float WindDownTime;
    private float WindingTime;
    public bool WindingUp;
    public bool WindingDown;
    public bool TimeToStrike;

    public bool CanAttack() => Time.time - LastAttackTime > AttackTime;

    void Update()
    {
        if (!CanAttack())
            return;

        if(WindingUp || WindingDown)
        {
            return;
        }


        List<Entity> Enemies = GameManager.Instance.EntitiesInGame
            .Where(e => e.Team != Entity.Team)
            .Where(e => e.Has<UnitHealth>())
            .OrderBy(e => transform.position.Dist2D(e.transform.position))
            .ToList();

        foreach (Entity Enemy in Enemies)
        {
            float Distance = transform.position.Dist2D(Enemy.transform.position);
            if (Distance > AttackDistance)
                return;

            RaycastHit Hit;
            if (Physics.Raycast(
                transform.position,
                (Enemy.transform.position - transform.position).normalized,
                out Hit,
                AttackDistance)
            && Hit.transform == Enemy.transform)
            {
                if (!TimeToStrike && !WindingUp && !WindingDown)
                {
                    StartCoroutine(AttackActionStart());
                    return;
                }
                Attack(Enemy);
                LastAttackTime = Time.time;
                StartCoroutine(AttackActionEnd());
                break;
            }
        }
    }

    private IEnumerator AttackActionStart()
    {
        WindingUp = true;
        yield return new WaitForSeconds(WindUpTime);
        WindingUp = false;
        TimeToStrike = true;
        yield return 0;
    }

    private IEnumerator AttackActionEnd() 
    {
        WindingDown = true;
        TimeToStrike = false;
        yield return new WaitForSeconds(WindDownTime);
        WindingDown = false;
        yield return 0;
    }

    protected virtual void Attack(Entity Entity) { }
}
