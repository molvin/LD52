using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitCell : MonoBehaviour
{
    public Image Fill;
    public Image SpriteImage;
    public TextMeshProUGUI Soul;

    public void Setup(Entity ent)
    {
        UnitHealth hp = ent.Get<UnitHealth>();
        Fill.fillAmount = hp.Current / (float)hp.Max;

        UnitName info = ent.Get<UnitName>();
        SpriteImage.sprite = info.Sprite;
        SpriteImage.color = info.Color;

        UnitSoul soul = ent.Get<UnitSoul>();
        Soul.text = $"{soul.SoulAmount}";
    }
}
