using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG_RandomMove : MonoBehaviour
{
    public float ThresholdRadius = 0.5f;
    private Object[] MovementComps;
    void Awake()
    {
        MovementComps = Resources.FindObjectsOfTypeAll(typeof(Movement));
    }

    void Start()
    {
        foreach (var Comp in MovementComps)
        {
            MoveToRandomDestination((Movement)Comp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var Comp in MovementComps)
        {
            var MovementComp = (Movement)Comp;
            if (MovementComp.transform.position.Dist2D(MovementComp.GetDestination()) <= ThresholdRadius)
            {
                MoveToRandomDestination(MovementComp);
            }
        }
    }

    void MoveToRandomDestination(Movement Comp)
    {
        Vector3 RandDest() => new Vector3((Random.value - 0.5f) * 10.0f, 0.0f, (Random.value - 0.5f) * 10.0f);

        Vector3 Dest;
        while (Comp.transform.position.Dist2D(Dest = RandDest()) < 7.0f);
        Comp.FindPath(Dest);
    }
}
