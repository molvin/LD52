using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Movement : UnitBase
{
    [Header("Collision")]
    public LayerMask ObstacleMask;

    [Header("Collision")]
    public float CollisionRadius = 0.5f;

    [Header("Steering")]
    public float Speed = 2.5f;

    [Header("Steering")]
    public float Acceleration = 16.0f;

    [Header("Steering")]
    public float StoppingDistance = 0.5f;

    public bool DebugDraw = false;        
    private Selectable Selectable = null;
    private Color DebugColor;

    private Vector3 CurrentDestination;
    private int ClearedPathPoint = 0;
    private List<Vector3> CurrentPath = new List<Vector3>();
    private NavMeshPath NavPath;
    const float Y = 1.0f;

    private Vector3 velocity = Vector3.zero;

    new protected void Awake()
    {
        base.Awake();    

        NavPath = new NavMeshPath();
        Selectable = GetComponent<Selectable>();

        if (DebugDraw)
        {
            Random.InitState(GetHashCode());
            DebugColor = Random.ColorHSV();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Selectable && Selectable.TargetPosition.Dist2D(CurrentDestination) >= StoppingDistance)
        {
            FindPath(Selectable.TargetPosition);
        }

        FollowPath();

        if (DebugDraw)
            DrawDebug();
    }

    public void FollowPath()
    {
        if (CurrentPath.Count == 0)
            return;

        // Target path point
        Vector3 Target = CurrentPath[0];
        if (ClearedPathPoint + 1 < CurrentPath.Count)
            Target = CurrentPath[ClearedPathPoint + 1];
        else
            Target = CurrentDestination;

        // Vector to target
        Vector3 ToTarget = Target - transform.position;
        ToTarget.y = 0.0f;
        Vector3 Direction = Vector3.ClampMagnitude(ToTarget, 1);

        // Calculate delta acceleration
        Vector3 Delta = Direction * Speed - velocity;
        Delta = Vector3.ClampMagnitude(Delta, Acceleration * Time.deltaTime);

        // Clamp max speed
        velocity += Delta;
        velocity = Vector3.ClampMagnitude(velocity, Speed);

        MoveWithCollision();

        // Check path
        if (ClearedPathPoint + 1 < CurrentPath.Count && transform.position.Dist2D(CurrentPath[ClearedPathPoint + 1]) <= StoppingDistance)
        {
            ClearedPathPoint++;
        }
    }

    private void MoveWithCollision()
    {
        transform.position += velocity * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, Y, transform.position.z);
    }

    public void FindPath(Vector3 Destination)
    {
        // Try to adjust point
        if (NavMesh.SamplePosition(Destination, out NavMeshHit NavHit, 20.0f, NavMesh.AllAreas))
        {
            Destination = NavHit.position;
        }

        CurrentDestination = Destination;
        Vector3 ToDestination = CurrentDestination - transform.position;

        ClearedPathPoint = 0;
        CurrentPath.Clear();

        if (!Physics.Raycast(transform.position, ToDestination.normalized, out RaycastHit RayHit, ToDestination.magnitude, ObstacleMask))
        {
            CurrentPath.Clear();
            CurrentPath.Add(transform.position);
            CurrentPath.Add(CurrentDestination);
        }
        else if (NavMesh.CalculatePath(transform.position, CurrentDestination, NavMesh.AllAreas, NavPath))
        {
            CurrentPath = NavPath.corners.ToList();
            CurrentPath.Insert(0, transform.position);
        }
        // Just move to the collision point
        else
        {
            CurrentDestination = RayHit.point - ToDestination.normalized * CollisionRadius;

            CurrentPath.Add(transform.position);
            CurrentPath.Add(CurrentDestination);
        }
    }

    public Vector3 GetDestination() => CurrentDestination;

    private void DrawDebug()
    {
        if (CurrentPath.Count <= 1)
        {
            Debug.DrawLine(transform.position, CurrentDestination, DebugColor, Time.deltaTime);
        }
        else
        {
            for (int i = 1; i < CurrentPath.Count; i++)
            {
                Debug.DrawLine(CurrentPath[i - 1], CurrentPath[i], DebugColor, Time.deltaTime);
            }
        }
    }
}
