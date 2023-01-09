using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbFieldOfViewMesh : MonoBehaviour
{

    public float viewRadius = 35;
    Mesh viewMesh;
    public MeshFilter viewMeshFilter;

    private FieldOfViewDataGenerator generator;
    public int frameInterval = 4;
    private int frameOfset;
    public float cleanDotArc = 0.01f;

    public void Start()
    {
        generator = FindObjectOfType<FieldOfViewDataGenerator>();
        viewMesh = new Mesh();

        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
        frameOfset = Random.Range(0, frameInterval);
    }

    public void onNewLevel()
    {
        generator = FindObjectOfType<FieldOfViewDataGenerator>();
    }

    public void Update()
    {
        if ((Time.frameCount + frameOfset) % frameInterval == 0)
            buildMesh();
      
    }

    //[ContextMenu("generateMesh")]
    public void buildMesh()
    {
        

        List<Vector3> viewPoints = getViewPoints();
        dotCleanPointCloud(ref viewPoints);
        Vector3[] vertices = new Vector3[viewPoints.Count + 1];
        Vector2[] uvs = new Vector2[viewPoints.Count + 1];
        int[] triangles = new int[(viewPoints.Count) * 3];

        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f);
        for (int i = 0; i < viewPoints.Count; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);


            float x = ((vertices[i + 1].x / viewRadius) / 2) + 0.5f;
            float y = ((vertices[i + 1].z / viewRadius) / 2) + 0.5f;

            uvs[i + 1] = new Vector2(x, y);
            if (i < viewPoints.Count - 1)
            {
                triangles[(i) * 3] = 0;
                triangles[(i) * 3 + 1] = i + 1;
                triangles[(i) * 3 + 2] = i + 2;
            }
        }




        viewMesh.Clear();


        viewMesh.vertices = vertices;
        viewMesh.uv = uvs;
        viewMesh.triangles = triangles;
        //viewMesh.SetUVs(0, uvs);


       // viewMesh.RecalculateNormals();
        //viewMesh.RecalculateUVDistributionMetrics();


    }

    void dotCleanPointCloud(ref List<Vector3> data)
    {
        for (int i = data.Count - 2; i > 2; i--)
        {
            Vector3 left = data[i + 1] - data[i];
            Vector3 right = data[i] - data[i - 1];
            float dot = Vector3.Dot(left.normalized, right.normalized);
            if (((dot + 1f) / 2f) > (1f - cleanDotArc))
                data.RemoveAt(i);
        }
    }

    public List<Vector3> getViewPoints()
    {
        List<float> disntances = generator.getPoints(transform.position);
        List<Vector3> potins = new List<Vector3>();
        for(int i = 0; i < disntances.Count; i++)
        {
            Vector3 dir = DirFromAngle(i, true);
            potins.Add(this.transform.position + dir * Mathf.Clamp(disntances[i], 0, viewRadius));
        }

        return potins;
    }


    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
