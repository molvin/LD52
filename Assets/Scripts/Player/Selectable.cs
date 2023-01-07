using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{   
    public int Type;
    public bool Selected;
    public Vector3 TargetPosition;
    public float Spacing;
    public bool DebugMove;

    public SpriteRenderer SelectedCircle;

    private void Awake()
    {
        TargetPosition = transform.position;
    }

    private void Update()
    {
        SelectedCircle.enabled = Selected;
        if(DebugMove)
        {
            Vector3 vel = Vector3.zero;
            Vector3 target = TargetPosition;
            target.y = transform.position.y;
            transform.position = Vector3.SmoothDamp(transform.position, target, ref vel, 0.1f);
        }
    }
}