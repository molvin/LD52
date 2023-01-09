using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class UnitHealth : UnitBase
{
    public int Max;
    public int Current;
    public HealthBar healthBar;
    private UnitRagdoll ragdoll;

      [Header("Audio")]
    public AudioClip DeathSound;



    [Header("AudioController")]
    public bool isAudioControllingUnit = false;
    public bool increasedSound;
    public float amountPerTick = 1;
    public AudioMixer mixer;
    public void Start()
    {
        ragdoll = transform.GetComponentInChildren<UnitRagdoll>();
    }
    public void TakeDamage(int dmg)
    {
        if (Current == 0)
            return;
        Current = Mathf.Max(Current - dmg, 0);
        healthBar?.UnitTakeDamage(dmg);
        if (Current == 0)
        {
            if(ragdoll != null)
                ragdoll.Explode(dmg, transform.position);
            AudioManager.Instance.PlayAudio(DeathSound, transform.position);
            GameManager.Instance?.KillUnit(Entity);
        }

        //Ugly audio stuff
        if (isAudioControllingUnit)
        {
            Current = 100;
            float orig;
            mixer.GetFloat("MasterVolume", out orig);
            mixer.SetFloat("MasterVolume", orig + amountPerTick * (increasedSound ? 1 : -1));

        }
    }
}
