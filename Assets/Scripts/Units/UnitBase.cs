using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    protected Entity Entity;
    protected void Awake()
    {
        Entity = GetComponent<Entity>();
        Entity.Add(this);
    }
}
