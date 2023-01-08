using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    public enum State
    {
        Start,
        Game,
        Harvest,
        End
    }


    public static GameManager Instance;
    public List<GameObject> StartSquad = new List<GameObject>();
    public List<Entity> playerUnits => EntitiesInGame.Where(e => e.Team == Team.Player).ToList();
    public List<Entity> enemyUnits => EntitiesInGame.Where(e => e.Team == Team.Enemy).ToList();
    public List<Entity> EntitiesInGame = new List<Entity>();

    public int Level;
    public string[] Levels;
    public string HarvestScene;

    public State CurrentState = State.Start;

    private Door EntryDoor;
    private Door ExitDoor;
    private Door HarvestDoor;

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

    private void Update()
    {
        switch (CurrentState)
        {
            case State.Start:
                break;
            case State.Game:
                if (enemyUnits.Count == 0)
                {
                    CurrentState = State.End;
                    StartCoroutine(End());
                }
                break;
            case State.Harvest:
                break;
            case State.End:
                break;
        }
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode _)
    {
        EntryDoor = HarvestDoor = ExitDoor = null;
        var doors = FindObjectsOfType<Door>();
        foreach (Door door in doors)
        {
            if (door.DoorType == Door.Type.Entry)
                EntryDoor = door;
            if (door.DoorType == Door.Type.Harvest)
                HarvestDoor = door;
            if (door.DoorType == Door.Type.Exit)
                ExitDoor = door;
        }

        CurrentState = State.Start;

        ObjectPool.Instance.Clear();
        EntitiesInGame = FindObjectsOfType<Entity>().ToList();

        if(scene.name == HarvestScene)
        {
            harvester = FindObjectOfType<Harvest>();
        }
        else if (Level == 0)
        {
            SpawnPlayerUnits(StartSquad);
        }

        StartCoroutine(StartLevel(playerUnits, scene.name == HarvestScene));
    }

    private IEnumerator StartLevel(List<Entity> ents, bool harvest)
    {
        ToggleEnemies(false);

        InputManager.Instance.enabled = false;

        List<Selectable> selected = new List<Selectable>();
        foreach (Entity ent in ents)
            selected.Add(ent.GetComponent<Selectable>());

        foreach(Selectable s in selected)
        {
            s.transform.position = EntryDoor.SpawnPoint.position;
            s.TargetPosition = EntryDoor.SpawnPoint.position;
        }

        yield return new WaitForSeconds(2.0f);

        yield return EntryDoor.Toggle(true);

        List<Vector3> targets = InputManager.Instance.GetTargets(EntryDoor.TargetPoint.position, selected);
        for (int i = 0; i < targets.Count; i++)
        {
            selected[i].TargetPosition = targets[i];
        }

        bool farAway = true;
        float startTime = Time.time;
        while(farAway && (Time.time - startTime) < 10.0f)
        {
            farAway = false;
            for (int i = 0; i < targets.Count; i++)
            {
                if (selected[i].transform.position.Dist2D(targets[i]) > 2)
                    farAway = true;
            }
            yield return null;
        }
        if((Time.time - startTime) > 10.0f)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                selected[i].transform.position = targets[i];
                selected[i].TargetPosition = targets[i];
            }
        }

        yield return EntryDoor.Toggle(false);

        InputManager.Instance.enabled = true;
        CurrentState = harvest ? State.Harvest : State.Game;

        ToggleEnemies(true);
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
    }
    public void KillUnit(Entity ent)
    {
        if (ent.Team == Team.Player)
        {
            InputManager.Instance?.RemoveSelected(ent.GetComponent<Selectable>());
        }
        EntitiesInGame.Remove(ent);
        Destroy(ent.gameObject);

        if(playerUnits.Count == 0 && SceneManager.GetActiveScene().name != HarvestScene)
        {
            StartCoroutine(Lose());
        }
    }

    private IEnumerator Lose()
    {
        Debug.Log("You Lose!");
        CurrentState = State.End;

        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene(0);
    }

    private IEnumerator End()
    {
        yield return new WaitForSeconds(1.0f);

        StartCoroutine(ExitDoor.Toggle(true));
        yield return HarvestDoor.Toggle(true);

        //todo wait for player to walk in, take control of their units, and transition level
        bool goToHarvest = false;
        bool goToNextLevel = false;


        while(!goToHarvest && !goToNextLevel)
        {
            foreach (Entity e in playerUnits)
            {
                // if close to harvest point or end point
                if (e.transform.position.Dist2D(HarvestDoor.TargetPoint.position) < 4.0f)
                {
                    goToHarvest = true;
                    break;
                }
                else if (e.transform.position.Dist2D(ExitDoor.TargetPoint.position) < 4.0f)
                {
                    goToNextLevel = true;
                    break;
                }
            }
            yield return null;
        }

        InputManager.Instance.enabled = false;
        foreach(Entity e in playerUnits)
        {
            e.Get<Selectable>().TargetPosition = goToHarvest ? HarvestDoor.SpawnPoint.position : ExitDoor.SpawnPoint.position;
        }

        float maxTime = 3.0f;
        float startTime = Time.time;

        HashSet<Entity> beenClose = new HashSet<Entity>();
        while((Time.time - startTime) < maxTime)
        {
            Vector3 target = goToHarvest ? HarvestDoor.TargetPoint.position : ExitDoor.TargetPoint.position;
            bool farAway = false;
            foreach (Entity e in playerUnits)
            {
                if (beenClose.Contains(e))
                    continue;
                if (e.transform.position.Dist2D(target) < 2.0f)
                    beenClose.Add(e);
                else
                    farAway = true;
            }
            if (!farAway)
                break;
            yield return null;
        }

        //TODO: fade out



        yield return NextLevel(goToNextLevel);
    }

    public IEnumerator NextLevel(bool skipHarvest)
    {
        InputManager.Instance?.ClearSelection();

        foreach (Entity e in playerUnits)
        {
            if (e.TryGet(out UnitSoul comp))
            {
                UnitSoul soul = (UnitSoul)comp;
                soul.SoulAmount = soul.SoulAmount == 0 ? soul.BaseSoul : soul.SoulAmount * soul.SoulGrowthRate;
            }
        }

        if(!skipHarvest)
        {
            SceneManager.LoadScene(HarvestScene);

            waiting = true;
            while (harvester == null || harvester.Harvesting)
            {
                yield return null;
            }
            waiting = false;

            foreach (Entity ent in playerUnits)
            {
                DontDestroyOnLoad(ent.gameObject);
            }

            InputManager.Instance?.ClearSelection();
        }

        Level = Mathf.Min(Level + 1, Levels.Length - 1);
        SceneManager.LoadScene(Levels[Level]);
        yield return null;
    }

    private void ToggleEnemies(bool on)
    {
        foreach(Entity ent in enemyUnits)
        {
            ent.GetComponent<AIBrainBase>().enabled = on;
        }
    }

}
