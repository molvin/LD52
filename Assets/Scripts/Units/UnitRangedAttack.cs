using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRangedAttack : UnitAttack
{
    [Header("Projectile")]
    public Projectile ProjectilePrefab;
    public int ProjectileCount = 1;
    public float ProjectileSpreadAngle;
    public bool Ricocheting, Pierce, ExplodingOnImpact, Split;
    public int PierceTargetCount;
    public float ProjectileSize = 1.0f;
    public float ProjectileSpeed, ProjectileLifetime, ImpactExplosionRadius;

    protected override void Attack(Entity entity)
    {
        Vector3 Dir = (entity.transform.position - transform.position).normalized;
        if (ProjectileCount == 1)
        {
            Fire(Dir);
        }
        else
        {
            Vector3 StartDir = Vector3.RotateTowards(Dir, new Vector3(Dir.y, 0.0f, -Dir.x), Mathf.Deg2Rad * ProjectileSpreadAngle, 1);
            Fire(StartDir);
            for (int i = 1; i < ProjectileCount; i++)
            {
                StartDir = Vector3.RotateTowards(StartDir, -new Vector3(Dir.y, 0.0f, -Dir.x), Mathf.Deg2Rad * ProjectileSpreadAngle * 2.0f / (ProjectileCount - 1), 1);
                Fire(StartDir);
            }
        }
        AudioManager.Instance.PlayAudio(AttackSound, transform.position);

        void Fire(Vector3 Direction)
        {
            Projectile projectile = ObjectPool.Instance.GetInstance(ProjectilePrefab.gameObject).GetComponent<Projectile>();

            Color color = Color.white;
            if (Entity.TryGet(out UnitName Name))
            {
                color = Name.Color;
            }

            projectile.Fire(
                ProjectilePrefab.gameObject,
                transform.position,
                Direction,
                this.Entity,
                Damage,
                Ricocheting,
                Pierce,
                ExplodingOnImpact,
                Split,
                PierceTargetCount,
                ProjectileSpeed,
                ProjectileSize,
                ProjectileLifetime,
                ImpactExplosionRadius,
                KnockbackForce,
                color,
                new List<Entity>());
            }
    }
}
