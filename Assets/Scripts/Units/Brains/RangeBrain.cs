using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class RangeBrain : AIBrainBase
{
    void Update()
    {
        if (!Entity.Has<Movement>())
            return;

        List<Entity> Enemies = GameManager.Instance.EntitiesInGame
            .Where(e => e.Team != Entity.Team)
            .OrderByDescending(e => e.transform.position.Dist2D(transform.position))
            .ToList();

        foreach (Entity Enemy in Enemies)
        {
            Vector3 Direction = Enemy.transform.position - transform.position;

            RaycastHit Hit;
            if (Physics.Raycast(
                transform.position,
                Direction.normalized,
                out Hit,
                Direction.magnitude)
            && Hit.transform == Enemy.transform)
            {
                var Attack = Entity.GetComponent<UnitAttack>();
                var Movement = Entity.Get<Movement>();
                Movement.FindPath(Hit.transform.position - Direction.normalized * Attack.AttackDistance * 0.8f);
                break;
            }
        }
    }
}