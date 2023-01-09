using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Rotator : MonoBehaviour
{
    private Movement unitMovement;
    private UnitAttack unitAttack;
    public Transform Sprite;    
    public float MinVelocity = 1.0f;
    public float RotationSpeed = 10.0f;

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
            Sprite.localRotation = Quaternion.RotateTowards(
                    Sprite.localRotation,
                    Quaternion.LookRotation(toTarget),
                    RotationSpeed * Time.deltaTime
                );
        }
        else if (unitMovement)
        {
            Vector3 velocity = unitMovement.velocity;
            if (velocity.magnitude > MinVelocity)
            {
                Sprite.localRotation = Quaternion.RotateTowards(
                    Sprite.localRotation,
                    Quaternion.LookRotation(velocity.normalized),
                    RotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
