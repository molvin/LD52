using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    public GameObject Single;
    public GameObject Multiple;
    public Animator Anim;

    public Transform Grid;
    public UnitCell UnitCellPrefab;
    public TextMeshProUGUI Title, Health, Dmg, Souls, GrowthRate, Description;
    
    private List<Selectable> lastSelected = new List<Selectable>();
    private List<(UnitCell, UnitHealth)> CurrentCells = new List<(UnitCell, UnitHealth)>();

    private bool active;

    public void Toggle(bool on)
    {
        active = on;
    }
    private void InternalToggle()
    {
        bool on = active;
        bool single = InputManager.Instance.Selected.Count == 1;

        if (InputManager.Instance.Selected.Count == 0)
            on = false;
        else
        {
            Single.SetActive(single);
            Multiple.SetActive(!single);
        }

        if(Anim.GetBool("Open") != on)
            Anim.SetBool("Open", on);

        var selected = InputManager.Instance.Selected;
        bool changed = selected.Count != lastSelected.Count;
        if (!changed)
        {
            for (int i = 0; i < selected.Count; i++)
            {
                if (!lastSelected.Contains(selected[i]))
                {
                    changed = true;
                    break;
                }
            }
        }

        if (changed)
        {
            lastSelected = new List<Selectable>(selected);
            if (lastSelected.Count > 0)
            {
                if (single)
                    RebuildSingle();
                else
                    RebuildMultiple();
            }

        }
    }

    private void RebuildSingle()
    {
        Entity ent = lastSelected[0].GetComponent<Entity>();
        UnitName unit = ent.Get<UnitName>();
        UnitHealth hp = ent.Get<UnitHealth>();
        UnitAttack atk = ent.GetComponent<UnitAttack>();
        UnitSoul soul = ent.Get<UnitSoul>();
        Title.text = $"{unit.UnitType}";
        Health.text = $"{hp.Current}/{hp.Max}";
        Dmg.text = $"{atk.Damage}";
        Souls.text = $"{soul.SoulAmount}";
        GrowthRate.text = $"{soul.BaseAmount}";
        Description.text = unit.Description;
    }
    private void RebuildMultiple()
    {
        foreach (Transform child in Grid)
            Destroy(child.gameObject);

        CurrentCells.Clear();

        var selected = lastSelected.ToList();
        for(int i = 0; i < selected.Count; i++)
        {
            UnitCell cell = Instantiate(UnitCellPrefab, Grid) as UnitCell;
            cell.Setup(selected[i].GetComponent<Entity>());
            CurrentCells.Add((cell, selected[i].GetComponent<UnitHealth>()));
        }
    }

    private void Update()
    {
        InternalToggle();

        foreach((UnitCell cell, UnitHealth ent) in CurrentCells)
        {
            cell.Fill.fillAmount = ent.Current / (float)ent.Max;
        }

        if (lastSelected.Count == 1)
            RebuildSingle();
    }

}
