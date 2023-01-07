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

    public bool CanAttack() => Time.time - LastAttackTime > AttackTime;

    void Update()
    {
        if (!CanAttack())
            return;

        List<Entity> Enemies = GameDirector.Instance.EntitiesInGame
            .Where(e => e.Team != Entity.Team)
            .Where(e => e.Components.ContainsKey(typeof(UnitHealth)))
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
                Attack(Entity);
                LastAttackTime = Time.time;
            }
        }
    }

    protected virtual void Attack(Entity Entity) { }
}
