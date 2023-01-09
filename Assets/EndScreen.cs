using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    // Start is called before the first frame update

    public TextMeshProUGUI text;

    public void OnEnable()
    {
        text.text = "You lose \n You got to level" + SceneManager.GetActiveScene().buildIndex;
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

}
