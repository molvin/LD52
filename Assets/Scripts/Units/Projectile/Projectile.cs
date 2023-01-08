using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask LayerMask;
    bool Ricocheting, Pierce, ExplodingOnImpact;
    int PierceTargetCount;
    float ProjectileSpeed, ImpactExplosionRadius;
    float ProjectileSize;
    int Damage;

    Entity Owner;
    float EndTime;
    BoxCollider Collider;

    void Awake()
    {
        Collider = GetComponentInChildren<BoxCollider>();
    }

    public void Fire(
        Vector3 Start,
        Vector3 Direction,
        Entity owner,
        int damage,
        bool ricocheting,
        bool pierce,
        bool explodingOnImpact,
        int pierceTargetCount,
        float projectileSpeed,
        float projectileSize,
        float projectileLifetime,
        float impactExplosionRadius
        )
    {
        transform.position = Start;
        transform.forward = Direction;
        ProjectileSize = projectileSize;
        Owner = owner;

        Damage                = damage;
        Ricocheting           = ricocheting;
        Pierce                = pierce;
        ExplodingOnImpact     = explodingOnImpact;
        PierceTargetCount     = pierceTargetCount;
        ProjectileSpeed       = projectileSpeed;
        ImpactExplosionRadius = impactExplosionRadius;

        EndTime = Time.time + projectileLifetime;
    }

    void Update()
    {
        if (Time.time >= EndTime)
        {
            ObjectPool.Instance.ReturnInstance(gameObject);
        }

        bool IsDone = false;
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
                    Debug.LogWarning("Projectile should explode!");
                }

                if (Entity && Entity.TryGet(out UnitHealth Health))
                {
                    Health.TakeDamage(Damage);
                }

                IsDone = true;
            }
        }

        transform.position += transform.forward * ProjectileSpeed * Time.deltaTime;

        if (IsDone)
        {
            ObjectPool.Instance.ReturnInstance(gameObject);
        }
    }
}
