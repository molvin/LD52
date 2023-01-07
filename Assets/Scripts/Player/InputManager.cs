using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Texture2D BoxSelectTexture;

    private bool selecting;
    private Vector2 selectStart;
    private Vector2 selectEnd;
    public List<Selectable> selected = new List<Selectable>();
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selecting = true;
            selectStart = Input.mousePosition;
        }
        if(selecting)
        {
            selectEnd = Input.mousePosition;
            if(Input.GetMouseButtonUp(0))
            {
                selecting = false;
                FinishSelect();
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            groundPlane.Raycast(ray, out float startEnter);
            Vector3 targetPos = ray.GetPoint(startEnter);

            foreach (Selectable s in selected)
                s.TargetPosition = targetPos;
        }
    }

    private void FinishSelect()
    {
        Camera cam = Camera.main;
        Ray startRay = cam.ScreenPointToRay(selectStart);
        Ray endRay = cam.ScreenPointToRay(selectEnd);
        groundPlane.Raycast(startRay, out float startEnter);
        groundPlane.Raycast(endRay, out float endEnter);
        Vector3 startPos = startRay.GetPoint(startEnter);
        Vector3 endPos = endRay.GetPoint(endEnter);

        Vector3 extents = (endPos - startPos) / 2.0f;
        extents.x = Mathf.Abs(extents.x);
        extents.y = 1000.0f;
        extents.z = Mathf.Abs(extents.z);
        Collider[] colls = Physics.OverlapBox((startPos + endPos) / 2.0f, extents);
        foreach (Selectable u in selected)
            u.Selected = false;

        selected.Clear();
        foreach(Collider coll in colls)
        {
            Selectable u;
            if (u = coll.GetComponent<Selectable>())
            {
                selected.Add(u);
                u.Selected = true;
            }
        }

    }

    private void OnGUI()
    {
        if (!selecting)
            return;

        Vector2 size = selectEnd - selectStart;

        Rect rect = new Rect
        {
            x = selectStart.x,
            y = Screen.height - selectStart.y,
            width = size.x,
            height = -size.y
        };
        GUI.DrawTexture(rect, BoxSelectTexture);
    }
}
