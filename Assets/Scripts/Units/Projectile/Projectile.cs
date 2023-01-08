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
    float KnockbackForce;
    int Damage;

    Entity Owner;
    float EndTime;
    BoxCollider Collider;
    GameObject Prefab;
    List<Entity> IgnoreTargets;

    bool BeingRemoved = false;

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
        float impactExplosionRadius,
        float knockbackForce,
        List<Entity> ignoreTargets
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
        KnockbackForce        = knockbackForce;

        ProjectileLifeTime = projectileLifetime;
        EndTime = Time.time + projectileLifetime;

        IgnoreTargets = ignoreTargets;
        BeingRemoved = false;
    }

    void Update()
    {
        if (BeingRemoved)
            return;

        if (Time.time >= EndTime)
        {
            if (ExplodingOnImpact)
            {
                Explode(transform.position);
            }
            Remove();
        }

        //bool IsDone = false;
        //float? HitEnemyRadius = null;
        Entity HitEntity = null;
        Vector3? HitPoint = null;
        Vector3 HitNormal = Vector3.zero;

        RaycastHit[] Hits = Physics.SphereCastAll(transform.position, ProjectileSize * 0.5f, transform.forward, ProjectileSpeed * Time.deltaTime * 1.5f, LayerMask);
        foreach (RaycastHit Hit in Hits)
        {
            var Entity = Hit.transform.GetComponent<Entity>();
            if (!Entity)
                Entity = Hit.transform.GetComponentInChildren<Entity>();
            if (!Entity)
                Entity = Hit.transform.GetComponentInParent<Entity>();

            if (Entity && Entity.Team != Owner.Team && !IgnoreTargets.Contains(Entity))
            {
                HitPoint = Hit.point;
                HitEntity = Entity;
                HitNormal = Hit.normal;
                break;

                //if (ExplodingOnImpact)
                //{
                    //ExplosionPoint = Hit.point;
                //}
                //else if (Entity && Entity.TryGet(out UnitHealth Health))
                //{
                    //if (Entity.TryGet(out Movement MoveComp))
                        //HitEnemyRadius = HitEnemyRadius != null ? Mathf.Max(HitEnemyRadius.Value, MoveComp.CollisionRadius) : MoveComp.CollisionRadius;
                    //else
                        //HitEnemyRadius = 0.0f;

                    //Health.TakeDamage(Damage);
                    //IgnoreTargets.Add(Entity);
                //}

                //IsDone = true;
            }
            else if (Entity == null)
            {
                HitPoint = Hit.point;
                HitNormal = Hit.normal;
                //ExplosionPoint = Hit.point;
                //IsDone = true;
            }
        }

        if (ExplodingOnImpact && HitEntity != null && ((Pierce && PierceTargetCount > 0) || Split))
        {
            HitEntity.Get<UnitHealth>().TakeDamage(Damage);
            IgnoreTargets.Add(HitEntity);
        }
        else if (ExplodingOnImpact && HitEntity == null && Ricocheting)
        {

        }
        // Explosion
        else if (ExplodingOnImpact && HitPoint != null)
        {
            Explode(HitPoint.Value);
        }
        // Hit an enemy
        else if (HitEntity != null)
        {
            if (HitEntity.TryGet(out Movement Move))
            {
                Move.AddForce(-HitNormal * KnockbackForce);
            }
            HitEntity.Get<UnitHealth>().TakeDamage(Damage);
            IgnoreTargets.Add(HitEntity);
        }
        // Hit something else
        else if (HitPoint != null)
        {

        }

        // Hit anything
        bool IsDone = HitPoint != null;

        // Pierce & Split enemies
        if (HitEntity != null)
        {
            float HitEnemyRadius = 0;
            if (HitEntity.TryGet(out Movement MoveComp))
                HitEnemyRadius = MoveComp.CollisionRadius;

            float Offset = ProjectileSize + HitEnemyRadius;

            if (Pierce && PierceTargetCount > 0)
            {
                PierceTargetCount--;
                IsDone = false;
            }
            else if (Split)
            {
                SplitProjectile(transform.forward * Offset);
            }
        }
        // Ricochete
        else if (HitPoint != null && Ricocheting)
        {
            IsDone = false;
            transform.forward = transform.forward - 2.0f * Vector3.Dot(transform.forward, HitNormal) * HitNormal; 
            Ricocheting = false;
        }

        transform.position += transform.forward * ProjectileSpeed * Time.deltaTime;

        if (IsDone)
        {
            Remove();
        }
    }

    void Explode(Vector3 Position)
    {
        GameManager.Instance.EntitiesInGame
            .Where(e => e.Team != Owner.Team && e.Has<UnitHealth>() && e.transform.position.Dist2D(Position) < ImpactExplosionRadius)
            .Where(e =>
            {
                Vector3 Direction = e.transform.position - Position;
                return Physics.Raycast(Position, Direction.normalized, out RaycastHit Hit, Direction.magnitude, LayerMask) && Hit.transform == e.transform;
            })
            .Select(e => (e, e.Get<UnitHealth>()))
            .ToList()
            .ForEach(tup => 
            {
                tup.Item2.TakeDamage(Damage);
                if (tup.Item1.TryGet(out Movement Move))
                {
                    Move.AddForce((tup.Item1.transform.position - Position).normalized * KnockbackForce);
                }
            });

        // Explosion effect
        GameObject Aoe = ObjectPool.Instance.GetInstance(AoeEffect);
        Vector3 pos = Position;
        Aoe.transform.position = new Vector3(pos.x, 0.1f, pos.z);
        Aoe.transform.localScale = Vector3.one * ImpactExplosionRadius;
    }

    void Remove()
    {
        BeingRemoved = true;
        StartCoroutine(Removal());
    }
    IEnumerator Removal()
    {
        yield return new WaitForSeconds(0.2f);
        ObjectPool.Instance.ReturnInstance(gameObject);
    }

    void SplitProjectile(Vector3 Offset)
    {
        Vector3 FirstDir = (transform.forward + transform.right).normalized;
        Vector3 SecondDir = (transform.forward - transform.right).normalized;

        Fire(FirstDir);
        Fire(SecondDir);

        void Fire(Vector3 Direction)
        {
            Vector3 SpawnPos = transform.position + Offset;

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
                 ImpactExplosionRadius,
                 KnockbackForce,
                 IgnoreTargets);
        }
    }
}
