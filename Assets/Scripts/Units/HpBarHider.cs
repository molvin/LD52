using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HpBarHider : MonoBehaviour
{

    public GameObject target_gameObject;
    public int radius = 35;
    public LayerMask obstacleLayer;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        bool isSeenByPlayer = canPlayerSeeMe();
     
        if (target_gameObject)
            target_gameObject.SetActive(isSeenByPlayer);

        this.GetComponent<Entity>().isSeenByPlayer = isSeenByPlayer;

    }

    public bool canPlayerSeeMe()
    {

        List<Entity> players = GameManager.Instance.EntitiesInGame
           .Where(e => e.Team == Team.Player)
           .Where(e => e.Has<UnitHealth>())
           .OrderBy(e => transform.position.Dist2D(e.transform.position))
           .ToList();

        foreach (Entity player in players)
        {
            float Distance = transform.position.Dist2D(player.transform.position);
            if (Distance > radius)
                continue;

            RaycastHit hit;
            Physics.Raycast(
                transform.position,
                (player.transform.position - transform.position).normalized,
                out hit,
                Distance, obstacleLayer);
            
            if(hit.collider == null)
            {
                return true;
            } 
        }
    
        return false;
    }
}
