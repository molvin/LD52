using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    Dictionary<GameObject, List<GameObject>> Pools = new Dictionary<GameObject, List<GameObject>>();

    void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;
    }

    public GameObject GetInstance(GameObject Prefab)
    {
        if (!Pools.ContainsKey(Prefab))
            Pools.Add(Prefab, new List<GameObject>());

        var Pool = Pools[Prefab];
        for (int i = 0; i < Pool.Count; i++)
        {
            if (!Pool[i].activeSelf)
            {
                GameObject Object = Pool[i];
                Object.SetActive(true);
                return Object;
            }
        }

        GameObject Instance = GameObject.Instantiate(Prefab);
        Pool.Add(Instance);
        return Instance;
    }

    public void ReturnInstance(GameObject Object)
    {
        var Body = GetComponent<Rigidbody>();
        if (Body)
        {
            Body.velocity = Vector3.zero;
            Body.angularVelocity = Vector3.zero;
        }

        Object.SetActive(false);
    }
    public void Clear()
    {
        Pools.Clear();
    }
}
