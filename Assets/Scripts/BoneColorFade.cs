using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneColorFade : MonoBehaviour
{
    //public GameObject Parent;
    public float Lifetime;
    private Vector3 initialPosition;
    private Rigidbody ridgidBod;
    private bool fading;
    private float startTime;

    private void Awake()
    {
        ridgidBod = GetComponent<Rigidbody>();
        initialPosition = transform.position;
        gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        startTime = Time.time;    
    }

    void FixedUpdate()
    {
        if((Time.time - startTime) > 0.25f && ridgidBod.velocity.magnitude <= float.Epsilon && !fading)
            StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        fading = true;
        ridgidBod.isKinematic = true;
        float time = 0;
        SpriteRenderer sp = GetComponent<SpriteRenderer>();
        Color start = sp.color;
        Color end = sp.color * 0.5f;
        end.a = 1f;
        while (time < Lifetime)
        {
            time += Time.deltaTime;
            float factor = time / Lifetime;
            sp.color = Color.Lerp(start, end, factor);
            yield return null;

        }
        yield return null;
    }
}
