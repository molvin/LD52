using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdoll : MonoBehaviour
{
    private Rigidbody[] Bones;
    public float ForceModifier;
    public SpriteRenderer unitSprite;

    private void Start()
    {
        Bones = transform.GetComponentsInChildren<Rigidbody>(true);
        SetColor();     
    }

    public void Explode(float force, Vector3 hitPosition)
    {
        this.transform.SetParent(null, true);
        for (int  i = 0; i < Bones.Length; i++)
        {
            Bones[i].gameObject.SetActive(true);
            Bones[i].AddExplosionForce(force * ForceModifier, hitPosition, 10f);
            // Debug.Log($"Exploding force: {force * ForceModifier} Distance: {(hitPosition - Bones[i].transform.position).magnitude}");
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
