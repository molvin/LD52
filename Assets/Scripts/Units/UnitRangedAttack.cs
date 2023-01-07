using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRangedAttack : UnitAttack
{
    public Projectile ProjectilePrefab;
    public bool Ricocheting, Pierce, ExplodingOnImpact;
    public int PierceTargetCount;
    public float ProjectileSize = 1.0f;
    public float ProjectileSpeed, ProjectileLifetime, ImpactExplosionRadius;

    protected override void Attack(Entity entity)
    {
         Projectile projectile = ObjectPool.Instance.GetInstance(ProjectilePrefab.gameObject).GetComponent<Projectile>();

         projectile.Fire(
            transform.position,
            (entity.transform.position - transform.position).normalized,
            this.Entity,
            Damage,
            Ricocheting,
            Pierce,
            ExplodingOnImpact,
            PierceTargetCount,
            ProjectileSpeed,
            ProjectileSize,
            ProjectileLifetime,
            ImpactExplosionRadius);
    }
}