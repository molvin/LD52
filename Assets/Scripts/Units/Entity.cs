using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Player,
    Enemy
}

public class Entity : MonoBehaviour
{
    public int Id;
    public Team Team;
    public Dictionary<System.Type, UnitBase> Components = new Dictionary<System.Type, UnitBase>();
}
