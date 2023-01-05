using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [System.Serializable]
    public struct Buttons
    {
        public Button Start;
        public Button Options;
        public Button Quit;
    }
    public Buttons Options;
    public OptionsMenu OptionsMenu;

    private void Start()
    {
        Options.Start.onClick.AddListener(StartGame);
        Options.Options.onClick.AddListener(OptionsMenu.Toggle);
        Options.Quit.onClick.AddListener(QuitGame);
    }

    private void StartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
