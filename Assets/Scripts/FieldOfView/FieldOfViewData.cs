using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class FieldOfViewData : ScriptableObject
{
    [SerializeField]
    public List<float>[,] data;
    [SerializeField]
    public List<List<List<float>>> data_2;

    public List<float> serialized_data;

    public void Flatten()
    {
        serialized_data.Clear();
        foreach(var l in data_2)
        {
            foreach(var l2 in l)
            {
                serialized_data.AddRange(l2);
            }
        }
    }
    public void Rebuild((int, int, int) shape)
    {
        int i = 0;
        data_2 = new List<List<List<float>>>();
        for(int x = 0; x < shape.Item1; x++)
        {
            data_2.Add(new List<List<float>>());
            for (int y = 0; y < shape.Item2; y++)
            {
                data_2[x].Add(new List<float>());
                for (int z = 0; z < shape.Item3; z++)
                {
                    float d = serialized_data[i++];
                    data_2[x][y].Add(d);
                }
            }
        }
    }
}
