using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitAoEAttack : UnitAttack
{
    public GameObject TelegrapEffect;
    public GameObject ExplosionEffect;
    public float BlastRadius;
    private Vector3 attackLocation;
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
            int close = 0;
            int far = 0;
            Vector3 closeVec = Enemies[0].transform.position;
            Vector3 farVec = Enemies[Enemies.Count - 1].transform.position;

            for (int i = 1; i < Enemies.Count - 1; i++)
            {
                float closeDist = Enemies[0].transform.position.Dist2D(Enemies[i].transform.position);
                float farDist = Enemies[Enemies.Count - 1].transform.position.Dist2D(Enemies[i].transform.position);
                if (closeDist < farDist)
                {
                    close++;
                    closeVec += Enemies[i].transform.position;
                }
                else
                {
                    farVec += Enemies[i].transform.position;
                    far++;
                }
            }

            if (Mathf.Max(close, far) <= 1)
            {
                Target = close >= far ? Enemies[0] : Enemies[Enemies.Count - 1];
            }
            else
            {
                Vector3 vec;
                if (close >= far)
                    vec = closeVec / (close + 1);
                else
                    vec = farVec / (far + 1);

                float best = float.MaxValue;
                foreach (Entity target in Enemies)
                {
                    float dist = target.transform.position.Dist2D(vec);
                    if (dist < best)
                    {
                        best = dist;
                        Target = target;
                    }
                }
            }
        }

        if (!CanAttack())
            return;

        if (TimeToStrike)
        {
            return;
        }

        if(Target)
        {
            GameObject telegraph = ObjectPool.Instance.GetInstance(TelegrapEffect);
            attackLocation = Target.transform.position;
            telegraph.transform.position = attackLocation;
            //telegraph.transform.localScale = Vector3.one * BlastRadius;

            StartCoroutine(AttackActionStart(Target));
        }
    }
    protected override void Attack(Entity _target)
    {
        AudioManager.Instance.PlayAudio(AttackSound, attackLocation);

        GameManager.Instance.EntitiesInGame
            .Where(e => e.Team != Entity.Team && e.Has<UnitHealth>() && e.transform.position.Dist2D(attackLocation) < BlastRadius)
            .Select(e => (e, e.Get<UnitHealth>()))
            .ToList()
            .ForEach(tup => 
            {
                float dist = tup.Item1.transform.position.Dist2D(attackLocation);
                float damage = (1.0f - (dist / BlastRadius)) * 0.5f + 0.5f;
                tup.Item2.TakeDamage((int)(Damage * damage), attackLocation);
                if (tup.Item1.TryGet(out Movement Move))
                {
                    Move.AddForce((tup.Item1.transform.position - attackLocation).normalized * KnockbackForce * damage);
                }
            });
        ObjectPool.Instance.GetInstance(ExplosionEffect).transform.position = attackLocation;
    }
}
