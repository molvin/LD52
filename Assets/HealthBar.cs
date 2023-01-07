using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image Background;
    public Image Fill;
    public bool ShowOnFullHealth;

    private UnitHealth health;

    private void Awake()
    {
        health = GetComponentInParent<UnitHealth>();
    }

    private void Update()
    {
        if(health)
        {
            Background.enabled = Fill.enabled = health.Current < health.Max;
            Fill.fillAmount = (health.Current / (float)health.Max);
        }
    }
}
