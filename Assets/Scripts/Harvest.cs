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
    public bool Harvesting;
    public Upgrade[] Upgrades;
    public BoxCollider KillZone;

    public Button ReapButton;
    public TextMeshProUGUI CurrentSoulsText;
    public TextMeshProUGUI ReapButtonText;

    private float currentSouls;

    private void Awake()
    {
        Instance = this;
        Harvesting = true;
        ReapButton.onClick.AddListener(Reap);
    }

    public void Update()
    {
        Collider[] colls = Physics.OverlapBox(KillZone.transform.position, KillZone.size * 0.5f);
        foreach(Collider coll in colls)
        {
            UnitSoul soul;
            if (soul = coll.GetComponent<UnitSoul>())
            {
                currentSouls += soul.SoulAmount;
                GameManager.Instance.KillUnit(soul.GetComponent<Entity>());
            }
        }

        CurrentSoulsText.text = $"Current Souls:\n {(int)currentSouls}";
        ReapButtonText.text = currentSouls == 0 ? "Go To Next Level" : "Reap Rewards";

    }

    public void Reap()
    {
        if(currentSouls > 0)
        {
            Upgrade[] availableUpgrades = Upgrades.Where(x => x.Cost < currentSouls).ToArray();
            availableUpgrades = availableUpgrades.OrderBy(x => Mathf.Abs(x.Cost - currentSouls)).ToArray();
            Upgrade u = availableUpgrades[0];
            GameManager.Instance.SpawnPlayerUnits(u.Units);
        }
        Harvesting = false;

    }
}
