using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    public Texture2D BoxSelectTexture;
    public float BoxSelectMinDist;
    public float DoubleClickTime;
    public float ClickSelectRadius;

    private bool selecting;
    private bool doubleClickWindow;
    private Vector2 selectStart;
    private Vector2 selectEnd;
    public List<Selectable> Selected = new List<Selectable>();
    private Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (!selecting && !doubleClickWindow && Input.GetMouseButtonDown(0))
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
        if(Input.GetButtonDown("Stop"))
        {
            foreach (Selectable s in Selected)
                s.TargetPosition = s.transform.position;
        }
    }
    private IEnumerator Select()
    {
        selectStart = selectEnd = Input.mousePosition;
        while(!Input.GetMouseButtonUp(0))
        {
            selectEnd = Input.mousePosition;
            yield return null;
        }
        StartCoroutine(DoubleClick());

        selecting = false;
        if((selectStart - selectEnd).magnitude < BoxSelectMinDist)
            FinishSelect(SelectOne(false));
        else
            FinishSelect(BoxSelect());
    }
    private IEnumerator DoubleClick()
    {
        doubleClickWindow = true;
        float t = DoubleClickTime;
        while (t > 0.0f)
        {
            t -= Time.deltaTime;
            if (Input.GetMouseButtonDown(0))
            {
                FinishSelect(SelectOne(true));
                break;
            }
            yield return null;
        }
        doubleClickWindow = false;
    }

    private List<Selectable> SelectOne(bool all)
    {
        Camera cam = Camera.main;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        groundPlane.Raycast(ray, out float enter);
        Vector3 center = ray.GetPoint(enter);
        Debug.DrawRay(center, Vector3.up * ClickSelectRadius, Color.red, 10.0f);
        Collider[] colls = Physics.OverlapSphere(center, ClickSelectRadius);

        List<Selectable> new_selected = new List<Selectable>();
        foreach(Collider coll in colls)
        {
            Selectable u;
            if (u = coll.GetComponent<Selectable>())
            {
                new_selected.Add(u);
                break;
            }
        }

        if(all && new_selected.Count > 0)
        {
            Selectable[] allSelectable = FindObjectsOfType<Selectable>();
            Selectable[] sameType = allSelectable.Where(x => x.Type == new_selected[0].Type).ToArray();
            new_selected.Clear();
            new_selected.AddRange(sameType);
        }

        return new_selected;
    }

    private List<Selectable> BoxSelect()
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

        List<Selectable> new_selected = new List<Selectable>();
        foreach(Collider coll in colls)
        {
            Selectable u;
            if (u = coll.GetComponent<Selectable>())
            {
                new_selected.Add(u);
            }
        }
        return new_selected;
    }

    private void FinishSelect(List<Selectable> newSelected)
    {
        if (Input.GetButton("Add"))
            newSelected = newSelected.Union(Selected).ToList();
        if (Input.GetButton("Subtract"))
        {
            var intersect = Selected.Intersect(newSelected).ToList();
            newSelected = Selected.Where(x => !intersect.Contains(x)).ToList();
        }

        foreach (Selectable s in Selected)
            s.Selected = false;

        foreach (Selectable s in newSelected)
            s.Selected = true;

        Selected = newSelected;
        Selected = Selected.OrderBy(x => x.GetComponent<Entity>().Id).ToList();
    }


    private Vector3 FibLattice(float i, float n)
    {
        float theta = (1.0f + Mathf.Sqrt(5)) / 2.0f;
        float x = (i / theta) % 1;
        float y = i / n;
        return new Vector3(x, 0, y);
    }
    private Vector3 FibDisc(float i, float n)
    {
        Vector3 pos = FibLattice(i, n);
        float angle = 2.0f * Mathf.PI * pos.x;
        float radius = Mathf.Sqrt(pos.z);

        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
    }

    public List<Vector3> GetTargets(Vector3 target, List<Selectable> selected)
    {
        if (selected.Count == 1)
        {
            return new List<Vector3> { target };
        }

        float radius = 0.0f;
        Vector3 center = Vector3.zero;
        foreach (Selectable s in selected)
        {
            radius += s.Spacing;
            center += s.transform.position;
        }
        radius /= selected.Count;
        center /= selected.Count;

        List<Vector3> result = new List<Vector3>();
        foreach (Selectable s in selected)
            result.Add(Vector3.zero);
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < selected.Count; i++)
        {
            points.Add(FibDisc(i, selected.Count) * radius);
        }

        var sortedSelected = selected.OrderBy(x => x.GetComponent<Entity>().Id).ToList();
        foreach (Selectable s in sortedSelected)
        {
            float dist = 10000.0f;
            int closest = 0;
            for (int i = 0; i < points.Count; i++)
            {
                float d = s.transform.position.Dist2D(center + points[i]);
                if (d < dist)
                {
                    dist = d;
                    closest = i;
                }
            }
            result[selected.IndexOf(s)] = points[closest] + target;
            points.RemoveAt(closest);
        }
        return result;
    }

    private void Move(Vector3 target)
    {
        var targets = GetTargets(target, Selected);
        for (int i = 0; i < targets.Count; i++)
            Selected[i].TargetPosition = targets[i];
    }

    public void RemoveSelected(Selectable s)
    {
        Selected.Remove(s);
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

    public void ClearSelection()
    {
        FinishSelect(new List<Selectable>());
    }
}
