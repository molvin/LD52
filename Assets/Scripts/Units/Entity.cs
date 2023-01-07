using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public static int IdCounter = 0;
    public int Id;

    public void Awake()
    {
        Id = IdCounter++;
    }
}
