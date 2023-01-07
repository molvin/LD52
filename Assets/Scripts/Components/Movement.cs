using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Movement : MonoBehaviour
{
    public bool DebugDraw = false;        
    private NavMeshAgent Agent;
    private Color DebugColor;

    void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        if (DebugDraw)
        {
            Random.InitState(GetHashCode());
            DebugColor = Random.ColorHSV();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DebugDraw)
        {
            Vector3[] corners = Agent.path.corners;
            Debug.Log(corners.Length);
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

    public void MoveTo(Vector3 Destination)
    {
        Agent.destination = Destination;
    }

    public Vector3 GetDestination() => Agent.destination;

}
