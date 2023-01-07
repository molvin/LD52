using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    protected Entity Entity;
    void Awake()
    {
        Entity = GetComponent<Entity>();
    }
}
