using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStats : MonoBehaviour
{
    public bool Ricocheting, Pierce, ExplodingOnImpact;
    public int PierceTargetCount;
    public float ProjectileSpeed, ProjectileSize, ProjectileLifetime, ImpactExplosionRadius;
    public GameObject ProjectilePrefab;
    
}
