using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float OpenTime = 1.0f;
    public float CloseTime = 0.3f;
    public Animator Anim;


    public Transform SpawnPoint;
    public Transform TargetPoint;

    public enum Type
    {
        Entry,
        Harvest,
        Exit
    }
    public Type DoorType;

    public IEnumerator Toggle(bool open)
    {
        float time = open ? OpenTime : CloseTime;

        Anim.SetBool("Open", open);
        Anim.SetBool("Close", !open);

        yield return new WaitForSeconds(time);

    }
}
