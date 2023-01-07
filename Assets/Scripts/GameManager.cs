using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> StartSquad = new List<GameObject>();
    public List<Entity> playerUnits => EntitiesInGame.Where(e => e.Team == Team.Player).ToList();
    public List<Entity> enemyUnits => EntitiesInGame.Where(e => e.Team == Team.Enemy).ToList();
    public List<Entity> EntitiesInGame = new List<Entity>();

    public int Level;
    public string[] Levels;
    public string HarvestScene;

    private Harvest harvester;
    private bool waiting;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    private void OnGUI()
    {
        if(enemyUnits.Count == 0)
        {
            bool next = GUI.Button(new Rect(100, 100, 100, 50), "Next Level");
            if (next)
                StartCoroutine(NextLevel());
        }

        if (waiting)
            GUI.Label(new Rect(100, 200, 200, 50), "Waiting for harvester");

        bool testDmg = GUI.Button(new Rect(100, 300, 100, 50), "Damage selected");
        if(testDmg)
        {
            var temp = playerUnits.OrderBy(x => Random.value);
            foreach(Entity entity in temp)
            {
                if(entity.GetComponent<Selectable>().Selected)
                    entity.GetComponent<UnitHealth>().TakeDamage(1); 
            }
        }
    }

    public IEnumerator NextLevel()
    {
        InputManager.Instance?.ClearSelection();

        foreach(Entity e in playerUnits)
        {
            if (e.TryGet(out UnitSoul comp))
            {
                UnitSoul soul = (UnitSoul) comp;
                soul.SoulAmount = soul.SoulAmount == 0 ? soul.BaseSoul : soul.SoulAmount * soul.SoulGrowthRate;
            }
        }

        SceneManager.LoadScene(HarvestScene);

        waiting = true;
        while (harvester == null || harvester.Harvesting)
        {
            yield return null;
        }
        waiting = false;

        foreach(Entity ent in playerUnits)
        {
            DontDestroyOnLoad(ent.gameObject);
        }

        InputManager.Instance?.ClearSelection();

        Level = Mathf.Min(Level + 1, Levels.Length - 1);
        SceneManager.LoadScene(Levels[Level]);
        yield return null;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode __)
    {
        EntitiesInGame = FindObjectsOfType<Entity>().ToList();

        if(scene.name == HarvestScene)
        {
            harvester = FindObjectOfType<Harvest>();
        }
        else if (Level == 0)
        {
            SpawnPlayerUnits(StartSquad);
        }
        GoToStartPositions(playerUnits);
    }

    private void GoToStartPositions(List<Entity> ents)
    {
        Origin origin = FindObjectOfType<Origin>();

        List<Selectable> selected = new List<Selectable>();
        foreach (Entity ent in ents)
            selected.Add(ent.GetComponent<Selectable>());
        List<Vector3> targets = InputManager.Instance.GetTargets(origin != null ? origin.transform.position : Vector3.zero, selected);
        for (int i = 0; i < targets.Count; i++)
        {
            selected[i].transform.position = targets[i];
            selected[i].TargetPosition = targets[i];
        }
    }

    public void SpawnPlayerUnits(List<GameObject> prefabs)
    {
        List<Entity> newEnts = new List<Entity>();
        foreach(GameObject prefab in prefabs)
        {
            GameObject instance = Instantiate(prefab);
            EntitiesInGame.Add(instance.GetComponent<Entity>());
            DontDestroyOnLoad(instance);
        }
        GoToStartPositions(newEnts);
    }
    public void KillUnit(Entity ent)
    {
        if (ent.Team == Team.Player)
        {
            InputManager.Instance?.RemoveSelected(ent.GetComponent<Selectable>());
        }
        EntitiesInGame.Remove(ent);
        Destroy(ent.gameObject);
    }
}
