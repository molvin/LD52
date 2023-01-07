using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class FieldOfView : MonoBehaviour
{
    public List<Transform> friendlies;
    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public float viewRadius = 10;

    [HideInInspector]
    public List<Transform> visibleTargets = new List<Transform>();


    public float meshResolution;
    public int edgeResolveIterations;
    public float edgeDstThreshold;

    public MeshFilter viewMeshFilter;
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

    List<(Transform, List<Vector3>)> getViewPoints()
    {
        List<(Transform, List<Vector3>)> viewPoints = new List<(Transform, List<Vector3>)>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        foreach (Transform friendly in friendlies) {
            List<Vector3> vp = new List<Vector3>();
            for (int i = 0; i <= 365; i++)
            {
                ViewCastInfo newViewCast = ViewCast(i, friendly);

                if (i > 0)
                {
                    bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                    if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                    {
                        EdgeInfo edge = FindEdge(oldViewCast, newViewCast, friendly);
                        if (edge.pointA != Vector3.zero)
                        {
                            vp.Add(edge.pointA);
                        }
                        if (edge.pointB != Vector3.zero)
                        {
                            vp.Add(edge.pointB);
                        }
                    }

                }


                vp.Add(newViewCast.point);
                oldViewCast = newViewCast;
            }
            viewPoints.Add((friendly, vp));
        }
        return viewPoints;

    }

    void DrawFieldOfView()
    {

        List<(Transform, List<Vector3>)> viewPoints = getViewPoints();

        int outerVertexCount = viewPoints.Sum(e => e.Item2.Count);

        Vector3[] vertices = new Vector3[outerVertexCount + viewPoints.Count ];
        int[] triangles = new int[(outerVertexCount) * 3];
        int globI = 0;
        foreach ((Transform, List<Vector3>) p in viewPoints)
        {
            vertices[globI] = p.Item1.transform.position;
            for(int i = 0; i < p.Item2.Count; i++)
            {
                
                vertices[globI + i + 1] = p.Item2[i];
                if(i < p.Item2.Count -1)
                {
                    triangles[(globI + i) * 3] = globI;
                    triangles[(globI + i) * 3 + 1] = globI + i + 1;
                    triangles[(globI + i) * 3 + 2] = globI + i + 2;

                } 
            }

            globI += p.Item2.Count ;

        }

        viewMesh.Clear();


        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();

        /**
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 2; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 4] = 0;
                triangles[i * 4 + 1] = i + 1;
                triangles[i * 4 + 2] = i + 2;
                triangles[i * 4 + 3] = i + 3;

            }
        }
        */
       
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
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
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
