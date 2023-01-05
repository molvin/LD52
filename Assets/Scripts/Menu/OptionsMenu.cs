using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsMenu : MonoBehaviour
{
    public GameObject Root;
    public Button ToMenu;
    public Button Close;
    public Slider MasterVolume;
    public Toggle Fullscreen;
    public AudioMixer Mixer;

    private bool InMenu => UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0;

    public void Awake()
    {
        ToMenu.onClick.AddListener(() => UnityEngine.SceneManagement.SceneManager.LoadScene(0));
        Close.onClick.AddListener(Toggle);
        MasterVolume.onValueChanged.AddListener(SetVolume);
        StartCoroutine(Startup());
    }
    private IEnumerator Startup()
    {
        yield return null;
        SetVolume(PlayerPrefs.GetFloat("MasterVolume", 0.5f));
    }

    public void Update()
    {
        if (!InMenu && Input.GetButtonDown("Start"))
            Toggle();

        ToMenu.interactable = !InMenu;
    }

    public void Toggle()
    {
        Root.SetActive(!Root.activeSelf);
        if(Root.activeSelf)
        {
            EventSystem.current.SetSelectedGameObject(Fullscreen.gameObject);
        }
        else if(InMenu)
        {
            EventSystem.current.SetSelectedGameObject(GameObject.Find("Start"));
        }
    }

    public void ToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
    public void SetVolume(float value)
    {
        MasterVolume.SetValueWithoutNotify(value);
        Mixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20.0f);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
}
