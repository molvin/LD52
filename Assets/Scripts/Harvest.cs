using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;

[System.Serializable]
public struct Upgrade
{
    public int Cost;
    public List<GameObject> Units;
}

public class Harvest : MonoBehaviour
{
    public static Harvest Instance;
    public Upgrade[] Upgrades;
    public BoxCollider KillZone;

    public float HarvestTime = 3.0f;
    public TextMeshProUGUI CurrentSoulsText;
    public Image TimerBar;

    private float currentSouls;
    private float lastHarvestTime;

    public bool CanHarvest = true;

    public Transform SpawnPoint;
    public Transform TargetPoint;

    private void Awake()
    {
        Instance = this;
        CanHarvest = true;
    }

    public void Update()
    {
        Collider[] colls = Physics.OverlapBox(KillZone.transform.position, KillZone.size * 0.5f);
        if (CanHarvest)
        {
            foreach (Collider coll in colls)
            {
                UnitSoul soul;
                if (soul = coll.GetComponent<UnitSoul>())
                {
                    currentSouls += soul.SoulAmount;
                    lastHarvestTime = Time.time;
                    GameManager.Instance.KillUnit(soul.GetComponent<Entity>());
                }
            }

            if (currentSouls > 0)
            {
                float t = (Time.time - lastHarvestTime) / HarvestTime;
                TimerBar.fillAmount = Mathf.Clamp01(1.0f - t);
                if (t > 1.0f)
                {
                    CanHarvest = false;
                    StartCoroutine(Reap());
                }
            }
        }

        CurrentSoulsText.text = $"Current Souls:\n {(int)currentSouls}";

    }

    public IEnumerator Reap()
    {
        if(currentSouls > 0)
        {
            Upgrade[] availableUpgrades = Upgrades.Where(x => x.Cost < currentSouls).ToArray();
            availableUpgrades = availableUpgrades.OrderBy(x => Mathf.Abs(x.Cost - currentSouls)).ToArray();
            Upgrade u = availableUpgrades[0];
            currentSouls = 0;

            var newUnits = GameManager.Instance.SpawnPlayerUnits(u.Units, SpawnPoint.position);

            foreach(Entity e in newUnits)
            {
                e.Get<Selectable>().TargetPosition = TargetPoint.position;
            }
            yield return new WaitForSeconds(3.0f);

            float t = 0.0f;
            while(t < 1.0f)
            {
                t += Time.deltaTime;
                TimerBar.fillAmount = t;
                yield return null;
            }
            CanHarvest = true;
        }
    }
}
