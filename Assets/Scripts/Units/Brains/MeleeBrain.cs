using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeleeBrain : AIBrainBase
{
    void Update()
    {
        if (!Entity.Has<Movement>())
            return;

        List<Entity> Enemies = GameManager.Instance.EntitiesInGame
            .Where(e => e.Team != Entity.Team)
            .OrderBy(e => e.transform.position.Dist2D(transform.position))
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
                var Movement = Entity.Get<Movement>();
                Movement.FindPath(Hit.transform.position);
                break;
            }
        }
    }
}
