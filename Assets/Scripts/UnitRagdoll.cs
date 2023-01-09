using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody[] Bones;
    public float force;
    public SpriteRenderer unitSprite;

    private void Start()
    {
        Bones = transform.GetComponentsInChildren<Rigidbody>();
        SetColor();     
    }

    public void Explode(float force, Vector3 hitPosition)
    {
        for (int  i = 0; i < Bones.Length; i++)
        {
            Bones[i].gameObject.SetActive(true);
            Bones[i].AddExplosionForce(force, hitPosition, 10f);
        }
          
    }

    private void SetColor()
    {
        Color color = unitSprite.color;
        foreach(Rigidbody rb in Bones)
        {
            SpriteRenderer sp = rb.gameObject.GetComponent<SpriteRenderer>();
            if(sp != null)
                sp.color = color;
        }
    }
}
