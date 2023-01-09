using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image Background;
    public Image Fill;
    public Image FillChange;
    public GameObject SegmentObj;
    public Transform SegmentParent;
    public int HPPerSegment;
    public int segmentOffset;
    public bool ShowOnFullHealth;
    public float GreenThreshold;
    public float YellowThreshold;

    //Damage anim
    public float TakeDamageTime;
    public AnimationCurve TakeDamageCurve;
    private float damageStartFill = 0;
    private float damageAnimTime = 0f;
    private bool isTakingDamageAnim = false;
    public Gradient takeDamageColors;

    public AnimationCurve DamageShakeCurve;
    public float ShakeMagnitude;
    public float ShakeTime;
    public float ShakeTriggerProcent;
    private Vector3 initialLocalPosition;

    public AnimationCurve LevelUpCurve;
    public float LevelUpEventTime;
    public TextMeshProUGUI LevelUpText;

    private UnitHealth health;

    public AudioClip LevelUpSound;

    private int level;

    private void Awake()
    {
        gameObject.SetActive(true);
        health = GetComponentInParent<UnitHealth>();
        initialLocalPosition = transform.localPosition;
        SetMaxHP(health);

        UnitSoul soul = GetComponentInParent<UnitSoul>();
        if (soul)
            LevelUpText.text = $"{level}";
        else
            LevelUpText.enabled = false;
    }

    private void SetMaxHP(UnitHealth health)
    {   
        int maxHealth = health.Max;
        int numOfSegments = (maxHealth / HPPerSegment) - 1;

        for (int i = 0; i <= numOfSegments; i++)
        {
            Instantiate(SegmentObj, SegmentParent);
        }
        updateHeathbar();
    }
    private void updateHeathbar()
    {
        if (health)
        {
            Fill.fillAmount = (health.Current / (float)health.Max);
        }
    }
    public void UnitTakeDamage(int damage)
    {
        updateHeathbar();
        
        if (!isTakingDamageAnim)
        {
            damageStartFill = FillChange.fillAmount;
            damageAnimTime = 0f;
            if (isActiveAndEnabled)
                StartCoroutine(takeDamage());
        }
        float damageTaken = damageStartFill - Fill.fillAmount;
        FillChange.color = takeDamageColors.Evaluate(damageTaken);
        if(damageTaken >= ShakeTriggerProcent && isActiveAndEnabled)
            StartCoroutine(DamageShake(damageTaken));
    }

    public void UnitLevelUp()
    {
        StartCoroutine(SetNewLevel());
    }

    private IEnumerator takeDamage()
    {
        isTakingDamageAnim = true;
        while (damageAnimTime < TakeDamageTime)
        {
            float factor = damageAnimTime / TakeDamageTime;
            damageAnimTime += Time.deltaTime;
            float framestep = TakeDamageCurve.Evaluate(factor);
            FillChange.fillAmount = Mathf.Lerp(damageStartFill, Fill.fillAmount, framestep);
            yield return null;
        }
        isTakingDamageAnim = false;
        yield return null;
    }

    private IEnumerator DamageShake(float amount)
    {
        float time = 0f;
        while (time < ShakeTime)
        {
            time += Time.deltaTime;
            float factor = time / ShakeTime;
            float framestep = DamageShakeCurve.Evaluate(factor) * amount * ShakeMagnitude;
            transform.localPosition = new Vector3(framestep, initialLocalPosition.y, initialLocalPosition.z);
            yield return null;
        }
        yield return null;
    }

    private IEnumerator SetNewLevel()
    {
        float time = 0;
        AudioManager.Instance.PlayAudio(LevelUpSound);
        while(time < LevelUpEventTime)
        {
            time += Time.deltaTime;
            float factor = time / LevelUpEventTime;
            float framestep = LevelUpCurve.Evaluate(factor);
            LevelUpText.rectTransform.anchoredPosition = new Vector2(LevelUpText.rectTransform.anchoredPosition.x, framestep);
            yield return null;
        }
        LevelUpText.text = ++level + "";
        yield return null;
    }
}
