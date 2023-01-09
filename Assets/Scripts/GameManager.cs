using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.UI.Image;

public class GameManager : MonoBehaviour
{
    public static int LevelCount = 15;
    public enum State
    {
        Start,
        Game,
        Harvest,
        End,
        Transition,
        StartMenu
    }


    public static GameManager Instance;
    public List<GameObject> StartSquad = new List<GameObject>();
    public List<Entity> playerUnits => EntitiesInGame.Where(e => e.Team == Team.Player).ToList();
    public List<Entity> enemyUnits => EntitiesInGame.Where(e => e.Team == Team.Enemy).ToList();
    public List<Entity> EntitiesInGame = new List<Entity>();

    public int Level;
    public string HarvestScene;
    public float DebugSoulAmount;

    public State CurrentState = State.Start;

    [Header("Audio")]
    public AudioClip GamePlayMusic;
    public AudioClip HarvestMusic;

    private Door EntryDoor;
    private Door ExitDoor;
    private Door HarvestDoor;
    private Door ExitGameDoor;

    private UnitInfo info;

    public Color[] PlayerColors;

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

                    if (SceneManager.GetActiveScene().name != "EndLevel")
                    {
                        //CurrentState = State.End;
                        StartCoroutine(End(false, false));
                    } else
                    {
                        FindObjectOfType<WinScreen>(true).gameObject.SetActive(true);
                    }
                    
                }
                break;
            case State.Harvest:
                break;
            case State.End:
                break;
            case State.Transition:
                break;
            case State.StartMenu:
                CurrentState = State.End;
                StartCoroutine(End(false, true));        
                info?.Toggle(false);
                break;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode _)
    {
        if(scene.name == "MainMenu")
        {
            Level = 0;
            foreach(Entity ent in playerUnits)
            {
                Destroy(ent.gameObject);
            }
            playerUnits.Clear();
            CurrentState = State.StartMenu;
        } else
        {
            CurrentState = State.Start;
        }

        info = FindObjectOfType<UnitInfo>();

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
            if (door.DoorType == Door.Type.Quit)
                ExitGameDoor = door;
        }

        //CurrentState = State.Start;

        ObjectPool.Instance.Clear();
        EntitiesInGame = FindObjectsOfType<Entity>().ToList();

        if(scene.name == HarvestScene)
        {
            if (playerUnits.Count == 0)
            {
                SpawnPlayerUnits(StartSquad);
                foreach(Entity ent in playerUnits)
                {
                    var soul = ent.Get<UnitSoul>();
                    soul.SoulAmount = DebugSoulAmount;
                }
            }

        }
        else if (Level == 0) //&& scene.name != "MainMenu")
        {
            SpawnPlayerUnits(StartSquad);
        }

        //if(scene.name != "MainMenu") 
            StartCoroutine(StartLevel(playerUnits, scene.name == HarvestScene));

        EntitiesInGame.ForEach(entity =>
        {
            if (entity.Team == Team.Player)
            {
                entity.GetComponentInChildren<DumbFieldOfViewMesh>().onNewLevel();
            }
        });
    }

    private IEnumerator StartLevel(List<Entity> ents, bool harvest)
    {
        if (harvest)
        {
            AudioManager.Instance.StopMusic(GamePlayMusic);
            AudioManager.Instance.PlayMusic(HarvestMusic);
        }
        else
        {
            AudioManager.Instance.PlayMusic(GamePlayMusic);
            AudioManager.Instance.StopMusic(HarvestMusic);
        }

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

        yield return FindObjectOfType<Fade>().FadeAlpha(0f);

        yield return new WaitForSeconds(0.5f);

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
                if (selected[i].transform.position.Dist2D(targets[i]) > 0.5f)
                    farAway = true;
            }
            yield return null;
        }
        if((Time.time - startTime) >= 10.0f)
        {
            for (int i = 0; i < targets.Count; i++)
            {
                selected[i].transform.position = targets[i];
                selected[i].TargetPosition = targets[i];
            }
        }
        InputManager.Instance.enabled = true;

        yield return EntryDoor.Toggle(false);

        CurrentState = harvest ? State.Harvest : State.Game;

        ToggleEnemies(true);

        if(harvest)
        {
            StartCoroutine(End(true, false));
        }
    }

    public List<Entity> SpawnPlayerUnits(List<GameObject> prefabs, Vector3 spawnPoint = default(Vector3))
    {
        Color color = PlayerColors[Random.Range(0, PlayerColors.Length)];
            
        List<Entity> newEnts = new List<Entity>();
        foreach(GameObject prefab in prefabs)
        {
            spawnPoint.y = 1.0f;
            GameObject instance = Instantiate(prefab, spawnPoint, Quaternion.identity);
            Entity ent = instance.GetComponent<Entity>();
            EntitiesInGame.Add(ent);
            DontDestroyOnLoad(instance);
            newEnts.Add(ent);

            if(ent.Team == Team.Player)
            {
                var info = ent.GetComponent<UnitName>();
                info.Color = color;                
                info.UpdateColor();

            }
        }
        return newEnts;
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
        GameObject endScreen = FindObjectOfType<EndScreen>(true).gameObject;
        endScreen.SetActive(true);
        yield return null;
        //TODO move to endscreen
        //yield return new WaitForSeconds(3.0f);
        //SceneManager.LoadScene(0);
    }

    private IEnumerator End(bool skipHarvest, bool menu)
    {
        // Get souls

        if(!skipHarvest && !menu)
        {
            foreach (Entity e in playerUnits)
            {
                UnitSoul soul = e.Get<UnitSoul>();
                soul.SoulAmount += soul.BaseAmount;

                UnitHealth health = e.Get<UnitHealth>();
                health.TakeDamage(-health.Max / 5, Vector3.zero);
                e.GetComponentInChildren<HealthBar>().UnitLevelUp();
                yield return new WaitForSeconds(0.2f);
            }
        }


        yield return new WaitForSeconds(1.0f);
        info?.Toggle(true);

        if (!skipHarvest)
            StartCoroutine(HarvestDoor.Toggle(true));
        if (ExitGameDoor) StartCoroutine(ExitGameDoor.Toggle(true));
        yield return ExitDoor.Toggle(true);

        //todo wait for player to walk in, take control of their units, and transition level
        bool goToHarvest = false;
        bool goToNextLevel = false;


        while(!goToHarvest && !goToNextLevel)
        {
            foreach (Entity e in playerUnits)
            {
                // if close to harvest point or end point
                if (!skipHarvest && e.transform.position.Dist2D(HarvestDoor.TargetPoint.position) < 4.0f)
                {
                    goToHarvest = true;
                    break;
                }
                else if (e.transform.position.Dist2D(ExitDoor.TargetPoint.position) < 4.0f)
                {
                    goToNextLevel = true;
                    break;
                } else if (ExitGameDoor != null && e.transform.position.Dist2D(ExitGameDoor.TargetPoint.position) < 4.0f)
                {
                    Debug.Log("Quitting game");
                    Application.Quit();
                }
            }
            yield return null;
        }
        CurrentState = State.Transition;
        info?.Toggle(false);


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

        yield return FindObjectOfType<Fade>().FadeAlpha(1f);

        yield return NextLevel(goToNextLevel);
    }

    public IEnumerator NextLevel(bool skipHarvest)
    {
        InputManager.Instance?.ClearSelection();

        if(!skipHarvest)
        {
            SceneManager.LoadScene(HarvestScene);
        }
        else
        {
            Level = Mathf.Min(Level + 1, LevelCount - 1);
            SceneManager.LoadScene($"Level {Mathf.Max(Level, 1)}");
        }

        yield return null;
    }

    private void ToggleEnemies(bool on)
    {
        foreach(Entity ent in enemyUnits)
        {
            AIBrainBase brain = ent.GetComponent<AIBrainBase>();
            if(brain)
                brain.enabled = on;
        }
    }

}
