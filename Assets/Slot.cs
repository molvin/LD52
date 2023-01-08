using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{

    public bool Spin;
    public float SpinSpeed;
    public float Smoothing;
    public float EndSmoothing;
    public Image[] Images;
    public float Lowest;
    public float CardSize;
    public float Buffer;
    public float MinVelocity;

    public int CurrentImage;
    public float velocity;
    public bool Stopped;

    private float acceleration;
    private bool[] stopped = { false, false, false };

    private void Awake()
    {
        CurrentImage = 1;
        float start = CardSize + Buffer;
        for(int i = 0; i < 3; i++)
        {
            Images[i].rectTransform.anchoredPosition = new Vector2(0, start - (Buffer + CardSize) * i);
        }
    }

    public void Update()
    {
        velocity = Mathf.SmoothDamp(velocity, Spin ? SpinSpeed : 0.0f, ref acceleration, Smoothing);
        float[] targetY = { CardSize + Buffer, 0, -(CardSize + Buffer)};
        for(int i = 0; i < 3; i++)
        {

            float x = Images[i].rectTransform.anchoredPosition.x;
            float y = Images[i].rectTransform.anchoredPosition.y;

            if(velocity < MinVelocity)
            {
                int index = ((i - CurrentImage) + 1) % 3;
                if (index == -1)
                    index = 2;
                y = Mathf.SmoothDamp(y, targetY[index], ref acceleration, EndSmoothing);
                if (Mathf.Abs(y - targetY[index]) < 0.001f)
                    stopped[i] = true;
                else
                    stopped[i] = false;

            }
            else
            {
                stopped[i] = false;

                y -= velocity * Time.deltaTime;
            }

            if (y < Lowest)
            {
                y = Images[CurrentImage].rectTransform.position.y + Buffer + CardSize;
                CurrentImage--;
                if (CurrentImage == -1)
                    CurrentImage = 2;
            }

            Images[i].rectTransform.anchoredPosition = new Vector2(x, y);
        }

        Stopped = false;
        if(velocity < MinVelocity)
        {
            Stopped = true;
            for(int i = 0; i < 3; i++)
            {
                if (!stopped[i])
                    Stopped = false;
            }
        }
    }
}
