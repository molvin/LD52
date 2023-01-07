using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_RandomMove : MonoBehaviour
{
    public float ThresholdRadius = 0.5f;
    private Movement MovementComp;
    void Awake()
    {
        MovementComp = GetComponent<Movement>(); 
    }

    void Start()
    {
        MoveToRandomDestination();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.Dist2D(MovementComp.GetDestination()) <= ThresholdRadius)
        {
            MoveToRandomDestination();
        }
    }

    void MoveToRandomDestination()
    {
        Vector3 RandDest() => new Vector3((Random.value - 0.5f) * 10.0f, 0.0f, (Random.value - 0.5f) * 10.0f);

        Vector3 Dest;
        while (transform.position.Dist2D(Dest = RandDest()) < 7.0f);
        MovementComp.MoveTo(Dest);
    }
}
