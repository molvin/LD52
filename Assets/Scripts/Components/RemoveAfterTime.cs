using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveAfterTime : MonoBehaviour
{
    public float LifeTime = 1.0f;

    private float DisableTime;

    void OnEnable()
    {
        DisableTime = Time.time + LifeTime; 
    }

    void Update()
    {
        if (Time.time > DisableTime)
        {
            ObjectPool.Instance.ReturnInstance(gameObject);
        }
    }
}
