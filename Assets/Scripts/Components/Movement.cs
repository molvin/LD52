using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Movement : UnitBase
{
    public LayerMask ObstacleMask;
    public float CollisionRadius = 0.5f;
    public float Mass = 1.0f;
    public float Speed = 2.5f;
    public float Acceleration = 16.0f;

    private Selectable Selectable = null;

    private Vector3 CurrentDestination;
    private int ClearedPathPoint = 0;
    private List<Vector3> CurrentPath = new List<Vector3>();
    private NavMeshPath NavPath;
    const float Y = 1.0f;
    private Vector3 LastSelectableTargetPos;

    private Vector3 velocity = Vector3.zero;

    float StoppingDistance => CollisionRadius * 0.5f;

    [HideInInspector]
    public bool CanMove = true;

    new protected void Awake()
    {
        base.Awake();    

        NavPath = new NavMeshPath();
        Selectable = GetComponent<Selectable>();
    }
    
    public void AddForce(Vector3 Force)
    {
        velocity += Force / Mass;
    }

    // Update is called once per frame
    void Update()
    {
        if (!CanMove)
        {
            return;
        }

        if (Selectable && Selectable.TargetPosition.Dist2D(LastSelectableTargetPos) >= StoppingDistance)
        {
            LastSelectableTargetPos = Selectable.TargetPosition;
            FindPath(Selectable.TargetPosition);
        }

        FollowPath();
        Avoidance();
        MoveWithCollision();
    }

    public void FollowPath()
    {
        if (CurrentPath.Count == 0)
            return;

        // Check path
        if (ClearedPathPoint + 1 < CurrentPath.Count && transform.position.Dist2D(CurrentPath[ClearedPathPoint + 1]) <= StoppingDistance)
        {
            ClearedPathPoint++;
        }

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
        //velocity = Vector3.ClampMagnitude(velocity, Speed);
    }

    private void MoveWithCollision()
    {
        Vector3 MovePosition = transform.position + velocity * Time.deltaTime;

        if (NavMesh.SamplePosition(MovePosition, out NavMeshHit NavHit, CollisionRadius * 2.0f, NavMesh.AllAreas))
        {
            Vector3 delta = MovePosition - NavHit.position;
            // Enough diff to do something
            if (delta.magnitude > CollisionRadius * 0.1f)
            {
                delta.Normalize();
                velocity = (velocity - Vector3.Project(velocity, delta) * Time.deltaTime).normalized * velocity.magnitude;
            }

            transform.position = new Vector3(NavHit.position.x, Y, NavHit.position.z);
        }

        // Collision with other movements
        List<Movement> OtherMovement = GameManager.Instance.EntitiesInGame
            .Where(e => e != Entity && e.Has<Movement>())
            .Select(e => e.Get<Movement>())
            .ToList();
        
        Vector3 LargestPenetration = Vector3.zero;
        foreach (Movement Other in OtherMovement)
        {
            Vector3 Delta = transform.position - Other.transform.position;
            float Penetration = (CollisionRadius + Other.CollisionRadius) - Delta.magnitude;
            if (Penetration > LargestPenetration.magnitude)
            {
                LargestPenetration = Delta.normalized * Penetration;
            }
        }

        transform.position += Vector3.ClampMagnitude(LargestPenetration * Speed * Time.deltaTime, LargestPenetration.magnitude);
    }

    private void Avoidance()
    {

        //List<Movement> OtherMovement = GameManager.Instance.EntitiesInGame
            //.Where(e => e != Entity && e.Has<Movement>())
            //.Select(e => e.Get<Movement>())
            //.ToList();

        
        //foreach (Movement Other in OtherMovement)
        //{
            //Vector3 ToOther = 
            //Vector3 nearest = FindNearestPointOnLine(transform.position, velocity, Other.transform.position);

        //}

        //Vector2 FindNearestPointOnLine(Vector2 origin, Vector2 end, Vector2 point)
        //{
            ////Get heading
            //Vector2 heading = (end - origin);
            //float magnitudeMax = heading.magnitude;
            //heading.Normalize();

            ////Do projection from the point but clamp it
            //Vector2 lhs = point - origin;
            //float dotP = Vector2.Dot(lhs, heading);
            //dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
            //return origin + heading * dotP;
        //}
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

        if (!Physics.SphereCast(transform.position, CollisionRadius, ToDestination.normalized, out RaycastHit RayHit, ToDestination.magnitude, ObstacleMask))
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
}
