using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class EndMenu : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }

    public void GoToMain()
    {
        SceneManager.LoadScene(0);
    }
}
