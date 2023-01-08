using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    public GameObject Single;
    public GameObject Multiple;
    public Animator Anim;

    public Transform Grid;
    public UnitCell UnitCellPrefab;
    
    private List<Selectable> lastSelected = new List<Selectable>();


    public void Toggle(bool on)
    {
        bool single = InputManager.Instance.Selected.Count == 1;

        if (InputManager.Instance.Selected.Count == 0)
            on = false;
        else
        {
            Single.SetActive(single);
            Multiple.SetActive(!single);
        }
        Anim.SetBool("Open", on);

        var selected = InputManager.Instance.Selected;
        bool changed = selected.Count != lastSelected.Count;
        if(!changed)
        {
            for(int i = 0; i < selected.Count; i++)
            {
                if (!lastSelected.Contains(selected[i]))
                {
                    changed = true;
                    break;
                }
            }
        }

        if(changed)
        {
            lastSelected = new List<Selectable>(selected);
            if(lastSelected.Count > 0)
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

    }
    private void RebuildMultiple()
    {
        foreach (Transform child in Grid)
            Destroy(child.gameObject);

        var selected = lastSelected.ToList();
        for(int i = 0; i < selected.Count; i++)
        {
            UnitCell cell = Instantiate(UnitCellPrefab, Grid) as UnitCell;
            cell.Setup(selected[i].GetComponent<Entity>());
        }
    }

    private void Update()
    {
        
    }

}
