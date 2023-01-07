using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameDirector : MonoBehaviour
{
    public static GameDirector Instance;
    public List<Entity> EntitiesInGame = new List<Entity>();

    void Awake()
    {
        if (Instance != null)
            Debug.LogError("There's already a GameDirector present, Niklas!");

        Instance = this;

        EntitiesInGame = Resources.FindObjectsOfTypeAll<Entity>().ToList();
    }
}
