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
}
