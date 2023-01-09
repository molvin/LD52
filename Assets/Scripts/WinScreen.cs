using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{

    public TextMeshProUGUI text;

    public void OnEnable()
    {
        text.text = "You are victorious";
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
