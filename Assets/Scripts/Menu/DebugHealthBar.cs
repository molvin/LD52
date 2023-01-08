using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DebugHealthBar : MonoBehaviour
{
    public Image HealthBarDebug;
    public GameObject Unit;
    UnitHealth unitHealth;

    void Start()
    {
        unitHealth = Unit.GetComponent<UnitHealth>();
    }

    void Update()
    {
        HealthBarDebug.fillAmount = (float)unitHealth.Current / (float)unitHealth.Max;
    }
}
