using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpacePressDamage : MonoBehaviour
{
    public UnitHealth healthComponent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            healthComponent.TakeDamage(Random.Range(3, 10), transform.position);
        }
    }
}
