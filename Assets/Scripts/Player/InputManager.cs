using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public Texture2D BoxSelectTexture;
    public float BoxSelectMinDist;
    public float DoubleClickTime;
    public float ClickSelectRadius;

    private bool selecting;
    private Vector2 selectStart;
    private Vector2 selectEnd;
    public List<Selectable> selected = new List<Selectable>();
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

    private void Update()
    {
        if (!selecting && Input.GetMouseButtonDown(0))
        {
            selecting = true;
            StartCoroutine(Select());
        }

        if (!selecting && Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            groundPlane.Raycast(ray, out float startEnter);
            Vector3 targetPos = ray.GetPoint(startEnter);

            Move(targetPos);
        }
    }
    private IEnumerator Select()
    {
        selectStart = selectEnd = Input.mousePosition;
        float startTime = Time.time;
        while(!Input.GetMouseButtonUp(0))
        {
            selectEnd = Input.mousePosition;
            yield return null;
        }
        bool doubleClick = false;
        float t = DoubleClickTime - (Time.time - startTime);
        while(t > 0.0f)
        {
            t -= Time.deltaTime;
            if(Input.GetMouseButtonDown(0))
            {
                doubleClick = true;
                break;
            }
            yield return null;
        }

        foreach (Selectable u in selected)
            u.Selected = false;
        selected.Clear();

        selecting = false;
        if((selectStart - selectEnd).magnitude < BoxSelectMinDist)
            SelectOne(doubleClick);
        else
            BoxSelect();
    }

    private void SelectOne(bool all)
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay((selectStart + selectEnd) / 2.0f);
        groundPlane.Raycast(ray, out float enter);
        Vector3 center = ray.GetPoint(enter);
        Debug.DrawRay(center, Vector3.up * ClickSelectRadius, Color.red, 10.0f);
        Collider[] colls = Physics.OverlapSphere(center, ClickSelectRadius);
        foreach(Collider coll in colls)
        {
            Selectable u;
            if (u = coll.GetComponent<Selectable>())
            {
                selected.Add(u);
                break;
            }
        }

        if(all && selected.Count > 0)
        {
            Selectable[] allSelectable = FindObjectsOfType<Selectable>();
            Selectable[] sameType = allSelectable.Where(x => x.Type == selected[0].Type).ToArray();
            selected.Clear();
            selected.AddRange(sameType);
        }

        foreach (Selectable s in selected)
            s.Selected = true;
    }

    private void BoxSelect()
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

    private Vector3 FibDisc(int i, int total, float radius)
    {
        float k = i + .5f;
        float r = Mathf.Sqrt((k) / total);
        float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * k;
        float x = r * Mathf.Cos(theta) * radius;
        float y = r * Mathf.Sin(theta) * radius;

        return new Vector3(x, 0, y);
    }
    private Vector3 FibLattice(float i, float n, float size)
    {
        float theta = (1 + Mathf.Sqrt(5)) / 2;
        float x = (i / theta) % 1;
        float y = i / n;
        return new Vector3(x * size, 0, y * size);
    }

    private void Move(Vector3 target)
    {
        if(selected.Count == 1)
        {
            selected[0].TargetPosition = target;
            return;
        }

        List<Vector3> points = new List<Vector3>();
        float radius = 0.0f;
        Vector3 center = Vector3.zero;
        foreach (Selectable s in selected)
        {
            radius += s.Spacing;
            center += s.transform.position;
        }
        radius /= selected.Count;
        center /= selected.Count;


        for (int i = 0; i < selected.Count; i++)
        {
            points.Add(target + FibLattice(i, selected.Count, radius));
        }

        foreach(Selectable s in selected)
        {
            Vector3 toCenter = (center - s.transform.position).normalized;
            int index = 0;
            float smallest = 1000.0f;
            for(int i = 0; i < points.Count; i++)
            {
                Vector3 toPoint = (points[i] - s.transform.position).normalized;
                float d = Vector3.Dot(toCenter, toPoint);
                if (d < smallest)
                {
                    smallest = d;
                    index = i;
                }
            }
            points.RemoveAt(index);
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
