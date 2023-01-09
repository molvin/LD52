using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class RemoveAfterTime : MonoBehaviour
{
    public float LifeTime = 1.0f;
    public float AlphaTime;

    private float DisableTime;
    private float startTime;
    public SpriteRenderer Renderer;

    void OnEnable()
    {
        startTime = Time.time;
        DisableTime = Time.time + LifeTime; 
    }

    void Update()
    {
        if (Time.time > DisableTime)
        {
            ObjectPool.Instance.ReturnInstance(gameObject);
        }

        float alpha = Mathf.Clamp01((Time.time - startTime) / AlphaTime);
        Color c = Renderer.color;
        c.a = alpha;
        Renderer.color = c;
    }
}
