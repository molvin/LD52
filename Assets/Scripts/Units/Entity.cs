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
    public static int IdCounter = 0;
    public int Id;
    public Team Team;
    Dictionary<System.Type, UnitBase> Components = new Dictionary<System.Type, UnitBase>();
    public bool isSeenByPlayer;

    public bool Has<T>() where T : UnitBase => Components.ContainsKey(typeof(T));
    public T Get<T>() where T : UnitBase => (T)Components[typeof(T)];
    public void Add(UnitBase Component) => Components.Add(Component.GetType(), Component);
    public bool TryGet<T>(out T Component) where T : UnitBase
    {
        Component = null;
        if (Has<T>())
        {
            Component = Get<T>();
        }
        return Component != null;
    }

    public void Awake()
    {
        Id = IdCounter++;
    }
    
    public void Update()
    {
        if (Team == Team.Enemy)
        {
            Vector3 p = transform.position;
            if (p.x < -41.9)
                p.x = -41.9f;
            if (p.x > 41.9f)
                p.x = 41.9f;
            if (p.z < -21.9f)
                p.z = -21.9f;
            if (p.z > 21.9f)
                p.z = 21.9f;
            transform.position = p;

            //if (transform.position.x < -42.2 || transform.position.x > 42.2 || transform.position.z < -22.2 || transform.position.z > 22.2)
            //{
                //UnitHealth Health = Get<UnitHealth>();
                //Health.TakeDamage(500000, Vector3.zero);
            //}
        }
    }
}
