using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitHealth : UnitBase
{
    public int Max;
    public int Current;
    public HealthBar healthBar;

      [Header("Audio")]
    public AudioClip DeathSound;

    public void TakeDamage(int dmg)
    {
        if (Current == 0)
            return;
        Current = Mathf.Max(Current - dmg, 0);
        healthBar?.UnitTakeDamage(dmg);
        if (Current == 0)
        {
            AudioManager.Instance.PlayAudio(DeathSound, transform.position);
            GameManager.Instance?.KillUnit(Entity);
        }
    }
}
