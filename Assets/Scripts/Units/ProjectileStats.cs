using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileStats : MonoBehaviour
{
    public bool Ricocheting, Pierce;
    public int PierceTargetCount;
    public float ProjectileSpeed, ProjectileSize, ProjectileLifetime;
    public GameObject ProjectilePrefab;
}
