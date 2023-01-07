using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<Entity> playerUnits = new List<Entity>();
    public List<Entity> enemyUnits = new List<Entity>();

    public int Level;

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
        }
    }

    private void OnGUI()
    {
        //GUI.Button(new Rect())
    }

    public IEnumerator NextLevel()
    {
        //TODO: activate and wait for harvest UI, also disable input manager

        bool harvestUiActive = false;
        while(harvestUiActive)
        {
            yield return null;
        }

        foreach(Entity ent in playerUnits)
        {
            DontDestroyOnLoad(ent.gameObject);
        }

        SceneManager.LoadScene(Mathf.Min(SceneManager.GetActiveScene().buildIndex + 1, SceneManager.sceneCount - 1));
        yield return null;

        Origin origin = FindObjectOfType<Origin>();

        List<Selectable> selected = new List<Selectable>();
        foreach (Entity ent in playerUnits)
            selected.Add(ent.GetComponent<Selectable>());
        List<Vector3> targets = InputManager.Instance.GetTargets(origin != null ? origin.transform.position : Vector3.zero, selected);
        for (int i = 0; i < targets.Count; i++)
            selected[i].transform.position = targets[i];
        
    }
    
}
