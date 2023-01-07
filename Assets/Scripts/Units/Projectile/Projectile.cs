using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    bool Ricocheting, Pierce, ExplodingOnImpact;
    int PierceTargetCount;
    float ProjectileSpeed, ImpactExplosionRadius;
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
        transform.localScale = Vector3.one * projectileSize;
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
        RaycastHit Hit;
        if (Physics.Raycast(transform.position, transform.forward, out Hit, ProjectileSpeed * Time.deltaTime * 1.5f))
        {
            Debug.Log(Hit.transform);
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

                var Health = GetComponent<UnitHealth>();
                if (Health)
                {
                    Health.Helath -= Damage;
                }

                IsDone = true;
            }
        }

        transform.position += transform.forward * ProjectileSpeed * Time.deltaTime;

        if (IsDone)
        {
            Debug.Log("Pew");
            ObjectPool.Instance.ReturnInstance(gameObject);
        }
    }
}
