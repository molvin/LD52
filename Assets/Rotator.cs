using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rotator : MonoBehaviour
{
    private Movement unitMovement;
    private UnitAttack unitAttack;
    public Transform Root;    
    public Transform Stretcher;
    public float MinVelocity = 1.0f;
    public float RotationSpeed = 10.0f;

    public float MinStretch;
    public float MaxStretch;
    public float MinAcceleration;
    public float MaxAcceleration;
    public float Acceleration;
    public float Stretch;
    public float StretchSmoothing = 0.1f;
    public float AccelerationSmoothing = 0.1f;
    private float stretchChange;
    private float accelerationChange;

    private void Start()
    {
        unitMovement = GetComponent<Movement>();
        unitAttack = GetComponent<UnitAttack>();
    }

    private void Update()
    {
        if (unitAttack && unitAttack.Target)
        {
            Vector3 toTarget = (unitAttack.Target.transform.position - transform.position).normalized;
            toTarget.y = 0.0f;
            Root.localRotation = Quaternion.RotateTowards(
                    Root.localRotation,
                    Quaternion.LookRotation(toTarget),
                    RotationSpeed * Time.deltaTime
                );
        }
        else if (unitMovement)
        {
            Vector3 velocity = unitMovement.velocity;
            if (velocity.magnitude > MinVelocity)
            {
                Root.localRotation = Quaternion.RotateTowards(
                    Root.localRotation,
                    Quaternion.LookRotation(velocity.normalized),
                    RotationSpeed * Time.deltaTime
                );
            }
 
        }

        if(unitMovement)
        {
            Acceleration += unitMovement.CurrentAcceleration;
            Acceleration = Mathf.SmoothDamp(Acceleration, 0.0f, ref accelerationChange, AccelerationSmoothing);
            Stretch = Mathf.SmoothDamp(Stretch, Acceleration, ref stretchChange, StretchSmoothing);

            Vector3 scale = Vector3.one;
            float factor = Mathf.Clamp01((Stretch + MaxAcceleration) / (MaxAcceleration * 2));
            scale.z = Mathf.Lerp(MinStretch, MaxStretch, factor);
            Stretcher.localScale = scale;
        }
    }
}
