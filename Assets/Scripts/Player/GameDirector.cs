using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameDirector : MonoBehaviour
{
    private static GameDirector Instance;
    private HashSet<Entity> PlayerEntities;
    private HashSet<Entity> EnemyEntities;

    void Awake()
    {
        if (Instance != null)
            Debug.LogError("There's already a GameDirector present, Niklas!");

        Instance = this;
    }

    public static bool IsOnPlayerTeam(Entity Entity) => Instance.PlayerEntities.Contains(Entity);
    public static List<Entity> GetEnemies(Entity Entity) => (IsOnPlayerTeam(Entity) ? Instance.EnemyEntities : Instance.PlayerEntities).ToList();
}
