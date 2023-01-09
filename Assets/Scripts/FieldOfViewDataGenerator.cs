using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

public class FieldOfViewDataGenerator : MonoBehaviour
{
    public FieldOfViewData fieldOfViewData;
    public float stepSize;
    public LayerMask obstacleLayer;

    public int resY = 108;
    public int resX = 192;
    public float stepY = -0.5f;
    public float stepX = 0.5f;

    public void Awake()
    {
        fieldOfViewData.Rebuild((resX, resY, Mathf.FloorToInt(360 / stepSize)));
    }

#if UNITY_EDITOR

    [ContextMenu("generateData")]
    private void generateData()
    {
        List<float>[,] data = new List<float>[resX, resY];
        List<List<List<float>>> data2 = new List<List<List<float>>>();
        Vector3 start = this.transform.position;

        

        for(int i = 0; i < resX; i++)
        {
            data2.Add(new List<List<float>>());
            for (int y = 0; y < resY; y++)
            {
                data2[i].Add(generatePoints(start + new Vector3(i * stepX, 0, y * stepY)));
                //data[i, y] = ;
            }
        }
        EditorUtility.SetDirty(fieldOfViewData);
        //fieldOfViewData.set
        fieldOfViewData.data_2 = data2;
        fieldOfViewData.Flatten();
        EditorUtility.SetDirty(fieldOfViewData);
        

    }
#endif
    private List<float> generatePoints(Vector3 origo)
    {
        int angleCount = Mathf.FloorToInt(360 / stepSize);

        List<float> distances = new List<float>(angleCount);
        for (int i = 0; i < angleCount; i++)
        {
            float angle = i * stepSize; //floor or ceil?
            RaycastHit hit;
            Physics.Raycast(origo, DirFromAngle(angle, true), out hit, 200, obstacleLayer);

            distances.Add(hit.collider == null ? 200 : hit.distance);
        }
            

        return distances;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }


    public List<float> getPoints(Vector3 pos)
    {
        Vector3 a = pos - transform.position;
        Vector3 b = new Vector3(a.x / stepX, 1, a.z / stepY);

        int x = (int)b.x;
        int y = (int)b.z;
        if (x < 0 || x > resX-1)
            return null;
        if (y < 0 || y > resY-1)
            return null;

        return fieldOfViewData.data_2[x][y];
    }
}
