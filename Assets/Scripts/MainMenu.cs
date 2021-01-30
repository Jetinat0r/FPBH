using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject otherMenu;

    public void SwapMenu()
    {
        gameObject.SetActive(false);
        otherMenu.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ChangeLevel()
    {
        SceneManager.LoadScene(EventSystem.current.currentSelectedGameObject.name);
    }
}
