using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelText : MonoBehaviour
{
    public TextMeshProUGUI Text;
    private void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("Level"))
            Text.text = sceneName;
        else
            Text.text = "";
    }
}
