using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isPaused = false;

    [SerializeField]
    private Transform playerSpawn;
    [SerializeField]
    private GameObject playerPrefab;

    private Player playerScript;

    private void Start()
    {
        GameObject _player = Instantiate(playerPrefab, playerSpawn.position, playerSpawn.rotation);
        _player.name = "Player";
        playerScript = _player.GetComponent<Player>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !playerScript.victoryAchieved)
        {
            if (isPaused)
            {
                ToggleIsPaused();

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                Time.timeScale = 1;
            }
            else
            {
                ToggleIsPaused();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0;
            }
            isPaused = !isPaused;
        }
    }

    public void ToggleIsPaused()
    {
        playerScript.ToggleIsPaused();
    }
}
