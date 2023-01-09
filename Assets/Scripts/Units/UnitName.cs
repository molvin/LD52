using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitName : UnitBase
{
    public string Name;
    public string UnitType;
    public Sprite Sprite => Renderer.sprite;
    public Color Color;

    [Multiline]
    public string Description;

    public SpriteRenderer Renderer;

    public void UpdateColor()
    {
        //Renderer.material = Instantiate(Renderer.material);
        //Renderer.material.SetColor("_Color", Color);
        Renderer.color = Color;
    }
}
