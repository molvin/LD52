using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Movement : UnitBase
{
    [Header("Steering")]
    public float Speed = 2.5f;

    [Header("Steering")]
    public float Acceleration = 16.0f;

    [Header("Steering")]
    public float AngularSpeed = 200.0f;

    [Header("Steering")]
    public float StoppingDistance = 0.5f;

    public bool DebugDraw = false;        
    private NavMeshAgent Agent;
    private Selectable Selectable = null;
    private Color DebugColor;

    new protected void Awake()
    {
        base.Awake();    

        Agent = GetComponent<NavMeshAgent>();
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
        { // Update agent
            Agent.speed            = Speed;
            Agent.angularSpeed     = AngularSpeed;
            Agent.acceleration     = Acceleration;
            Agent.stoppingDistance = StoppingDistance;
        }

        if (Selectable && Selectable.TargetPosition.Dist2D(Agent.destination) >= StoppingDistance)
        {
            Agent.destination = Selectable.TargetPosition;
        }

        if (DebugDraw)
            DrawDebug();
    }

    public void MoveTo(Vector3 Destination)
    {
        Agent.destination = Destination;
    }

    public Vector3 GetDestination() => Agent.destination;

    private void DrawDebug()
    {
        Vector3[] corners = Agent.path.corners;
        if (corners.Length <= 1)
        {
            Debug.DrawLine(transform.position, Agent.destination, DebugColor, Time.deltaTime);
        }
        else
        {
            for (int i = 1; i < corners.Length; i++)
            {
                Debug.DrawLine(corners[i - 1], corners[i], DebugColor, Time.deltaTime);
            }
        }
    }
}
