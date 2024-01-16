using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        GameManager.instance.presentScene = Scene.Lobby;
        SceneManager.LoadScene("Lobby");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
