using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private InputManager input;
    [SerializeField] public GameObject gKeyImage;
    [SerializeField] public GameObject showGameTitleImage;
    [SerializeField] private GameObject[] gameNameText;
    private string gameName;

    private void Start()
    {
        input = GameObject.Find("GameManager").GetComponent<InputManager>();
    }

    private void Update()
    {
        if (showGameTitleImage.activeSelf)
        {
            SelectGameInUI();
        }
    }

    private void SelectGameInUI()
    {
        gameName = gameNameText[1].GetComponent<Text>().text;

        if(input.press_DownArrow)
        {
            TextScrollingDown();
        }
        else if(input.press_UpArrow)
        {
            TextScrollingUp();
        }
        

        if(input.press_Enter)
        {
            SelectGameTitle();
        }
    }

    // 1 -> 0, 0 -> 2, 2 -> 1
    private void TextScrollingDown()
    {
        for (int i = 0; i< gameNameText.Length-1; i++)
        {
            Swap(i);
        }
    }

    // 1 -> 2, 2-> 0, 0 -> 1
    private void TextScrollingUp()
    {
        for (int i = gameNameText.Length - 2; i >= 0; i--)
        {
            Swap(i);
        }
    }

    private void Swap(int i)
    {
        string temp = gameNameText[i].GetComponent<Text>().text;
        gameNameText[i].GetComponent<Text>().text = gameNameText[i + 1].GetComponent<Text>().text;
        gameNameText[i+1].GetComponent<Text>().text = temp;
    }

    private void SelectGameTitle()
    {
        UpdateSceneName(gameName);
        SceneManager.LoadScene(gameName);
    }

    public void ClickGameTitle()
    {
        UpdateSceneName(gameName);
        SceneManager.LoadScene(gameName);
    }

    private void UpdateSceneName(string gameName)
    {
        switch (gameName)
        {
            case "Tetris":
                GameManager.instance.presentScene = Scene.Tetris;
                break;
            case "Snake":
                GameManager.instance.presentScene = Scene.Snake;
                break;
            case "JJump":
                GameManager.instance.presentScene = Scene.JJump;
                break;
        }

    }

}
