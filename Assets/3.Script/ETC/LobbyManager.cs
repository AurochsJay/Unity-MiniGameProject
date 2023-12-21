using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] public GameObject gKeyImage;
    [SerializeField] public GameObject showGameTitleImage;
    [SerializeField] private Text gameNameText;
    private string gameName;

    private void Update()
    {
        gameName = gameNameText.text;
    }

    public void ClickGameTitle()
    {
        SceneManager.LoadScene(gameName);
    }
    
}
