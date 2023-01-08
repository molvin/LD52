using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class FieldOfView : MonoBehaviour
{
    public LayerMask obstacleMask;

    public float viewRadius = 10;

    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;
    public int stepSize = 5;

    public MeshFilter viewMeshFilter;
    public float cleanDotArc = 0.1f;
    Mesh viewMesh;

    void Start()
    {
        viewMesh = new Mesh();
        
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }


 
    void LateUpdate()
    {
        DrawFieldOfView();
    }

    List<Vector3> getViewPoints()
    {
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= 360/ stepSize; i++)
        {
            
            ViewCastInfo newViewCast = ViewCast(i * stepSize, this.transform);

            if (i > 0)
            {
                
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast, this.transform);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }

            }


            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }
        return viewPoints;

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
    void DrawFieldOfView()
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
            if(i < viewPoints.Count - 1)
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
        

        viewMesh.RecalculateNormals();
        viewMesh.RecalculateUVDistributionMetrics();

    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast, Transform transform)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle, transform);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }


    ViewCastInfo ViewCast(float globalAngle, Transform transform)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            if(globalAngle == 1)
                Debug.DrawLine(transform.position, hit.point, Color.green);

            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            if (globalAngle == 1)
                Debug.DrawLine(transform.position, transform.position + dir * viewRadius, Color.green);
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
