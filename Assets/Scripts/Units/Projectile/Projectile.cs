using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask LayerMask;
    public GameObject AoeEffect;
    bool Ricocheting, Pierce, ExplodingOnImpact, Split;
    int PierceTargetCount;
    float ProjectileSpeed, ImpactExplosionRadius, ProjectileLifeTime;
    float ProjectileSize;
    int Damage;

    Entity Owner;
    float EndTime;
    BoxCollider Collider;
    GameObject Prefab;

    void Awake()
    {
        Collider = GetComponentInChildren<BoxCollider>();
    }

    public void Fire(
        GameObject prefab,
        Vector3 Start,
        Vector3 Direction,
        Entity owner,
        int damage,
        bool ricocheting,
        bool pierce,
        bool explodingOnImpact,
        bool split,
        int pierceTargetCount,
        float projectileSpeed,
        float projectileSize,
        float projectileLifetime,
        float impactExplosionRadius
        )
    {
        Prefab = prefab;
        transform.position = Start;
        transform.forward = Direction;
        ProjectileSize = projectileSize;
        Owner = owner;

        Damage                = damage;
        Ricocheting           = ricocheting;
        Pierce                = pierce;
        ExplodingOnImpact     = explodingOnImpact;
        Split                 = split;
        PierceTargetCount     = pierceTargetCount;
        ProjectileSpeed       = projectileSpeed;
        ImpactExplosionRadius = impactExplosionRadius;

        ProjectileLifeTime = projectileLifetime;
        EndTime = Time.time + projectileLifetime;
    }

    void Update()
    {
        if (Time.time >= EndTime)
        {
            ObjectPool.Instance.ReturnInstance(gameObject);
        }

        bool IsDone = false;
        float? HitEnemyRadius = null;
        Vector3? ExplosionPoint = null;

        RaycastHit[] Hits = Physics.SphereCastAll(transform.position, ProjectileSize * 0.5f, transform.forward, ProjectileSpeed * Time.deltaTime * 1.5f, LayerMask);
        foreach (RaycastHit Hit in Hits)
        {
            var Entity = Hit.transform.GetComponent<Entity>();
            if (!Entity)
                Entity = Hit.transform.GetComponentInChildren<Entity>();
            if (!Entity)
                Entity = Hit.transform.GetComponentInParent<Entity>();

            if (Entity == null || Entity.Team != Owner.Team)
            {
                if (ExplodingOnImpact)
                {
                    ExplosionPoint = Hit.point;
                }
                else if (Entity && Entity.TryGet(out UnitHealth Health))
                {
                    Health.TakeDamage(Damage);
                }

                if (Entity)
                {
                    if (Entity.TryGet(out Movement MoveComp))
                        HitEnemyRadius = HitEnemyRadius != null ? Mathf.Max(HitEnemyRadius.Value, MoveComp.CollisionRadius) : MoveComp.CollisionRadius;
                    else
                        HitEnemyRadius = 0.0f;
                }

                IsDone = true;
            }
        }

        if (ExplosionPoint != null)
        {
            GameManager.Instance.EntitiesInGame
                .Where(e => e.Team != Owner.Team && e.Has<UnitHealth>() && e.transform.position.Dist2D(ExplosionPoint.Value) < ImpactExplosionRadius)
                .Where(e =>
                {
                    Vector3 Direction = e.transform.position - transform.position;
                    return Physics.Raycast(transform.position, Direction.normalized, out RaycastHit Hit, Direction.magnitude, LayerMask) && Hit.transform == e.transform;
                })
                .Select(e =>  e.Get<UnitHealth>())
                .ToList()
                .ForEach(h => h.TakeDamage(Damage));

            // Explosion effect
            GameObject Aoe = ObjectPool.Instance.GetInstance(AoeEffect);
            Vector3 pos = transform.position;
            Aoe.transform.position = new Vector3(pos.x, 0.1f, pos.z);
            Aoe.transform.localScale = Vector3.one * ImpactExplosionRadius;
        }

        transform.position += transform.forward * ProjectileSpeed * Time.deltaTime;

        if (HitEnemyRadius != null && Split)
        {
            Vector3 FirstDir = (transform.forward + transform.right).normalized;
            Vector3 SecondDir = (transform.forward - transform.right).normalized;

            Fire(FirstDir);
            Fire(SecondDir);

            void Fire(Vector3 Direction)
            {
                Vector3 SpawnPos = transform.position + Direction * (ProjectileSize + HitEnemyRadius.Value) * 1.5f;

                Projectile projectile = ObjectPool.Instance.GetInstance(Prefab).GetComponent<Projectile>();
                projectile.Fire(
                     Prefab,
                     SpawnPos,
                     Direction,
                     Owner,
                     Damage,
                     Ricocheting,
                     Pierce,
                     ExplodingOnImpact,
                     false,
                     PierceTargetCount,
                     ProjectileSpeed,
                     ProjectileSize,
                     ProjectileLifeTime,
                     ImpactExplosionRadius);
            }
        }

        if (IsDone)
        {
            ObjectPool.Instance.ReturnInstance(gameObject);
        }
    }
}
